
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;

namespace Starfury
{
    class Starfury : GameWindow
    {
		public static Display display = new Display();
		public static List<ISurface> surfaces = new List<ISurface>();
		public static Stopwatch Time = Stopwatch.StartNew();
		public static IMode currentMode = null;
		public bool renderNeeded = true;
		public static Xkb.Context context;
		public static Xkb.Keymap keymap;
		public static Xkb.State state;

		public static void RemoveSurface(ISurface resource)
		{
			surfaces.Remove(resource);
		}

		public static void RemoveSurface(IntPtr resourcePointer)
		{
			ISurface resource = surfaces.Find(r => ((Resource) r).resource == resourcePointer);
			Console.WriteLine("Found surface: " + resource);
			surfaces.Remove(resource);
		}

		public static ISurface GetSurface(IntPtr surfacePtr)
		{
			return surfaces.Find(r => ((Resource) r).resource == surfacePtr);
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Title = "Compositor.NET";
			GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
			context = new Xkb.Context();
			keymap = new Xkb.Keymap(context, "evdev", "apple", "gb", "", "");
			state = new Xkb.State(keymap);
		}

		protected override void OnResize(EventArgs e)
		{
  			base.OnResize(e);
            //GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			display.Terminate();
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}

		private void InitializeWayland()
		{
			Console.WriteLine("Display: {0}", display);
			Console.WriteLine("Socket: {0}", display.AddSocketAuto());			
			
			display.InitSHM();

			WaylandInterfaces.Initialize();
			XdgShellUnstableV6Interfaces.Initialize();
			
			OutputGlobal outputGlobal = new OutputGlobal();
			display.GlobalCreate(outputGlobal, 2);

			CompositorGlobal compositorGlobal = new CompositorGlobal();
			display.GlobalCreate(compositorGlobal, 3);

			ShellGlobal shellGlobal = new ShellGlobal();
			display.GlobalCreate(shellGlobal, 1);

			SeatGlobal seatGlobal = new SeatGlobal();
			display.GlobalCreate(seatGlobal, 3);

			DataDeviceManagerGlobal dataDeviceManagerGlobal = new DataDeviceManagerGlobal();
			display.GlobalCreate(dataDeviceManagerGlobal, 3);
			
			SubcompositorGlobal subcompositorGlobal = new SubcompositorGlobal();
			display.GlobalCreate(subcompositorGlobal, 1);

			XdgShellV6Global xdgShellV6Global = new XdgShellV6Global();
			display.GlobalCreate(xdgShellV6Global, 1);
		}
		
		static void Main(string[] args) {

			Starfury compositor = new Starfury();

			Console.CancelKeyPress +=
				(sender, e) => {
					Console.WriteLine("CTRL+C detected!\n");
					display.Terminate();
					System.Diagnostics.Process.GetCurrentProcess().Kill();
			};

			compositor.KeyDown += delegate (object sender, KeyboardKeyEventArgs e) {
                Console.WriteLine(e.Key);
            };

			compositor.KeyUp += delegate (object sender, KeyboardKeyEventArgs e) {
                Console.WriteLine(e.Key);
            };
			
			compositor.InitializeWayland();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			/*
				Ideally we'd use a single thread and wait on file descriptors
				for Wayland, libinput and DRM events. Instead here we have
				separate threads for OpenTK, Wayland and libinput (via OpenTK).
				OpenTK by design wants to constantly render even if we have no
				changes in what is being displayed. So that we don't use 100%
				CPU, the main thread sleeps every loop. This reduces CPU load
				to a low level but is still not ideal. Need to think of a way
				that we may wait within RenderFrame if there has been no
				activity on libinput or wayland.
			*/
			
			compositor.Load += (sender, e) =>
			{		
				DesktopMode desktopMode = new DesktopMode(compositor.Width, compositor.Height);
				currentMode = desktopMode;
			};

			compositor.RenderFrame += (sender, e) =>
            {
				//Console.WriteLine("Dispatching");
				display.GetEventLoop().Dispatch(5);
				display.FlushClients();

				if (compositor.renderNeeded)
				{

            		GL.Viewport(0, 0, compositor.Width, compositor.Height);
                	GL.Clear(ClearBufferMask.ColorBufferBit);

					//Console.WriteLine("Beginning render");
					currentMode.Render(compositor.Width, compositor.Height);
					compositor.SwapBuffers();
				}
				// During initial development use the following
				// to check we're not collecting memory that should
				// be kept.
				GC.Collect();
				GC.WaitForPendingFinalizers();
            };
        	compositor.VSync = VSyncMode.Off;
			compositor.Run(60.0);
		}
    }
}
