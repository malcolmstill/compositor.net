using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Xkb
{
    public class Context
    {
        public IntPtr Pointer { get; set; }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_context_new")]
        private static extern IntPtr NewContext(UInt32 flags);
        public Context()
        {
            Pointer = NewContext(0);
        }

        public Context(UInt32 flags)
        {
            Pointer = NewContext(flags);
        }
    }

    public class Keymap
    {
        public IntPtr Pointer { get; set; }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct RuleNames
        {
            public IntPtr rules;
            public IntPtr model;
            public IntPtr layout;
            public IntPtr variant;
            public IntPtr options;
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_keymap_new_from_names")]
        private static extern IntPtr xkb_keymap_new_from_names(IntPtr context, IntPtr names, UInt32 flags);

        public Keymap(Context context, string rules, string model, string layout, string variant, string options)
        {
            RuleNames names = new RuleNames();
            names.rules = Marshal.StringToHGlobalAnsi(rules);
            names.model = Marshal.StringToHGlobalAnsi(model);
            names.layout = Marshal.StringToHGlobalAnsi(layout);
            names.variant = Marshal.StringToHGlobalAnsi(variant);
            names.options = Marshal.StringToHGlobalAnsi(options);
            IntPtr namesUnmanaged = Marshal.AllocHGlobal(Marshal.SizeOf(names));
            Marshal.StructureToPtr(names, namesUnmanaged, false);
            Pointer = xkb_keymap_new_from_names(context.Pointer, namesUnmanaged, 0);
            Marshal.FreeHGlobal(namesUnmanaged);
            Marshal.FreeHGlobal(names.rules);
            Marshal.FreeHGlobal(names.model);
            Marshal.FreeHGlobal(names.layout);
            Marshal.FreeHGlobal(names.variant);
            Marshal.FreeHGlobal(names.options);
        }
                
        [DllImport("libxkbcommon.so", EntryPoint="xkb_keymap_get_as_string")]
        private static extern IntPtr xkb_keymap_get_as_string(IntPtr keymap, int format);

        [DllImport("libc")]
        private static extern IntPtr getenv(string variable);

        [DllImport("libc")]
        private static extern int mkstemp(string file);

        [DllImport("libc")]
        private static extern int ftruncate(int fd, int length);

        [DllImport("libc")]
        private static extern IntPtr mmap(IntPtr addr, uint length, int prot, int flags, int fd, int offset);
        
        [DllImport("libc")]
        private static extern int munmap(IntPtr addr, int length);

        public Tuple<int,int> GetKeymap()
        {
            return GetKeymap(1);
        }

        public Tuple<int,int> GetKeymap(int format)
        {
            string keymapString = Marshal.PtrToStringAnsi(xkb_keymap_get_as_string(Pointer, format));
            string dir = Marshal.PtrToStringAnsi(getenv("XDG_RUNTIME_DIR"));
            int fd = mkstemp(dir + "/keymapXXXXXX");
            int size = keymapString.Length + 1;
            ftruncate(fd, size);
            IntPtr map = mmap(IntPtr.Zero, (uint) size, 1 | 2, 1, fd, 0);
            Marshal.Copy(Encoding.ASCII.GetBytes(keymapString), 0, map, keymapString.ToCharArray().Length * Marshal.SizeOf(typeof(Char)));
            munmap(map, size);
            return new Tuple<int,int>(fd, size);
        }
        
        [DllImport("libxkbcommon.so", EntryPoint="xkb_keymap_key_by_name")]
        private static extern UInt32 xkb_keymap_key_by_name(IntPtr keymap, string name);
        public UInt32 KeyByName(string name)
        {
            return xkb_keymap_key_by_name(Pointer, name);
        }


        [DllImport("libxkbcommon.so", EntryPoint="xkb_keymap_min_keycode")]
        private static extern UInt32 xkb_keymap_min_keycode(IntPtr keymap);
        public UInt32 MinKeycode()
        {
            return xkb_keymap_min_keycode(Pointer);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_keymap_max_keycode")]
        private static extern UInt32 xkb_keymap_max_keycode(IntPtr keymap);
        public UInt32 MaxKeycode()
        {
            return xkb_keymap_max_keycode(Pointer);
        }
    }

    public class State
    {
        public IntPtr Pointer { get; set; }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_new")]
        public static extern IntPtr xkb_state_new(IntPtr keymap);

        public State(Keymap keymap)
        {
            Pointer = xkb_state_new(keymap.Pointer);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_key_get_one_sym")]
        private static extern UInt32 xkb_state_key_get_one_sym(IntPtr state, UInt32 keycode);
        public UInt32 KeyGetOneSym(UInt32 keycode)
        {
            return xkb_state_key_get_one_sym(Pointer, keycode);
        }
        
        [DllImport("libxkbcommon.so", EntryPoint="xkb_keysym_get_name")]
        private static extern int xkb_keysym_get_name(UInt32 keysym, string buffer, UInt32 size);

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_mod_name_is_active")]
        private static extern int xkb_state_mod_name_is_active(IntPtr state, string modifier, int type);
        public int ModNameIsActive(string modifier, int type)
        {
            return xkb_state_mod_name_is_active(Pointer, modifier, type);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_update_key")]
        private static extern int xkb_state_update_key(IntPtr state, UInt32 key, int direction);
        public int UpdateKey(UInt32 key, int direction)
        {
            return xkb_state_update_key(Pointer, key, direction);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_serialize_mods")]
        private static extern UInt32 xkb_state_serialize_mods(IntPtr state, int components);
        public UInt32 SerializeMods(int components)
        {
            return xkb_state_serialize_mods(Pointer, components);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_serialize_layout")]
        private static extern UInt32 xkb_state_serialize_layout(IntPtr state, int components);
        public UInt32 SerializeLayout(int components)
        {
            return xkb_state_serialize_layout(Pointer, components);
        }

        [DllImport("libxkbcommon.so", EntryPoint="xkb_state_unref")]
        private static extern void xkb_state_unref(IntPtr state);
        public void Unref()
        {
            xkb_state_unref(Pointer);   
        }
    }
}