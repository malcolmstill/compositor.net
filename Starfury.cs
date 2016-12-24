
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using XdgShellUnstableV6.Server.Protocol;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
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
    	static AutoResetEvent waylandFinished;
    	static AutoResetEvent renderFinished;    

		public static void RemoveSurface(ISurface resource)
		{
			surfaces.Remove(resource);
		}

		public static void RemoveSurface(IntPtr resourcePointer)
		{
			ISurface resource = surfaces.Find(r => ((Resource) r).resource == resourcePointer);
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

		public void RunWayland()
		{
			while (true)
			{			
				//Console.WriteLine("Locked wayland thread;");
				//Console.WriteLine("WAYLAND: Wayland waiting on render...");
				renderFinished.WaitOne();
				Console.WriteLine("WAYLAND: Wayland starting");
				display.GetEventLoop().Dispatch(-1);
				display.FlushClients();
				Console.WriteLine("WAYLAND: Wayland finished");
				waylandFinished.Set();
			}
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

        	waylandFinished = new AutoResetEvent(true);  
			renderFinished = new AutoResetEvent(false);
			Thread waylandThread = new Thread(compositor.RunWayland);
			waylandThread.Start();
			GC.Collect();
			GC.WaitForPendingFinalizers();	
			//display.Run();

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
			
			compositor.RenderFrame += (sender, e) =>
            {
				//Console.WriteLine("RENDER: Render waiting on wayland");
				waylandFinished.WaitOne();
				Console.WriteLine("RENDER: Render starting");
                GL.Clear(ClearBufferMask.ColorBufferBit);
				//display.GetEventLoop().Dispatch(5);
				//display.FlushClients();

					lock (surfaces)
					{
						foreach (ISurface s in surfaces)
						{
							SfSurface surface = s.GetSurface();
							lock (surface)
							{
								if (surface.committed)
								{
									if (surface.callback != null)
									{
										surface.callback.SendDone(0);
										surface.callback.Remove();
										surface.callback = null;
									}
									surface.committed = false;
								}
							}
						}
					}

					display.FlushClients();
					Console.WriteLine("RENDER: render finished");
					renderFinished.Set();
				

                compositor.SwapBuffers();
				// Sleep some time to not use 100% CPU...this is a hack
				// Ideally we want to yield to the OS until there is Input
				// from libinput or wayland.
            };
        	compositor.VSync = VSyncMode.On;
			compositor.Run(60.0);
		}
    }
}
