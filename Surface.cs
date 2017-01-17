
using System;
using Wayland.Server;
using Wayland.Server.Protocol;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace WindowManager
{
    public class WMSurface : WlSurface, ISurface
    {
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;
		public int Width { get; set; } = -1;
		public int Height { get; set; } = -1;
		public int originX { get; set; } = 0;
		public int originY { get; set; } = 0;
		public IntPtr buffer = IntPtr.Zero;
		public WlCallback callback { get; set; }
		public WMRegion InputRegion { get; set; }
		public WMRegion OpaqueRegion { get; set; }
		//public WlCallback callback = new WlCallback(client, 0, false);
		public bool callbackNeeded = false;
		// public Int32 width { get; set; } = 0;
		// public Int32 height { get; set; } = 0;
		public bool committed = false;
		public int texture = -1;
		// We may need to get the role object from the WMSurface (for example getting the correct parent object in subcompositor)
		// Could just store a role object that gets updated:
		public ISurface Role { get; set; }
		public List<ISurface> Subsurfaces { get; set; } = new List<ISurface>();
		Vector3[] verts;
		Vector2[] texcoords;
		uint[] indices;
		bool geometryChanged = true;
		int vertbo;
		int texcoordbo;
		int ebo;


		public WMSurface(IntPtr client, Int32 version, UInt32 id) : base(client, version, id)
		{
			// Call the base class constructor
			Role = this;
		}

		public override string ToString()
		{
			return String.Format("WMSurface@{0:X8}", resource.ToInt32());
		}

		public override void Attach(IntPtr client, IntPtr resource, IntPtr buffer, Int32 x, Int32 y)
		{
			this.buffer = buffer;
		}
		
		public override void Frame(IntPtr client, IntPtr resource, UInt32 callbackId)
		{
			/*
			if (callbackNeeded)
			{
				callback.SendDone((UInt32) WindowManager.Time.ElapsedMilliseconds); // Don't need remove because we pass in false
				callback.Remove();
			}
			*/

			// Instead of making a new WlCallback for every Frame just create a resource on the libwayland side
			if (callback == null)
			{
				callback = new WlCallback(client, 1, callbackId, false);
				WindowManager.WlCallbackCount += 1;
			}
			/*
			else
			{
				//callback.Remove();
				callback.resource = Resource.Create(client, WaylandInterfaces.wlCallbackInterface.ifaceNative, 1, callbackId);
			}
			callbackNeeded = true;
			*/
		}

		public override void Commit(IntPtr client, IntPtr resource)
		{
			/*
			We set committed to true so that we know to draw it. However, a commit can be
			made without and associated buffer (during initialisation typically). So that
			we are drawing completed output, we check that there is an associated buffer
			before setting committed true. This came about because there was a flash of
			subsurfaces not being in the correct location with weston-subsurfaces.
			*/
			if (buffer != IntPtr.Zero)
			{
				committed = true;
			}
			this.CreateTexture();
			WindowManager.renderNeeded = true;
		}

		public override void SetInputRegion(IntPtr client, IntPtr resource, IntPtr region)
		{
			//WMSurface surface = WindowManager.FindSurface(Resource.GetUserData(resource));
			InputRegion = (WMRegion) this.client.FindResource(region);
		}

		public override void SetOpaqueRegion(IntPtr client, IntPtr resource, IntPtr region)
		{
			
		}

		public override void SetBufferScale(IntPtr client, IntPtr resource, Int32 scale)
		{

		}

		public override void Damage(IntPtr client, IntPtr resource, Int32 x, Int32 y, Int32 width, Int32 height)
		{

		}

		public override void Delete(IntPtr resource)
		{
			WindowManager.RemoveSurface(this.resource);
			WindowManager.renderNeeded = true;
		}

		public WMSurface Surface
		{
			get {
				return this;
			}
		}

		public void Render()
		{
			if (callback != null)
			{
				callback.SendDone(0);
				callback = null;
			}
		}

		public void Activate()
		{

		}

		public void Deactivate()
		{

		}

		public void SendMouseButton(uint time, uint button, uint state)
		{
			client.pointer?.SendButton(0, time, button, state);
		}

		public void SendMouseEnter(int x, int y)
		{
			client.pointer?.SendEnter(0, this.resource, x, y);
		}

		public void SendMouseLeave()
		{
			client.pointer?.SendLeave(0, this.resource);
		}

		public void SendMouseMove(uint time, int x, int y)
		{
			client.pointer?.SendMotion(time, 256 * x, 256 * y);
		}

		public void SendKeyboardEnter()
		{
			WlArray array = new WlArray();
			client.keyboard?.SendEnter(0, this.resource, array.array);
		}
		
		public void SendKeyboardLeave()
		{
			client.keyboard?.SendLeave(0, this.resource);
		}

		public void SendKey(uint time, uint key, uint state)
		{
			client.keyboard?.SendKey(0, time, key, state);
		}

		public void SendMods(uint mods_depressed, uint mods_latched, uint mods_locked, uint group)
		{
			client.keyboard?.SendModifiers(0, mods_depressed, mods_latched, mods_locked, group);
		}

		public void SendDone()
		{
			/*
			if (callbackNeeded)
			{
				callback.SendDone((UInt32) WindowManager.Time.ElapsedMilliseconds);
				callback.Remove();
			}
			callbackNeeded = false;
			*/
			if (callback != null)
			{
				callback.SendDone((UInt32) WindowManager.Time.ElapsedMilliseconds);
				callback.Remove();
				callback = null;
			}
		}

		public int Texture()
		{
			return texture;
		}

		public bool PointInSurface(int x, int y)
		{
			return x >= this.X && x <= (this.X + this.Width) && y >= this.Y && y <= (this.Y + this.Height);
		}

		public void Geometry(int vertAttrib, int texcoordAttrib)
		{
			if (geometryChanged)
			{
				verts = new Vector3[] { new Vector3(0.0f, 0.0f, 0.0f),
							  			new Vector3(0.0f, this.Height, 0.0f),
							  			new Vector3(this.Width, this.Height, 0.0f),
							  			new Vector3(this.Width, 0.0f, 0.0f) };

				texcoords = new Vector2[] { new Vector2(0.0f, 0.0f),
											new Vector2(0.0f, 1.0f),
											new Vector2(1.0f, 1.0f),
											new Vector2(1.0f, 0.0f) };

				indices = new uint[] { 0,1,2, 2,3,0 };

					
				geometryChanged = false;
			}
			
			if (vertbo > 0)
			{
				GL.DeleteBuffers(1, ref vertbo);
			}
			GL.GenBuffers(1, out vertbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertbo);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(verts.Length * Vector3.SizeInBytes), verts, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(vertAttrib, 3, VertexAttribPointerType.Float, false, 0, 0);

			if (texcoordbo > 0)
			{
				GL.DeleteBuffers(1, ref texcoordbo);
			}
			GL.GenBuffers(1, out texcoordbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, texcoordbo);
			GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(texcoords.Length * Vector2.SizeInBytes), texcoords, BufferUsageHint.StaticDraw);
			GL.VertexAttribPointer(texcoordAttrib, 2, VertexAttribPointerType.Float, false, 0, 0);

			if (ebo > 0)
			{
				GL.DeleteBuffers(1, ref ebo);
			}
			GL.GenBuffers(1, out ebo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
			GL.BufferData<uint>(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * Marshal.SizeOf(typeof(uint))), indices, BufferUsageHint.StaticDraw);
				
		}

		public void CreateTexture()
		{
			if (buffer != IntPtr.Zero)
			{
				SHMBuffer shmBuffer = new SHMBuffer(buffer);
				int newWidth = shmBuffer.GetWidth();
				int newHeight = shmBuffer.GetHeight();
				//Console.WriteLine("SHM buffer format:" + shmBuffer.GetFormat());

				//Console.WriteLine("SHM buffer stride:" + shmBuffer.GetStride());
				if (newHeight != Height || newWidth != Width)
				{
					geometryChanged = true;
					Width = newWidth;
					Height = newHeight;
				} 
				IntPtr data = shmBuffer.GetData();

				if (texture >= 0)
				{
					GL.DeleteTextures(1, ref texture);
				}

				GL.Enable(EnableCap.Texture2D);
				GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
				GL.GenTextures(1, out texture);
            	GL.BindTexture(TextureTarget.Texture2D, texture);
            	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            	GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);

            	GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
				Utils.ReleaseBuffer(buffer);
				buffer = IntPtr.Zero;
			}
		}
    }
}
