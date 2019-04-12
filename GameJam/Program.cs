using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using LowLevelInput.Converters;
using LowLevelInput.Hooks;
using System.Windows.Forms;
using GameJam.Models;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using Console = Colorful.Console;

namespace GameJam
{
    class Program
    {

        public static bool Exit { get; private set; }
        public static int Breads { get; private set; }
        public static int BreadsPerSecond { get; private set; }
        public static int BreadsPerClick { get; private set; }
        public static int DelayTime { get; private set; } // in miliseconds - define in SetUpVars().
        public static LowLevelKeyboardHook KeyboardHook { get; private set; }
        public static LowLevelMouseHook MouseHook { get; private set; }
        public static GameData Data { get; set; }
        public static string DefaultDB { get; private set; }
        public static Thread t1 { get; set; }

        public static Color Red;
        public static Color White;
        public static Color Green;
        
        public static Color[] Colors;
        public static string[] Names;


        static void Main(string[] args)
        {
            SetUp();
            BPS();
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
            GetData();
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

        private static void GetData()
        {
            try
            {
                Data = JsonConvert.DeserializeObject<GameData>(File.ReadAllText("data.json"));
            }
            catch
            {
                NewGame();
            }
        }

        //Runs after Esc pressed, defines the closing behaviour.
        private static void SetDown()
        {
            MouseHook.Dispose();
            KeyboardHook.Dispose();
            PersistChanges();
        }

        private static void PersistChanges()
        {
            Data.Breads = Breads;
            Data.BreadsPerSecond = BreadsPerSecond;
            Data.BreadsPerClick = BreadsPerClick;
            var json = JsonConvert.SerializeObject(Data);
            File.WriteAllText("data.json", json);

        }

        // On this call, defines all the default values for the properties
        private static void SetUpVars()
        {
            Console.SetWindowSize(136, 27);
            Red = Color.FromArgb(255, 82, 82);
            White = Color.FromArgb(255, 255, 255);
            Green = Color.FromArgb(81, 255, 178);
            Colors = new Color[20];
            Names = new string[20];
            Exit = false;
            Breads = Data.Breads;
            BreadsPerSecond = Data.BreadsPerSecond;
            BreadsPerClick = Data.BreadsPerClick;
            DelayTime = 150;


        }

        //This function runs every x mileseconds, defined on the SetUpVars call.
        private static void Update()
        {
            // CheckStuff();
            UpdateUI();
        }

        private static void BPS()
        {

            t1 = new Thread(new ThreadStart(SeccondsAccumuler));
            t1.Start();

        }

        private static void SeccondsAccumuler()
        {
            while (true)
            {

                Thread.Sleep(1000);
                Breads += BreadsPerSecond;
            }
        }

        //This method runs every x milesconds, inside the Update method,
        private static void UpdateUI()
        {
            Console.Clear();

            for (int i = 0; i < 20; i++)
            {
                if (Data.Upgrades[i].Cost <= Breads)
                {
                    if (Data.Upgrades[i].Bought)
                    {
                        Colors[i] = Green;
                    }
                    else
                    {
                        Colors[i] = White;
                    }
                }
                else
                {
                    if (Data.Upgrades[i].Bought)
                    {
                        Colors[i] = Green;
                    }
                    else
                        Colors[i] = Red;
                }
            }

            for (int i = 0; i < 20; i++)
            {
                if (Data.Upgrades[i].Cost <= Breads)
                {
                    if (!Data.Upgrades[i].Bought)
                        Names[i] = "???";
                    else
                        Names[i] = Data.Upgrades[i].Name;
                }
                else
                {
                    if (!Data.Upgrades[i].Bought)
                        Names[i] = "???";
                    else
                        Names[i] = Data.Upgrades[i].Name;
                }
            }

            Construct();
        }



        static void Construct()
        {

           
            Console.WriteLine("                                                            Patricio's Bakery ");
            Console.WriteLine("     |----Statistics----| ");
            Console.Write("{0,-62}  ___________\n","");
            Console.Write("{0,7} ",Breads,Green); Console.Write("{0,-54} (   )_______)\n","Pães");
            Console.Write("{0,7} ", BreadsPerSecond,Green); Console.Write("{0,-54} |   |Clique!|\n","Pães Por Segundo");
            Console.Write("{0,7} ", BreadsPerClick, Green); Console.Write("{0,-54} |___|_______|\n","Pães Por Click");
            Console.WriteLine("\n\n\n                                                            |----Upgrades----|\n");
            Console.WriteLine("                                                          	                        @*#* ");
            Console.WriteLine("                                                 +                                    _*                             _____ ");
            Console.WriteLine("              (o_                                A_                              _   | |                            /    /|");
            Console.WriteLine("              //\\                               /\\-\\                            | |__| |                           /    //");
            Console.WriteLine("              V_/_                             _||_|_                         __|      |__                        (====|/ \n");
            Console.WriteLine("              (A)                                (S)                               (D)                               (F) \n", Color.FromArgb(81, 255, 178));
            Console.WriteLine("------------AJUDANTES------------|-------------IGREJA--------------|-------------FÁBRICA-------------|------------PESQUISA-------------");

            // Line 1
            Console.Write("{0,-28}{1}", Names[0], Data.Upgrades[0].CostText, Colors[0]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[1], Data.Upgrades[1].CostText, Colors[1]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[2], Data.Upgrades[2].CostText, Colors[2]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[3], Data.Upgrades[3].CostText, Colors[3]); Console.Write("\n", White);

            // Line 2
            Console.Write("{0,-28}{1}", Names[4], Data.Upgrades[4].CostText, Colors[4]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[5], Data.Upgrades[5].CostText, Colors[5]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[6], Data.Upgrades[6].CostText, Colors[6]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[7], Data.Upgrades[7].CostText, Colors[7]); Console.Write("\n", White);

            // Line 3
            Console.Write("{0,-28}{1}", Names[8], Data.Upgrades[8].CostText, Colors[8]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[9], Data.Upgrades[9].CostText, Colors[9]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[10], Data.Upgrades[10].CostText, Colors[10]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[11], Data.Upgrades[11].CostText, Colors[11]); Console.Write("\n", White);
            // Line 4
            Console.Write("{0,-28}{1}", Names[12], Data.Upgrades[12].CostText, Colors[12]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[13], Data.Upgrades[13].CostText, Colors[13]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[14], Data.Upgrades[14].CostText, Colors[14]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[15], Data.Upgrades[15].CostText, Colors[15]); Console.Write("\n", White);
            // Line 5
            Console.Write("{0,-28}{1}", Names[16], Data.Upgrades[16].CostText, Colors[16]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[17], Data.Upgrades[17].CostText, Colors[17]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[18], Data.Upgrades[18].CostText, Colors[18]); Console.Write("|", White);
            Console.Write("{0,-28}{1}", Names[19], Data.Upgrades[19].CostText, Colors[19]); Console.Write("\n", White);

            Console.Write("\n                                                           DINOSSOUR ", Color.FromArgb(81, 255, 178));
            Console.Write("GAMES ", Color.FromArgb(164, 82, 255));
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
            if (state == KeyState.Down) Breads += BreadsPerClick;
        }

        //This method deals with all the keyboard inputs, once the Console.Reads doesn't work anymore. 
        private static void KeyboardHook_OnKeyboardEvent(VirtualKeyCode key, KeyState state)
        {
            if (state == KeyState.Down)
            {
                switch (key)
                {

                    case VirtualKeyCode.Escape:
                        t1.Abort();
                        Exit = true;
                        break;
                    case VirtualKeyCode.A:
                        //Ajudantes Buy
                        if (!Data.Upgrades[0].Bought)
                        {
                            if (Breads > Data.Upgrades[0].Cost)
                            {
                                Comprar(0);
                            }

                        }
                        else
                        {
                            if (!Data.Upgrades[4].Bought)
                            {
                                if (Breads > Data.Upgrades[4].Cost)
                                {
                                    Comprar(4);
                                }
                            }
                            else
                            {
                                if (!Data.Upgrades[8].Bought)
                                {
                                    if (Breads > Data.Upgrades[8].Cost)
                                    {
                                        Comprar(8);
                                    }
                                }
                                else
                                {
                                    if (!Data.Upgrades[12].Bought)
                                    {
                                        if (Breads > Data.Upgrades[12].Cost)
                                        {
                                            Comprar(12);
                                        }
                                    }
                                    else
                                    {
                                        if (!Data.Upgrades[16].Bought)
                                        {
                                            if (Breads > Data.Upgrades[16].Cost)
                                            {
                                                Comprar(16);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    case VirtualKeyCode.S:
                        if (!Data.Upgrades[1].Bought)
                        {
                            if (Breads > Data.Upgrades[1].Cost)
                            {
                                Comprar(1);
                            }

                        }
                        else
                        {
                            if (!Data.Upgrades[5].Bought)
                            {
                                if (Breads > Data.Upgrades[5].Cost)
                                {
                                    Comprar(5);
                                }
                            }
                            else
                            {
                                if (!Data.Upgrades[9].Bought)
                                {
                                    if (Breads > Data.Upgrades[9].Cost)
                                    {
                                        Comprar(9);
                                    }
                                }
                                else
                                {
                                    if (!Data.Upgrades[13].Bought)
                                    {
                                        if (Breads > Data.Upgrades[13].Cost)
                                        {
                                            Comprar(13);
                                        }
                                    }
                                    else
                                    {
                                        if (!Data.Upgrades[17].Bought)
                                        {
                                            if (Breads > Data.Upgrades[17].Cost)
                                            {
                                                Comprar(17);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case VirtualKeyCode.D:
                        //Fábrica
                        if (!Data.Upgrades[2].Bought)
                        {
                            if (Breads > Data.Upgrades[2].Cost)
                            {
                                Comprar(2);
                            }

                        }
                        else
                        {
                            if (!Data.Upgrades[6].Bought)
                            {
                                if (Breads > Data.Upgrades[6].Cost)
                                {
                                    Comprar(6);
                                }
                            }
                            else
                            {
                                if (!Data.Upgrades[10].Bought)
                                {
                                    if (Breads > Data.Upgrades[10].Cost)
                                    {
                                        Comprar(10);
                                    }
                                }
                                else
                                {
                                    if (!Data.Upgrades[14].Bought)
                                    {
                                        if (Breads > Data.Upgrades[14].Cost)
                                        {
                                            Comprar(14);
                                        }
                                    }
                                    else
                                    {
                                        if (!Data.Upgrades[18].Bought)
                                        {
                                            if (Breads > Data.Upgrades[18].Cost)
                                            {
                                                Comprar(18);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case VirtualKeyCode.F:
                        //Pesquisa
                        if (!Data.Upgrades[3].Bought)
                        {
                            if (Breads > Data.Upgrades[3].Cost)
                            {
                                Comprar(3, true);
                            }

                        }
                        else
                        {
                            if (!Data.Upgrades[7].Bought)
                            {
                                if (Breads > Data.Upgrades[7].Cost)
                                {
                                    Comprar(7, true);
                                }
                            }
                            else
                            {
                                if (!Data.Upgrades[11].Bought)
                                {
                                    if (Breads > Data.Upgrades[11].Cost)
                                    {
                                        Comprar(11, true);
                                    }
                                }
                                else
                                {
                                    if (!Data.Upgrades[15].Bought)
                                    {
                                        if (Breads > Data.Upgrades[15].Cost)
                                        {
                                            Comprar(15, true);
                                        }
                                    }
                                    else
                                    {
                                        if (!Data.Upgrades[19].Bought)
                                        {
                                            if (Breads > Data.Upgrades[19].Cost)
                                            {
                                                Comprar(19, true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case VirtualKeyCode.F5:
                        NewGame();
                        break;
                }

            }
        }
        private static void NewGame()
        {
            Data = JsonConvert.DeserializeObject<GameData>(File.ReadAllText("user.dll"));
            SetUpVars();
        }

        static void Comprar(int position, params bool[] last)
        {
            if (last.Length == 0)
            {

                Data.Upgrades[position].Bought = true;
                Breads -= Data.Upgrades[position].Cost;
                BreadsPerSecond += Data.Upgrades[position].PPSUpgrade;
            }
            else
            {
                Data.Upgrades[position].Bought = true;
                Breads -= Data.Upgrades[position].Cost;
                BreadsPerClick += Data.Upgrades[position].PPSUpgrade;
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
