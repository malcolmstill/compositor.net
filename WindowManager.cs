
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

namespace WindowManager
{
    class WindowManager : GameWindow
    {
		public static WindowManager Compositor { get; set; }
		public static Display display = new Display();
		// public static List<ISurface> Surfaces = new List<ISurface>(); // store a list of all surfaces regardless of view
		public static List<WMSurface> Surfaces = new List<WMSurface>();
		public static List<VirtualDesktop> VirtualDesktops = new List<VirtualDesktop>();
		public static Stopwatch Time = Stopwatch.StartNew();
		long lastTime = 0;
		//public static IMode currentMode = null;
		public static VirtualDesktop CurrentVirtualDesktop;
		public static bool renderNeeded = true;
		public static Xkb.Context context;
		public static Xkb.Keymap keymap;
		public static Xkb.State state;
		public static uint[] Mods = new uint[4];
		public static MovingSurface MovingSurface = null;
		public static ISurface ResizingSurface = null;
		public static ISurface PointerSurface = null;
		public static int WlCallbackCount = 0;
		public static TTY tty;

		/*
		So previously surfaces were swapped out of the list of Surfaces when their role changed
		RemoveSurface was only removing the specialised WlResource
		*/

		public static void RemoveSurface(ISurface surface)
		{
			if (surface == PointerSurface)
			{
				PointerSurface = null;
			}
			if (surface != null)
			{
				Surfaces.Remove(surface.Surface);
				foreach (VirtualDesktop virtualDesktop in VirtualDesktops)
				{
					virtualDesktop.RemoveSurface(surface.Surface.Role);
				}
			}
		}

		public static void RemoveSurface(IntPtr resourcePointer)
		{
			ISurface surface = Surfaces.Find(s => ((Resource) s.Role).resource == resourcePointer);
			RemoveSurface(surface);
		}

		public static WMSurface FindSurface(IntPtr surfacePtr)
		{
			return Surfaces.Find(s => s.resource == surfacePtr);
		}

		public void Cleanup()
		{
			Console.WriteLine("Cleaning up");
			tty.ExitTTY();				
			display.Terminate();
			this.Dispose(); // Reset CRTC properly
    		System.Environment.Exit(1);	
		}
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			Title = "WindowManager.NET";
			GL.ClearColor(0.3f, 0.3f, 0.3f, 1.0f);
			context = new Xkb.Context();
			keymap = new Xkb.Keymap(context, "evdev", "apple", "gb", "", "");
			state = new Xkb.State(keymap);
			EvdevKeyboard.Initialize();

			tty = new TTY();
		}

		protected override void OnResize(EventArgs e)
		{
  			base.OnResize(e);
			WindowManager.renderNeeded = true;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			this.Cleanup();
			base.OnClosing(e);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);
			display.GetEventLoop().Dispatch(10);
			display.FlushClients();

			//foreach(WMSurface surface in Surfaces)
			//{
			//	Console.WriteLine(surface.client);
			//}
			if (WindowManager.renderNeeded)
			{
            	GL.Viewport(0, 0, Width, Height);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

				CurrentVirtualDesktop.CurrentMode.Render(Width, Height);
				SwapBuffers();
				WindowManager.renderNeeded = false;
			}
			
			if (Time.ElapsedMilliseconds - lastTime > 5000)
			{
				//Console.WriteLine("COLLECTING AT: " + Time.ElapsedMilliseconds);
				GC.Collect();
				GC.WaitForPendingFinalizers();
				lastTime = Time.ElapsedMilliseconds;
			}
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

			WindowManager compositor = new WindowManager();
			WindowManager.Compositor = compositor;

			Console.CancelKeyPress +=
				(sender, e) => {
					Console.WriteLine("CTRL+C detected!\n");
					compositor.Cleanup();
			};

			compositor.KeyDown += delegate (object sender, KeyboardKeyEventArgs e)
			{
				if(!e.IsRepeat)
				{
                	Console.WriteLine(e.Key);
					if (e.Key == Key.Q)
					{
						compositor.Cleanup();
					}
					else
					{
						state.KeyGetOneSym(EvdevKeyboard.Keycode[e.Key] + 8);
						state.UpdateKey(EvdevKeyboard.Keycode[e.Key] + 8, 1);
						Mods[0] = state.SerializeMods(1);
						Mods[1] = state.SerializeMods(2);
						Mods[2] = state.SerializeMods(4);
						Mods[3] = state.SerializeMods(64);
						CurrentVirtualDesktop.CurrentMode.KeyPress((uint) WindowManager.Time.ElapsedMilliseconds, EvdevKeyboard.Keycode[e.Key], 1);	
					}
				}

            };

			compositor.KeyUp += delegate (object sender, KeyboardKeyEventArgs e)
			{
				if(!e.IsRepeat)
				{
					state.KeyGetOneSym(EvdevKeyboard.Keycode[e.Key] + 8);
					state.UpdateKey(EvdevKeyboard.Keycode[e.Key] + 8, 0);
					Mods[0] = state.SerializeMods(1);
					Mods[1] = state.SerializeMods(2);
					Mods[2] = state.SerializeMods(4);
					Mods[3] = state.SerializeMods(64);
					CurrentVirtualDesktop.CurrentMode.KeyPress((uint) WindowManager.Time.ElapsedMilliseconds, EvdevKeyboard.Keycode[e.Key], 0);
				}
            };

			compositor.Mouse.ButtonDown += delegate (object sender, MouseButtonEventArgs e)
			{
				CurrentVirtualDesktop.CurrentMode.MouseButton((uint) WindowManager.Time.ElapsedMilliseconds, 0x110, 1);
			};

			compositor.Mouse.ButtonUp += delegate (object sender, MouseButtonEventArgs e)
			{
				CurrentVirtualDesktop.CurrentMode.MouseButton((uint) WindowManager.Time.ElapsedMilliseconds, 0x110, 0);
			};

			compositor.Mouse.Move += delegate (object sender, MouseMoveEventArgs e)
			{
				CurrentVirtualDesktop.CurrentMode.MouseMove((uint) WindowManager.Time.ElapsedMilliseconds, compositor.Mouse.X, compositor.Mouse.Y);
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
				CurrentVirtualDesktop = new VirtualDesktop(new DesktopMode());
				VirtualDesktops.Add(CurrentVirtualDesktop);
			};

			/*
			compositor.RenderFrame += (sender, e) =>
            {

				// During initial development use the following
				// to check we're not collecting memory that should
				// be kept.
				//GC.Collect();
				//GC.WaitForPendingFinalizers();
            };
			*/
			compositor.Keyboard.KeyRepeat = false;
        	compositor.VSync = VSyncMode.Off;
			compositor.Run(1.0, 0.0);
		}
    }
}
