using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LowLevelInput.Converters;
using LowLevelInput.Hooks;

namespace GameJam
{
    class Program
    {
        public static bool Exit { get; private set; }
        public static int Clicks { get; private set; }
        public static int DelayTime { get; private set; } // in miliseconds - define in SetUpVars().
        public static LowLevelKeyboardHook KeyboardHook { get; private set; }
        public static LowLevelMouseHook MouseHook { get; private set; }

        static void Main(string[] args)
        {
            SetUp();
            while (!Exit)
            {
                Update();
                Thread.Sleep(DelayTime);
            }
            SetDown();
        }

        //The Core off applications is here
        #region SetUp/SetDown/CheckUps/Updates
        private static void SetUp()
        {
            SetUpVars();
            // Disable the Console Edit mode, in way to never lock the main thread during execution due to selections on the window.
            DisableQuickEdit();
            // Initialize Hookers who deal with all the inputs after now.
            KeyboardHook = new LowLevelKeyboardHook();
            MouseHook = new LowLevelMouseHook();
            // Defines actions to events.
            KeyboardHook.OnKeyboardEvent += KeyboardHook_OnKeyboardEvent;
            MouseHook.OnMouseEvent += MouseHook_OnMouseEvent;
            // Attach the hookers into main thread.
            KeyboardHook.InstallHook();
            MouseHook.InstallHook();
        }

        //Runs after Esc pressed, defines the closing behaviour.
        private static void SetDown()
        {
            MouseHook.Dispose();
            KeyboardHook.Dispose();
        }

        // On this call, defines all the default values for the properties
        private static void SetUpVars()
        {
            Exit = false;
            Clicks = 0;
            DelayTime = 50;
        }

        //This function runs every x mileseconds, defined on the SetUpVars call.
        private static void Update()
        {
            //CheckStuff();
            UpdateUI();
        }
        //This method runs every x milesconds, inside the Update method,
        private static void UpdateUI()
        {
            Console.Clear();
            Console.WriteLine("Clicks --- {0} ", Clicks);
        }
        //All the Checkings that needs to run every frame goes here, this is the first method that run on Update().
        private static void CheckStuff()
        {

        }
        #endregion

        // Add keyBindings Here
        #region Mouse/Keyboard Hooks Interactions
        //This is the cherry on top of the cake, this method handle the mouse clicks and has ONLY ONE FUNCTION, update the Click property... ^_^
        private static void MouseHook_OnMouseEvent(VirtualKeyCode key, KeyState state, int x, int y)
        {
            if (state == KeyState.Down) Clicks++;
        }

        //This method deals with all the keyboard inputs, once the Console.Reads doesn't work anymore. 
        private static void KeyboardHook_OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            switch (key)
            {
                case VirtualKeyCode.Escape:
                    Exit = true;
                    break;
            }
        }
        #endregion

        //This region deals with the QuickEdit mode included in console.
        #region defines console edit mode

        public const int STD_INPUT_HANDLE = -10; // to disable quick edit

        // config to disable close button start

        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
            [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        // config to disable close button end


        // configurations to disable quick edit mode start
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int nStdHandle);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(
            IntPtr hConsoleHandle,
            out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(
            IntPtr hConsoleHandle,
            int ioMode);

        const int QuickEditMode = 64;

        const int ExtendedFlags = 128;


        static void DisableQuickEdit()
        {
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_CLOSE, MF_BYCOMMAND);
            IntPtr conHandle = GetStdHandle(STD_INPUT_HANDLE);
            int mode;

            if (!GetConsoleMode(conHandle, out mode))
            {
                // error getting the console mode. Exit.
                return;
            }

            mode = mode & ~(QuickEditMode | ExtendedFlags);

            if (!SetConsoleMode(conHandle, mode))
            {
                // error setting console mode.
            }
        }

        //static void EnableQuickEdit()
        //{
        //    IntPtr conHandle = GetStdHandle(STD_INPUT_HANDLE);
        //    int mode;

        //    if (!GetConsoleMode(conHandle, out mode))
        //    {
        //        // error getting the console mode. Exit.
        //        return;
        //    }

        //    mode = mode | (QuickEditMode | ExtendedFlags);

        //    if (!SetConsoleMode(conHandle, mode))
        //    {
        //        // error setting console mode.
        //    }
        //}
        // configurations to disable quick edit mode end

        #endregion

    }
}
