
using System;
using System.Runtime.InteropServices;

namespace WindowManager
{
    public class TTY
    {
        int fd;
        int kbmode;

        public TTY()
        {
            fd = open("/dev/tty", OpenFlags.ReadWrite | OpenFlags.NoCTTY);
			IntPtr kbmodeptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)));
			ioctl(fd, KeyboardIoctlCode.GetMode, kbmodeptr);
			kbmode = Marshal.ReadInt32(kbmodeptr);
			ioctl(fd, KeyboardIoctlCode.SetMode, 0x04);	
        }

        public void ExitTTY()
        {
            ioctl(fd, KeyboardIoctlCode.SetMode, kbmode);
			close(fd);	
        }

        [DllImport("libc")]
        public static extern int ioctl(int d, KeyboardIoctlCode request, IntPtr data);

        [DllImport("libc")]
        public static extern int ioctl(int d, KeyboardIoctlCode request, int data);

        [DllImport("libc")]
        public static extern int open([MarshalAs(UnmanagedType.LPStr)]string pathname, OpenFlags flags);
        
		[DllImport("libc")]
        public static extern int close(int fd);

		[Flags]
    	public enum OpenFlags
    	{
        	ReadOnly = 0x0000,
        	WriteOnly = 0x0001,
        	ReadWrite = 0x0002,
        	NonBlock = 0x0800,
			NoCTTY = 256,
        	CloseOnExec = 0x0080000
    	}

    	public enum KeyboardIoctlCode
    	{
	        GetMode = 0x4b44,
    	    SetMode = 0x4b45,
    	}
    }

}