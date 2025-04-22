using System;
using System.Runtime.InteropServices;

public class NativeDialog
{
    #if UNITY_STANDALONE_WIN

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, int type);

        public static void ShowMessage(string message, string title = "")
        {
            MessageBox(IntPtr.Zero, message, title, 0x40);
        }

    #elif UNITY_STANDALONE_OSX

        [DllImport("libobjc.dylib", EntryPoint = "objc_getClass")]
        private static extern IntPtr objc_getClass(string className);

        [DllImport("libobjc.dylib", EntryPoint = "sel_registerName")]
        private static extern IntPtr sel_registerName(string selector);

        // objc_msgSend の異なるシグネチャを明示的に定義
        [DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg);

        [DllImport("libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern IntPtr IntPtr_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg);

        public static void ShowMessage(string message, string title = "")
        {
            IntPtr alertClass = objc_getClass("NSAlert");
            IntPtr alloc = sel_registerName("alloc");
            IntPtr init = sel_registerName("init");

            IntPtr alertInstance = IntPtr_objc_msgSend(alertClass, alloc);
            alertInstance = IntPtr_objc_msgSend(alertInstance, init);

            IntPtr setMessageText = sel_registerName("setMessageText:");
            void_objc_msgSend_IntPtr(alertInstance, setMessageText, NSStringFromString(title));

            IntPtr setInformativeText = sel_registerName("setInformativeText:");
            void_objc_msgSend_IntPtr(alertInstance, setInformativeText, NSStringFromString(message));

            IntPtr runModal = sel_registerName("runModal");
            void_objc_msgSend(alertInstance, runModal);
        }

        private static IntPtr NSStringFromString(string str)
        {
            return IntPtr_objc_msgSend_IntPtr(objc_getClass("NSString"), sel_registerName("stringWithUTF8String:"), Marshal.StringToHGlobalAuto(str));
        }

    #endif
}