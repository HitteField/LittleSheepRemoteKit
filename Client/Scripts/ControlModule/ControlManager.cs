using System;
using LittleSheep.XamlWindows;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep.ControlModule
{

    class ControlSetter
    {
        #region 单例类
        class danli
        {
            static internal readonly ControlSetter instance = new ControlSetter();
        }
        private ControlSetter() { }
        public static ControlSetter Instance { get { return danli.instance; } }

        #endregion


        #region 定义鼠标键盘事件和键位
        /// <summary>
        /// 鼠标事件
        /// </summary>
        enum MOUSEEVENTF : uint
        {
            // Movement occurred.
            MOUSEEVENTF_MOVE = 0x0001,
            // The left button was pressed.
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            // The left button was released.
            MOUSEEVENTF_LEFTUP = 0x0004,
            //The right button was pressed.
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            //The right button was released.
            MOUSEEVENTF_RIGHTUP = 0x0010,
            //The middle button was pressed.
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            //The middle button was released.
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            //An X button was pressed.
            MOUSEEVENTF_XDOWN = 0x0080,
            //An X button was released.
            MOUSEEVENTF_XUP = 0x0100,
            //The wheel was moved, if the mouse has a wheel. The amount of movement is specified in mouseData.
            MOUSEEVENTF_WHEEL = 0x0800,
            //The wheel was moved horizontally, if the mouse has a wheel. The amount of movement is specified in mouseData.
            MOUSEEVENTF_HWHEEL = 0x1000,
            //	The WM_MOUSEMOVE messages will not be coalesced. The default behavior is to coalesce WM_MOUSEMOVE messages.
            MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000,
            //Maps coordinates to the entire desktop. Must be used with MOUSEEVENTF_ABSOLUTE.
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            /// The dx and dy members contain normalized absolute coordinates. 
            /// If the flag is not set, dxand dy contain relative data (the change in position since the last reported position). 
            /// This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system.
            /// For further information about relative mouse motion, see the following Remarks section.
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }
        enum VirtualKey : short
        {
            ///<summary>
            ///Left mouse button
            ///</summary>
            LBUTTON = 0x01,
            ///<summary>
            ///Right mouse button
            ///</summary>
            RBUTTON = 0x02,
            ///<summary>
            ///Control-break processing
            ///</summary>
            CANCEL = 0x03,
            ///<summary>
            ///Middle mouse button (three-button mouse)
            ///</summary>
            MBUTTON = 0x04,
            ///<summary>
            ///Windows 2000/XP: X1 mouse button
            ///</summary>
            XBUTTON1 = 0x05,
            ///<summary>
            ///Windows 2000/XP: X2 mouse button
            ///</summary>
            XBUTTON2 = 0x06,
            ///<summary>
            ///BACKSPACE key
            ///</summary>
            BACK = 0x08,
            ///<summary>
            ///TAB key
            ///</summary>
            TAB = 0x09,
            ///<summary>
            ///CLEAR key
            ///</summary>
            CLEAR = 0x0C,
            ///<summary>
            ///ENTER key
            ///</summary>
            RETURN = 0x0D,
            ///<summary>
            ///SHIFT key
            ///</summary>
            SHIFT = 0x10,
            ///<summary>
            ///CTRL key
            ///</summary>
            CONTROL = 0x11,
            ///<summary>
            ///ALT key
            ///</summary>
            MENU = 0x12,
            ///<summary>
            ///PAUSE key
            ///</summary>
            PAUSE = 0x13,
            ///<summary>
            ///CAPS LOCK key
            ///</summary>
            CAPITAL = 0x14,
            ///<summary>
            ///Input Method Editor (IME) Kana mode
            ///</summary>
            KANA = 0x15,
            ///<summary>
            ///IME Hangul mode
            ///</summary>
            HANGUL = 0x15,
            ///<summary>
            ///IME Junja mode
            ///</summary>
            JUNJA = 0x17,
            ///<summary>
            ///IME final mode
            ///</summary>
            FINAL = 0x18,
            ///<summary>
            ///IME Hanja mode
            ///</summary>
            HANJA = 0x19,
            ///<summary>
            ///IME Kanji mode
            ///</summary>
            KANJI = 0x19,
            ///<summary>
            ///ESC key
            ///</summary>
            ESCAPE = 0x1B,
            ///<summary>
            ///IME convert
            ///</summary>
            CONVERT = 0x1C,
            ///<summary>
            ///IME nonconvert
            ///</summary>
            NONCONVERT = 0x1D,
            ///<summary>
            ///IME accept
            ///</summary>
            ACCEPT = 0x1E,
            ///<summary>
            ///IME mode change request
            ///</summary>
            MODECHANGE = 0x1F,
            ///<summary>
            ///SPACEBAR
            ///</summary>
            SPACE = 0x20,
            ///<summary>
            ///PAGE UP key
            ///</summary>
            PRIOR = 0x21,
            ///<summary>
            ///PAGE DOWN key
            ///</summary>
            NEXT = 0x22,
            ///<summary>
            ///END key
            ///</summary>
            END = 0x23,
            ///<summary>
            ///HOME key
            ///</summary>
            HOME = 0x24,
            ///<summary>
            ///LEFT ARROW key
            ///</summary>
            LEFT = 0x25,
            ///<summary>
            ///UP ARROW key
            ///</summary>
            UP = 0x26,
            ///<summary>
            ///RIGHT ARROW key
            ///</summary>
            RIGHT = 0x27,
            ///<summary>
            ///DOWN ARROW key
            ///</summary>
            DOWN = 0x28,
            ///<summary>
            ///SELECT key
            ///</summary>
            SELECT = 0x29,
            ///<summary>
            ///PRINT key
            ///</summary>
            PRINT = 0x2A,
            ///<summary>
            ///EXECUTE key
            ///</summary>
            EXECUTE = 0x2B,
            ///<summary>
            ///PRINT SCREEN key
            ///</summary>
            SNAPSHOT = 0x2C,
            ///<summary>
            ///INS key
            ///</summary>
            INSERT = 0x2D,
            ///<summary>
            ///DEL key
            ///</summary>
            DELETE = 0x2E,
            ///<summary>
            ///HELP key
            ///</summary>
            HELP = 0x2F,
            ///<summary>
            ///0 key
            ///</summary>
            KEY_0 = 0x30,
            ///<summary>
            ///1 key
            ///</summary>
            KEY_1 = 0x31,
            ///<summary>
            ///2 key
            ///</summary>
            KEY_2 = 0x32,
            ///<summary>
            ///3 key
            ///</summary>
            KEY_3 = 0x33,
            ///<summary>
            ///4 key
            ///</summary>
            KEY_4 = 0x34,
            ///<summary>
            ///5 key
            ///</summary>
            KEY_5 = 0x35,
            ///<summary>
            ///6 key
            ///</summary>
            KEY_6 = 0x36,
            ///<summary>
            ///7 key
            ///</summary>
            KEY_7 = 0x37,
            ///<summary>
            ///8 key
            ///</summary>
            KEY_8 = 0x38,
            ///<summary>
            ///9 key
            ///</summary>
            KEY_9 = 0x39,
            ///<summary>
            ///A key
            ///</summary>
            KEY_A = 0x41,
            ///<summary>
            ///B key
            ///</summary>
            KEY_B = 0x42,
            ///<summary>
            ///C key
            ///</summary>
            KEY_C = 0x43,
            ///<summary>
            ///D key
            ///</summary>
            KEY_D = 0x44,
            ///<summary>
            ///E key
            ///</summary>
            KEY_E = 0x45,
            ///<summary>
            ///F key
            ///</summary>
            KEY_F = 0x46,
            ///<summary>
            ///G key
            ///</summary>
            KEY_G = 0x47,
            ///<summary>
            ///H key
            ///</summary>
            KEY_H = 0x48,
            ///<summary>
            ///I key
            ///</summary>
            KEY_I = 0x49,
            ///<summary>
            ///J key
            ///</summary>
            KEY_J = 0x4A,
            ///<summary>
            ///K key
            ///</summary>
            KEY_K = 0x4B,
            ///<summary>
            ///L key
            ///</summary>
            KEY_L = 0x4C,
            ///<summary>
            ///M key
            ///</summary>
            KEY_M = 0x4D,
            ///<summary>
            ///N key
            ///</summary>
            KEY_N = 0x4E,
            ///<summary>
            ///O key
            ///</summary>
            KEY_O = 0x4F,
            ///<summary>
            ///P key
            ///</summary>
            KEY_P = 0x50,
            ///<summary>
            ///Q key
            ///</summary>
            KEY_Q = 0x51,
            ///<summary>
            ///R key
            ///</summary>
            KEY_R = 0x52,
            ///<summary>
            ///S key
            ///</summary>
            KEY_S = 0x53,
            ///<summary>
            ///T key
            ///</summary>
            KEY_T = 0x54,
            ///<summary>
            ///U key
            ///</summary>
            KEY_U = 0x55,
            ///<summary>
            ///V key
            ///</summary>
            KEY_V = 0x56,
            ///<summary>
            ///W key
            ///</summary>
            KEY_W = 0x57,
            ///<summary>
            ///X key
            ///</summary>
            KEY_X = 0x58,
            ///<summary>
            ///Y key
            ///</summary>
            KEY_Y = 0x59,
            ///<summary>
            ///Z key
            ///</summary>
            KEY_Z = 0x5A,
            ///<summary>
            ///Left Windows key (Microsoft Natural keyboard)
            ///</summary>
            LWIN = 0x5B,
            ///<summary>
            ///Right Windows key (Natural keyboard)
            ///</summary>
            RWIN = 0x5C,
            ///<summary>
            ///Applications key (Natural keyboard)
            ///</summary>
            APPS = 0x5D,
            ///<summary>
            ///Computer Sleep key
            ///</summary>
            SLEEP = 0x5F,
            ///<summary>
            ///Numeric keypad 0 key
            ///</summary>
            NUMPAD0 = 0x60,
            ///<summary>
            ///Numeric keypad 1 key
            ///</summary>
            NUMPAD1 = 0x61,
            ///<summary>
            ///Numeric keypad 2 key
            ///</summary>
            NUMPAD2 = 0x62,
            ///<summary>
            ///Numeric keypad 3 key
            ///</summary>
            NUMPAD3 = 0x63,
            ///<summary>
            ///Numeric keypad 4 key
            ///</summary>
            NUMPAD4 = 0x64,
            ///<summary>
            ///Numeric keypad 5 key
            ///</summary>
            NUMPAD5 = 0x65,
            ///<summary>
            ///Numeric keypad 6 key
            ///</summary>
            NUMPAD6 = 0x66,
            ///<summary>
            ///Numeric keypad 7 key
            ///</summary>
            NUMPAD7 = 0x67,
            ///<summary>
            ///Numeric keypad 8 key
            ///</summary>
            NUMPAD8 = 0x68,
            ///<summary>
            ///Numeric keypad 9 key
            ///</summary>
            NUMPAD9 = 0x69,
            ///<summary>
            ///Multiply key
            ///</summary>
            MULTIPLY = 0x6A,
            ///<summary>
            ///Add key
            ///</summary>
            ADD = 0x6B,
            ///<summary>
            ///Separator key
            ///</summary>
            SEPARATOR = 0x6C,
            ///<summary>
            ///Subtract key
            ///</summary>
            SUBTRACT = 0x6D,
            ///<summary>
            ///Decimal key
            ///</summary>
            DECIMAL = 0x6E,
            ///<summary>
            ///Divide key
            ///</summary>
            DIVIDE = 0x6F,
            ///<summary>
            ///F1 key
            ///</summary>
            F1 = 0x70,
            ///<summary>
            ///F2 key
            ///</summary>
            F2 = 0x71,
            ///<summary>
            ///F3 key
            ///</summary>
            F3 = 0x72,
            ///<summary>
            ///F4 key
            ///</summary>
            F4 = 0x73,
            ///<summary>
            ///F5 key
            ///</summary>
            F5 = 0x74,
            ///<summary>
            ///F6 key
            ///</summary>
            F6 = 0x75,
            ///<summary>
            ///F7 key
            ///</summary>
            F7 = 0x76,
            ///<summary>
            ///F8 key
            ///</summary>
            F8 = 0x77,
            ///<summary>
            ///F9 key
            ///</summary>
            F9 = 0x78,
            ///<summary>
            ///F10 key
            ///</summary>
            F10 = 0x79,
            ///<summary>
            ///F11 key
            ///</summary>
            F11 = 0x7A,
            ///<summary>
            ///F12 key
            ///</summary>
            F12 = 0x7B,
            ///<summary>
            ///F13 key
            ///</summary>
            F13 = 0x7C,
            ///<summary>
            ///F14 key
            ///</summary>
            F14 = 0x7D,
            ///<summary>
            ///F15 key
            ///</summary>
            F15 = 0x7E,
            ///<summary>
            ///F16 key
            ///</summary>
            F16 = 0x7F,
            ///<summary>
            ///F17 key  
            ///</summary>
            F17 = 0x80,
            ///<summary>
            ///F18 key  
            ///</summary>
            F18 = 0x81,
            ///<summary>
            ///F19 key  
            ///</summary>
            F19 = 0x82,
            ///<summary>
            ///F20 key  
            ///</summary>
            F20 = 0x83,
            ///<summary>
            ///F21 key  
            ///</summary>
            F21 = 0x84,
            ///<summary>
            ///F22 key, (PPC only) Key used to lock device.
            ///</summary>
            F22 = 0x85,
            ///<summary>
            ///F23 key  
            ///</summary>
            F23 = 0x86,
            ///<summary>
            ///F24 key  
            ///</summary>
            F24 = 0x87,
            ///<summary>
            ///NUM LOCK key
            ///</summary>
            NUMLOCK = 0x90,
            ///<summary>
            ///SCROLL LOCK key
            ///</summary>
            SCROLL = 0x91,
            ///<summary>
            ///Left SHIFT key
            ///</summary>
            LSHIFT = 0xA0,
            ///<summary>
            ///Right SHIFT key
            ///</summary>
            RSHIFT = 0xA1,
            ///<summary>
            ///Left CONTROL key
            ///</summary>
            LCONTROL = 0xA2,
            ///<summary>
            ///Right CONTROL key
            ///</summary>
            RCONTROL = 0xA3,
            ///<summary>
            ///Left MENU key
            ///</summary>
            LMENU = 0xA4,
            ///<summary>
            ///Right MENU key
            ///</summary>
            RMENU = 0xA5,
            ///<summary>
            ///Windows 2000/XP: Browser Back key
            ///</summary>
            BROWSER_BACK = 0xA6,
            ///<summary>
            ///Windows 2000/XP: Browser Forward key
            ///</summary>
            BROWSER_FORWARD = 0xA7,
            ///<summary>
            ///Windows 2000/XP: Browser Refresh key
            ///</summary>
            BROWSER_REFRESH = 0xA8,
            ///<summary>
            ///Windows 2000/XP: Browser Stop key
            ///</summary>
            BROWSER_STOP = 0xA9,
            ///<summary>
            ///Windows 2000/XP: Browser Search key
            ///</summary>
            BROWSER_SEARCH = 0xAA,
            ///<summary>
            ///Windows 2000/XP: Browser Favorites key
            ///</summary>
            BROWSER_FAVORITES = 0xAB,
            ///<summary>
            ///Windows 2000/XP: Browser Start and Home key
            ///</summary>
            BROWSER_HOME = 0xAC,
            ///<summary>
            ///Windows 2000/XP: Volume Mute key
            ///</summary>
            VOLUME_MUTE = 0xAD,
            ///<summary>
            ///Windows 2000/XP: Volume Down key
            ///</summary>
            VOLUME_DOWN = 0xAE,
            ///<summary>
            ///Windows 2000/XP: Volume Up key
            ///</summary>
            VOLUME_UP = 0xAF,
            ///<summary>
            ///Windows 2000/XP: Next Track key
            ///</summary>
            MEDIA_NEXT_TRACK = 0xB0,
            ///<summary>
            ///Windows 2000/XP: Previous Track key
            ///</summary>
            MEDIA_PREV_TRACK = 0xB1,
            ///<summary>
            ///Windows 2000/XP: Stop Media key
            ///</summary>
            MEDIA_STOP = 0xB2,
            ///<summary>
            ///Windows 2000/XP: Play/Pause Media key
            ///</summary>
            MEDIA_PLAY_PAUSE = 0xB3,
            ///<summary>
            ///Windows 2000/XP: Start Mail key
            ///</summary>
            LAUNCH_MAIL = 0xB4,
            ///<summary>
            ///Windows 2000/XP: Select Media key
            ///</summary>
            LAUNCH_MEDIA_SELECT = 0xB5,
            ///<summary>
            ///Windows 2000/XP: Start Application 1 key
            ///</summary>
            LAUNCH_APP1 = 0xB6,
            ///<summary>
            ///Windows 2000/XP: Start Application 2 key
            ///</summary>
            LAUNCH_APP2 = 0xB7,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_1 = 0xBA,
            ///<summary>
            ///Windows 2000/XP: For any country/region, the '+' key
            ///</summary>
            OEM_PLUS = 0xBB,
            ///<summary>
            ///Windows 2000/XP: For any country/region, the ',' key
            ///</summary>
            OEM_COMMA = 0xBC,
            ///<summary>
            ///Windows 2000/XP: For any country/region, the '-' key
            ///</summary>
            OEM_MINUS = 0xBD,
            ///<summary>
            ///Windows 2000/XP: For any country/region, the '.' key
            ///</summary>
            OEM_PERIOD = 0xBE,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_2 = 0xBF,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_3 = 0xC0,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_4 = 0xDB,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_5 = 0xDC,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_6 = 0xDD,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_7 = 0xDE,
            ///<summary>
            ///Used for miscellaneous characters; it can vary by keyboard.
            ///</summary>
            OEM_8 = 0xDF,
            ///<summary>
            ///Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
            ///</summary>
            OEM_102 = 0xE2,
            ///<summary>
            ///Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
            ///</summary>
            PROCESSKEY = 0xE5,
            ///<summary>
            ///Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes.
            ///The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information,
            ///see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
            ///</summary>
            PACKET = 0xE7,
            ///<summary>
            ///Attn key
            ///</summary>
            ATTN = 0xF6,
            ///<summary>
            ///CrSel key
            ///</summary>
            CRSEL = 0xF7,
            ///<summary>
            ///ExSel key
            ///</summary>
            EXSEL = 0xF8,
            ///<summary>
            ///Erase EOF key
            ///</summary>
            EREOF = 0xF9,
            ///<summary>
            ///Play key
            ///</summary>
            PLAY = 0xFA,
            ///<summary>
            ///Zoom key
            ///</summary>
            ZOOM = 0xFB,
            ///<summary>
            ///Reserved
            ///</summary>
            NONAME = 0xFC,
            ///<summary>
            ///PA1 key
            ///</summary>
            PA1 = 0xFD,
            ///<summary>
            ///Clear key
            ///</summary>
            OEM_CLEAR = 0xFE
        }
        enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        enum ScanCodeShort : short
        {
            LBUTTON = 0,
            RBUTTON = 0,
            CANCEL = 70,
            MBUTTON = 0,
            XBUTTON1 = 0,
            XBUTTON2 = 0,
            BACK = 14,
            TAB = 15,
            CLEAR = 76,
            RETURN = 28,
            SHIFT = 42,
            CONTROL = 29,
            MENU = 56,
            PAUSE = 0,
            CAPITAL = 58,
            KANA = 0,
            HANGUL = 0,
            JUNJA = 0,
            FINAL = 0,
            HANJA = 0,
            KANJI = 0,
            ESCAPE = 1,
            CONVERT = 0,
            NONCONVERT = 0,
            ACCEPT = 0,
            MODECHANGE = 0,
            SPACE = 57,
            PRIOR = 73,
            NEXT = 81,
            END = 79,
            HOME = 71,
            LEFT = 75,
            UP = 72,
            RIGHT = 77,
            DOWN = 80,
            SELECT = 0,
            PRINT = 0,
            EXECUTE = 0,
            SNAPSHOT = 84,
            INSERT = 82,
            DELETE = 83,
            HELP = 99,
            KEY_0 = 11,
            KEY_1 = 2,
            KEY_2 = 3,
            KEY_3 = 4,
            KEY_4 = 5,
            KEY_5 = 6,
            KEY_6 = 7,
            KEY_7 = 8,
            KEY_8 = 9,
            KEY_9 = 10,
            KEY_A = 30,
            KEY_B = 48,
            KEY_C = 46,
            KEY_D = 32,
            KEY_E = 18,
            KEY_F = 33,
            KEY_G = 34,
            KEY_H = 35,
            KEY_I = 23,
            KEY_J = 36,
            KEY_K = 37,
            KEY_L = 38,
            KEY_M = 50,
            KEY_N = 49,
            KEY_O = 24,
            KEY_P = 25,
            KEY_Q = 16,
            KEY_R = 19,
            KEY_S = 31,
            KEY_T = 20,
            KEY_U = 22,
            KEY_V = 47,
            KEY_W = 17,
            KEY_X = 45,
            KEY_Y = 21,
            KEY_Z = 44,
            LWIN = 91,
            RWIN = 92,
            APPS = 93,
            SLEEP = 95,
            NUMPAD0 = 82,
            NUMPAD1 = 79,
            NUMPAD2 = 80,
            NUMPAD3 = 81,
            NUMPAD4 = 75,
            NUMPAD5 = 76,
            NUMPAD6 = 77,
            NUMPAD7 = 71,
            NUMPAD8 = 72,
            NUMPAD9 = 73,
            MULTIPLY = 55,
            ADD = 78,
            SEPARATOR = 0,
            SUBTRACT = 74,
            DECIMAL = 83,
            DIVIDE = 53,
            F1 = 59,
            F2 = 60,
            F3 = 61,
            F4 = 62,
            F5 = 63,
            F6 = 64,
            F7 = 65,
            F8 = 66,
            F9 = 67,
            F10 = 68,
            F11 = 87,
            F12 = 88,
            F13 = 100,
            F14 = 101,
            F15 = 102,
            F16 = 103,
            F17 = 104,
            F18 = 105,
            F19 = 106,
            F20 = 107,
            F21 = 108,
            F22 = 109,
            F23 = 110,
            F24 = 118,
            NUMLOCK = 69,
            SCROLL = 70,
            LSHIFT = 42,
            RSHIFT = 54,
            LCONTROL = 29,
            RCONTROL = 29,
            LMENU = 56,
            RMENU = 56,
            BROWSER_BACK = 106,
            BROWSER_FORWARD = 105,
            BROWSER_REFRESH = 103,
            BROWSER_STOP = 104,
            BROWSER_SEARCH = 101,
            BROWSER_FAVORITES = 102,
            BROWSER_HOME = 50,
            VOLUME_MUTE = 32,
            VOLUME_DOWN = 46,
            VOLUME_UP = 48,
            MEDIA_NEXT_TRACK = 25,
            MEDIA_PREV_TRACK = 16,
            MEDIA_STOP = 36,
            MEDIA_PLAY_PAUSE = 34,
            LAUNCH_MAIL = 108,
            LAUNCH_MEDIA_SELECT = 109,
            LAUNCH_APP1 = 107,
            LAUNCH_APP2 = 33,
            OEM_1 = 39,
            OEM_PLUS = 13,
            OEM_COMMA = 51,
            OEM_MINUS = 12,
            OEM_PERIOD = 52,
            OEM_2 = 53,
            OEM_3 = 41,
            OEM_4 = 26,
            OEM_5 = 43,
            OEM_6 = 27,
            OEM_7 = 40,
            OEM_8 = 0,
            OEM_102 = 86,
            PROCESSKEY = 0,
            PACKET = 0,
            ATTN = 0,
            CRSEL = 0,
            EXSEL = 0,
            EREOF = 93,
            PLAY = 0,
            ZOOM = 98,
            NONAME = 0,
            PA1 = 0,
            OEM_CLEAR = 0,
        }

        #endregion

        #region 定义结构体和系统调用函数
        [StructLayout(LayoutKind.Sequential)]
        // 定义鼠标事件
        private struct MOUSEINPUT
        {
            /// <summary>
            /// 绝对横坐标
            /// </summary>
            internal int dx;
            /// <summary>
            /// 绝对纵坐标
            /// </summary>
            internal int dy;
            /// <summary>
            /// 如果 dwFlags 包含 MOUSEEVENTF_WHEEL，则 mouseData 指定滚轮移动量。 正值表示滚轮向前旋转，远离用户； 负值表示轮子向后旋转，朝向用户。 一次滚轮点击定义为 WHEEL_DELTA，即 120。Windows Vista：如果 dwFlags 包含 MOUSEEVENTF_HWHEEL，则 dwData 指定滚轮移动量。 正值表示轮子向右旋转； 负值表示轮子向左旋转。 一次滚轮单击定义为 WHEEL_DELTA，即 120。如果 dwFlags 不包含 MOUSEEVENTF_WHEEL、MOUSEEVENTF_XDOWN 或 MOUSEEVENTF_XUP，则 mouseData 应为零。 如果 dwFlags 包含 MOUSEEVENTF_XDOWN 或 MOUSEEVENTF_XUP，则 mouseData 指定按下或释放了哪些 X 按钮。 该值可以是以下标志的任意组合。
            /// </summary>
            internal int Mousedata;
            /// <summary>
            /// MONSEEVENT枚举
            /// </summary>
            internal MOUSEEVENTF dwFlag;
            /// <summary>
            /// 一般是0
            /// </summary>
            public uint time;
            /// <summary>
            /// 
            /// </summary>
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            /// <summary>
            /// KEYBDEVENT
            /// </summary>
            internal VirtualKey wVk;
            internal ScanCodeShort wScan;
            /// <summary>
            /// 0表示按下，2表示松开
            /// </summary>
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            int uMsg;
            short wParamL;
            short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        //type:0:表示鼠标事件，1：表示键盘事件，2：表示硬件事件
        private struct INPUT
        {
            internal uint type;
            internal InputUnion U;
            internal static int Size {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [DllImport("user32.dll", EntryPoint = "SendInput")]
        ///pInputs是的INPUT结构数组类型,nInputs为pInputs数组长度，cbSize为INPUT结构的大小。
        private static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
        #endregion

        #region 对外接口
        public void Set(byte[] controlMSG) {
            INPUT[] inps = new INPUT[1];
            if(controlMSG[0] == 0) {
                //鼠标事件处理
                INPUT inp = MouseMsgPro(BitConverter.ToInt16(controlMSG, 1), BitConverter.ToInt16(controlMSG, 3), BitConverter.ToInt16(controlMSG, 7));
                inps[0] = inp;
            } else if(controlMSG[0] == 1) {
                //键盘事件处理
                INPUT inp = KeybdMsdPro(controlMSG[2], controlMSG[1]);
                inps[0] = inp;
            }

            SendInput(1, inps, INPUT.Size);
        }
        #endregion

        #region 处理不同的信息的处理函数
        private INPUT KeybdMsdPro(byte vmkey, byte state) {
            INPUT inp = new INPUT();
            KEYBDINPUT keyinput = new KEYBDINPUT();
            keyinput.time = 0;
            keyinput.dwExtraInfo = UIntPtr.Zero;
            keyinput.dwFlags = (KEYEVENTF)state;
            keyinput.wVk = (VirtualKey)vmkey;
            keyinput.wScan = ScanCodeShort.ACCEPT;
            inp.U.ki = keyinput;
            inp.type = 1;
            return inp;
        }

        private INPUT MouseMsgPro(short mousekey, int dx, int dy) {
            INPUT inp = new INPUT();
            MOUSEINPUT mouseinput = new MOUSEINPUT();
            mouseinput.time = 0;
            mouseinput.dwExtraInfo = UIntPtr.Zero;
            inp.type = 0;
            mouseinput.dwFlag = (MOUSEEVENTF)mousekey;

            if((MOUSEEVENTF)mousekey == MOUSEEVENTF.MOUSEEVENTF_WHEEL) {
                mouseinput.Mousedata = dx == 1 ? 120 : -120;
            } else {
                mouseinput.Mousedata = 0;
            }
            if((MOUSEEVENTF)mousekey == MOUSEEVENTF.MOUSEEVENTF_MOVE) {
                mouseinput.dx = dx;
                mouseinput.dy = dy;
            } else {
                mouseinput.dx = 0;
                mouseinput.dy = 0;
            }

            inp.U.mi = mouseinput;

            return inp;
        }
        #endregion
    }

    internal class WindowsHook
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果小于零，则挂钩过程必须将消息传递给 CallNextHookEx 函数而不进行进一步处理，
        /// 并且应该返回 CallNextHookEx 返回的值。若为0，则wParam和lParameter包含键盘的信息。</param>
        /// <param name="wParam">键盘的识别信息，包含WM_KEYDOWN,WM_KEYUP,WM_SYSKEYDOWN,WM_SYSKEYUP.</param>
        /// <param name="lParam">指向一个MSLLHOOKSTRUCT结构的指针.</param>
        /// <returns>如果nCode小于零，则挂钩程序必须返回 CallNextHookEx返回的值。
        /// 如果nCode大于等于0，并且hook过程没有处理该消息，强烈建议调用CallNextHookEx并返回它返回的值；
        /// 否则，其他安装了WH_MOUSE_LL挂钩的应用程序将不会收到挂钩通知，因此可能会出现错误行为。
        /// 如果钩子过程处理了消息，它可能会返回一个非零值，以防止系统将消息传递给钩子链的其余部分或目标窗口过程。</returns>
        protected delegate int HookProcHandler(int nCode, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 安装钩子开始监听
        /// </summary>
        /// <param name="idHook">低级键盘钩子14</param>
        /// <param name="lpfn">自己定义的钩子的消息处理方法(对应我们前面定义的委托)指向挂钩过程的指针。 
        /// 如果dwThreadId参数为零或指定由不同进程创建的线程的标识符，则lpfn参数必须指向DLL中的挂钩过程。 
        /// 否则，lpfn可以指向与当前进程关联的代码中的挂钩过程。</param>
        /// <param name="hmod">模块的句柄，在本机代码中，对应dll的句柄(可在dll的入口函数中获取)；而我们是托管代码
        /// 包含lpfn参数指向的钩子函数的DLL句柄。如果dwThreadId参数指定由当前进程创建的线程并且挂钩过程在与当前进程关联的代码内,
        /// 则hMod参数必须设置为 NULL。</param>
        /// <param name="dwThreadId">线程Id，传入0则为全局所有线程，否则传入特定的线程Id
        /// 与挂钩过程关联的线程的标识符。对于桌面应用程序，如果此参数为零，则挂钩过程与在与调用线程相同的桌面中运行的所有现有线程相关联。</param>
        /// <returns>返回hook函数的句柄，否则返回空</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern int SetWindowsHookEx(int idHook, HookProcHandler lpfn, IntPtr hmod, int dwThreadId);


        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="idHook">钩子对应的序号</param>
        /// <returns>返回是否成功销毁</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern bool UnhookWindowsHookEx(int idHook);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHook">忽略</param>
        /// <param name="nCode">传递给当前钩子过程的钩子代码。下一个钩子函数使用此代码来确定如何处理钩子信息。</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern int CallNextHookEx(int idHook, int nCode, UIntPtr wParam, IntPtr lParam);

    }

    internal class KeybdHook : WindowsHook
    {

        #region 单例类
        class danli
        {
            static internal readonly KeybdHook instance = new KeybdHook();
        }
        private KeybdHook() {
        }
        public static KeybdHook Instance { get { return danli.instance; } }
        #endregion


        #region 定义钩子

        //定义钩子句柄
        public static int _hook = 0;
        //定义钩子类型
        private const int WH_KEYBOARD_LL = 13;// 低级键盘钩子


        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            /// <summary>
            /// 键位，KETBDCODE
            /// </summary>
            public int vkCode;
            /// <summary> 硬件扫描码 </summary>
            public int scanCode;
            /// <summary>
            /// https://docs.microsoft.com/zh-cn/windows/win32/api/winuser/ns-winuser-kbdllhookstruct?redirectedfrom=MSDN
            /// </summary>
            public int flags;
            /// <summary>
            /// 事件的时间戳
            /// </summary>
            public int time;
            /// <summary>
            /// 事件的额外信息
            /// </summary>
            public int dwExtraInfo;
        }

        #endregion

        #region 枚举键盘的事件信息
        /// <summary>
        /// 键盘事件，按下或松开
        /// </summary>
        enum KEYBDEVENT
        {
            /// <summary>
            /// 按下键盘键
            /// </summary>
            WM_KEYDOWN = 0x0100,
            /// <summary>
            /// 松开键盘键
            /// </summary>
            WM_KEYUP = 0x0101,
            /// <summary>
            /// Alt之类的，信息在lParam中
            /// </summary>
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105
        }

        /// <summary>
        /// 控制键位
        /// https://docs.microsoft.com/zh-cn/windows/win32/inputdev/virtual-key-codes
        /// </summary>
        enum KEYBDCODE
        {
            //键盘左键,
            VK_LBUTTON = 1,
            //键盘右键,
            VK_RBUTTON = 2,
            // Cancel
            KB_CANCEL = 3,
            //键盘中键
            VK_MBUTTON = 4,
            VK_XBUTTON1 = 5,
            VK_XBUTTON2 = 6,
            //Backspace
            VK_BACK = 8,
            //Tab
            VK_TAB = 9,
            //Clear
            VK_CLEAR = 12,
            //Enter
            VK_RETURN = 13,
            //Shift
            VK_SHIFT = 16,
            //Ctrl
            VK_CONTROL = 17,
            //Alt
            VK_MENU = 18,
            //Pause
            VK_PAUSE = 19,
            //CapsLock
            VK_CAPITAL = 20,
            VK_KANA = 21,
            VK_HANGUL = 21,
            VK_JUNJA = 23,
            VK_FINAL = 24,
            // 大键盘*
            VK_HANJA = 25,
            VK_KANJI = 25,
            //Esc
            VK_ESCAPE = 27,
            VK_CONVERT = 28,
            VK_NONCONVERT = 29,
            VK_ACCEPT = 30,
            VK_MODECHANGE = 31,
            //Space
            VK_SPACE = 32,
            //PageUp
            VK_PRIOR = 33,
            //PageDown
            VK_NEXT = 34,
            //End
            VK_END = 35,
            //Home
            VK_HOME = 36,
            //LeftArrow
            VK_LEFT = 37,
            //UpArrow
            VK_UP = 38,
            //RightArrow
            VK_RIGHT = 39,
            //DownArrow
            VK_DOWN = 40,
            //Select
            VK_SELECT = 41,
            //Print
            VK_PRINT = 42,
            //Execute
            VK_EXECUTE = 43,
            //Snapshot
            VK_SNAPSHOT = 44,
            //Insert
            VK_INSERT = 45,
            //Delete
            VK_DELETE = 46,
            //Help
            VK_HELP = 47,
            ZERO = 48,
            ONE = 49,
            TWO = 50,
            THREE = 51,
            FOUR = 52,
            FIVE = 53,
            SIX = 54,
            SEVEN = 55,
            EIGHT = 56,
            NINE = 57,
            A = 65,
            B = 66,
            C = 67,
            D = 68,
            E = 69,
            F = 70,
            G = 71,
            H = 72,
            I = 73,
            J = 74,
            K = 75,
            L = 76,
            M = 77,
            N = 78,
            O = 79,
            P = 80,
            Q = 81,
            R = 82,
            S = 83,
            T = 84,
            U = 85,
            V = 86,
            W = 87,
            X = 88,
            Y = 89,
            Z = 90,
            VK_LWIN = 91,
            VK_RWIN = 92,
            VK_APPS = 93,
            VK_SLEEP = 95,
            //小键盘0
            VK_NUMPAD0 = 96,
            //小键盘1
            VK_NUMPAD1 = 97,
            //小键盘2
            VK_NUMPAD2 = 98,
            //小键盘3
            VK_NUMPAD3 = 99,
            //小键盘4
            VK_NUMPAD4 = 100,
            //小键盘5
            VK_NUMPAD5 = 101,
            //小键盘6
            VK_NUMPAD6 = 102,
            //小键盘7
            VK_NUMPAD7 = 103,
            //小键盘8
            VK_NUMPAD8 = 104,
            //小键盘9
            VK_NUMPAD9 = 105,
            //小键盘*
            VK_MULTIPLY = 106,
            //小键盘+
            VK_ADD = 107,
            //小键盘Enter
            VK_SEPARATOR = 108,
            //小键盘-
            VK_SUBTRACT = 109,
            //小键盘.
            VK_DECIMAL = 110,
            //小键盘/
            VK_DIVIDE = 111,
            //F1
            VK_F1 = 112,
            //F2
            VK_F2 = 113,
            //F3
            VK_F3 = 114,
            //F4
            VK_F4 = 115,
            //F5
            VK_F5 = 116,
            //F6
            VK_F6 = 117,
            //F7
            VK_F7 = 118,
            //F8
            VK_F8 = 119,
            //F9
            VK_F9 = 120,
            //F10
            VK_F10 = 121,
            //F11
            VK_F11 = 122,
            //F12
            VK_F12 = 123,
            VK_F13 = 124,
            VK_F14 = 125,
            VK_F15 = 126,
            VK_F16 = 127,
            VK_F17 = 128,
            VK_F18 = 129,
            VK_F19 = 130,
            VK_F20 = 131,
            VK_F21 = 132,
            VK_F22 = 133,
            VK_F23 = 134,
            VK_F24 = 135,
            //NumLock
            VK_NUMLOCK = 144,
            //Scroll
            VK_SCROLL = 145,
            VK_LSHIFT = 160,
            VK_RSHIFT = 161,
            VK_LCONTROL = 162,
            VK_RCONTROL = 163,
            VK_LMENU = 164,
            VK_RMENU = 165,
            VK_BROWSER_BACK = 166,
            VK_BROWSER_FORWARD = 167,
            VK_BROWSER_REFRESH = 168,
            VK_BROWSER_STOP = 169,
            VK_BROWSER_SEARCH = 170,
            VK_BROWSER_FAVORITES = 171,
            VK_BROWSER_HOME = 172,
            //VolumeMute
            VK_VOLUME_MUTE = 173,
            //VolumeDown
            VK_VOLUME_DOWN = 174,
            //VolumeUp
            VK_VOLUME_UP = 175,
            VK_MEDIA_NEXT_TRACK = 176,
            VK_MEDIA_PREV_TRACK = 177,
            VK_MEDIA_STOP = 178,
            VK_MEDIA_PLAY_PAUSE = 179,
            VK_LAUNCH_MAIL = 180,
            VK_LAUNCH_MEDIA_SELECT = 181,
            VK_LAUNCH_APP1 = 182,
            VK_LAUNCH_APP2 = 183,
            //大键盘; :
            VK_OEM_1 = 186,
            //大键盘= +
            VK_OEM_PLUS = 187,
            VK_OEM_COMMA = 188,
            ////大键盘- _
            VK_OEM_MINUS = 189,
            VK_OEM_PERIOD = 190,
            //大键盘/ ? 
            VK_OEM_2 = 191,
            //大键盘 ` ~
            VK_OEM_3 = 192,
            //大键盘[ {
            VK_OEM_4 = 219,
            //大键盘 \ |
            VK_OEM_5 = 220,
            // 大键盘] }
            VK_OEM_6 = 221,
            //大键盘' "
            VK_OEM_7 = 222,
            VK_OEM_8 = 223,
            VK_OEM_102 = 226,
            VK_PACKET = 231,
            VK_PROCESSKEY = 229,
            VK_ATTN = 246,
            VK_CRSEL = 247,
            VK_EXSEL = 248,
            VK_EREOF = 249,
            VK_PLAY = 250,
            VK_ZOOM = 251,
            VK_NONAME = 252,
            VK_PA1 = 253,
            VK_OEM_CLEAR = 254
        }

        #endregion

        #region 定义键盘事件处理函数
        /// <summary>
        /// 当产生键盘事件时，钩子的回调函数，绑定到委托MouseHookProcHandler
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果小于零，则挂钩过程必须将消息传递给 CallNextHookEx 函数而不进行进一步处理，
        /// 并且应该返回 CallNextHookEx 返回的值。若为0，则wParam和lParameter包含键盘的信息。</param>
        /// <param name="wParam">键盘的识别信息，包含WM_LBUTTONDOWN, WM_LBUTTONUP, WM_MOUSEMOVE, 
        /// WM_MOUSEWHEEL, WM_MOUSEHWHEEL, WM_RBUTTONDOWN, or WM_RBUTTONUP.</param>
        /// <param name="lParam">指向一个MSLLHOOKSTRUCT结构的指针.</param>
        /// <returns>如果nCode小于零，则挂钩程序必须返回 CallNextHookEx返回的值。
        /// 如果nCode大于等于0，并且hook过程没有处理该消息，强烈建议调用CallNextHookEx并返回它返回的值；
        /// 否则，其他安装了WH_MOUSE_LL挂钩的应用程序将不会收到挂钩通知，因此可能会出现错误行为。
        /// 如果钩子过程处理了消息，它可能会返回一个非零值，以防止系统将消息传递给钩子链的其余部分或目标窗口过程。</returns>
        private int KeybdHookProc(int nCode, UIntPtr wParam, IntPtr lParam) {
            KBDLLHOOKSTRUCT MyKeybdHookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if(nCode < 0) {
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            } else {
                KEYBDEVENT keybdEventIdenti = (KEYBDEVENT)wParam;
                byte kystate = 0;
                switch(keybdEventIdenti) {
                    case KEYBDEVENT.WM_KEYDOWN:
                    // 键盘键按下
                    case KEYBDEVENT.WM_SYSKEYDOWN:
                        // 键盘系统键按下
                        kystate = 0;
                        break;
                    case KEYBDEVENT.WM_KEYUP:
                    // 键盘键松开
                    case KEYBDEVENT.WM_SYSKEYUP:
                        // 键盘系统键松开
                        kystate = 2;
                        break;
                }

                byte[] mode = BitConverter.GetBytes((int)keybdEventIdenti);
                byte[] keymsg = { (byte)1, kystate, (byte)MyKeybdHookStruct.vkCode, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0 };
                KeybdMsgEvent(keymsg);
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            }
        }


        private event KeybdMsgHandler KeybdMsgEvent;
        #endregion

        #region 对外接口

        /// <summary>
        /// 设置钩子开是监听鼠标事件
        /// </summary>
        /// <param name="handler">若订阅的函数为Test()，则传入new KeybdHook.KeybdMsgHandler(test)。</param>
        /// <returns>返回是否设置成功</returns>
        public bool SetHook(KeybdMsgHandler handler) {
            if(_hook == 0) {
                _hook = SetWindowsHookEx(WH_KEYBOARD_LL, new HookProcHandler(this.KeybdHookProc),
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if(_hook != 0) {
                    this.KeybdMsgEvent += handler;
                    return true;
                } else return false;
            } else {
                DestroyHook();
                return SetHook(handler);
            }
        }

        /// <summary>
        /// 判断是否有鼠标钩子在监听
        /// </summary>
        /// <returns>返回是否有鼠标钩子在监听</returns>
        public bool HasHook() {
            if(_hook == 0) return false;
            else return true;
        }

        /// <summary>
        /// 删除钩子，删除前自行判断是否存在钩子，否则报错
        /// </summary>
        public bool DestroyHook() {
            bool suc = UnhookWindowsHookEx(_hook);
            if(suc) {
                _hook = 0;
                KeybdMsgEvent = null;
                return true;
            } else {
                return false;
            }
        }

        public delegate void KeybdMsgHandler(byte[] msg);
        #endregion
    }

    /// <summary>
    /// 用来监听鼠标事件
    /// </summary>
    internal class MouseHook : WindowsHook
    {

        #region 单例类
        class danli
        {
            static internal readonly MouseHook instance = new MouseHook();
        }
        private MouseHook() {
        }
        public static MouseHook Instance { get { return danli.instance; } }
        #endregion

        #region 定义钩子


        private int _hook = 0;

        //定义钩子句柄
        //定义钩子类型
        private const int WH_MOUSE_LL = 14;// 低级鼠标钩子

        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            /// <summary>
            /// 光标的x，y坐标
            /// </summary>
            public POINT pt;
            /// <summary>
            /// 如果消息是WM_MOUSEWHEEL，则高位两字节是滚动变化量。低位两字节无用。
            /// 正值表示滚轮向前旋转，远离用户；负值表示轮子向后旋转，朝向用户。滚轮点击定义为WHEEL_DELTA，即 120。
            /// 如果消息是WM_XBUTTONDOWN、WM_XBUTTONUP、WM_XBUTTONDBLCLK、WM_NCXBUTTONDOWN、WM_NCXBUTTONUP或WM_NCXBUTTONDBLCLK，
            /// 则高位字指定按下或释放哪个 X 按钮，而低位字被保留。该值可以是1：第一个xbuttom，2：第二个xbuttom。否则， 不使用mouseData 。
            /// </summary>
            public int mouseData;
            public int flags;
            /// <summary>
            /// 事件的时间戳
            /// </summary>
            public int time;
            /// <summary>
            /// 事件的额外信息
            /// </summary>
            public int dwExtraInfo;
        }

        #endregion

        enum MOUSEEVENT : ushort
        {
            /// <summary>
            /// 按下鼠标左键
            /// </summary>
            WM_LBUTTONDOWN = 0x0201,
            /// <summary>
            /// 松开鼠标左键
            /// </summary>
            WM_LBUTTONUP = 0x0202,
            /// <summary>
            /// 鼠标移动
            /// </summary>
            WM_MOUSEMOVE = 0x0200,
            /// <summary>
            /// 鼠标滚轮滚动
            /// </summary>
            WM_MOUSEWHEEL = 0x020A,
            /// <summary>
            /// 水平滚轮
            /// </summary>
            WM_MOUSEHWHEEL = 0x020E,
            /// <summary>
            /// 按下鼠标右键
            /// </summary>
            WM_RBUTTONDOWN = 0x0204,
            /// <summary>
            /// 松开鼠标右键
            /// </summary>
            WM_RBUTTONUP = 0x0205
        }

        enum MOUSEEVENTF : ushort
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        private int MouseHookProc(int nCode, UIntPtr wParam, IntPtr lParam) {
            MSLLHOOKSTRUCT MyMouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MOUSEEVENT mouseEventIdenti = (MOUSEEVENT)wParam;
            if(nCode < 0) {
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            } else {


                UInt16 mouseEventf = 0;
                int x = 0;
                int y = 0;
                switch(mouseEventIdenti) {
                    case MOUSEEVENT.WM_LBUTTONDOWN:
                        // 鼠标左键按下
                        mouseEventf = (UInt16)MOUSEEVENTF.LEFTDOWN;
                        break;
                    case MOUSEEVENT.WM_LBUTTONUP:
                        // 鼠标左键松开
                        mouseEventf = (UInt16)MOUSEEVENTF.LEFTUP;
                        break;
                    case MOUSEEVENT.WM_MOUSEWHEEL:
                        // 滚轮移动
                        mouseEventf = (UInt16)MOUSEEVENTF.WHEEL;
                        int wheelData = MyMouseHookStruct.mouseData >> 16;
                        // 滚动数值,正值表示滚轮向前旋转，远离用户；负值表示轮子向后旋转，朝向用户。
                        if(wheelData > 0) {
                            x = 1;
                        } else {
                            x = -1;
                        }
                        break;
                    case MOUSEEVENT.WM_MOUSEHWHEEL:
                        // 滚轮水平移动
                        mouseEventf = (UInt16)MOUSEEVENTF.HWHEEL;
                        break;
                    case MOUSEEVENT.WM_MOUSEMOVE:
                        // 鼠标移动
                        mouseEventf = (UInt16)MOUSEEVENTF.MOVE;
                        x = MyMouseHookStruct.pt.x;
                        y = MyMouseHookStruct.pt.y;
                        
                        break;
                    case MOUSEEVENT.WM_RBUTTONDOWN:
                        // 鼠标右键按下
                        mouseEventf = (UInt16)MOUSEEVENTF.RIGHTDOWN;
                        break;
                    case MOUSEEVENT.WM_RBUTTONUP:
                        //鼠标右键松开
                        mouseEventf = (UInt16)MOUSEEVENTF.RIGHTUP;
                        break;
                }
                byte[] xbytes = BitConverter.GetBytes(x);
                byte[] ybytes = BitConverter.GetBytes(y);
                byte[] mEventfs = BitConverter.GetBytes(mouseEventf);
                byte[] msg = {(byte)0, mEventfs[0], mEventfs[1], xbytes[0], xbytes[1], xbytes[2], xbytes[3],
                                ybytes[0],ybytes[1],ybytes[2],ybytes[3]};
                ConstructMsg(msg);

                return CallNextHookEx(_hook, nCode, wParam, lParam);
            }
        }


        #region 不同鼠标事件处理函数

        private void ConstructMsg(byte[] ConrtrolMSG) {
            MouseMsgEvent(ConrtrolMSG);
        }

        private event MouseMsgHandler MouseMsgEvent;
        #endregion

        #region 定义对外接口

        /// <summary>
        /// 设置钩子开是监听鼠标事件
        /// </summary>
        /// <param name="handler">若订阅的函数为Test()，则传入new MouseHook.MouseMsgHandler(test)。</param>
        /// <returns>返回是否设置成功</returns>
        public bool SetHook(MouseMsgHandler handler) {
            if(_hook == 0) {
                _hook = SetWindowsHookEx(WH_MOUSE_LL, new HookProcHandler(this.MouseHookProc),
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if(_hook != 0) {
                    this.MouseMsgEvent += handler;
                    return true;
                } else return false;
            } else {
                DestroyHook();
                return SetHook(handler);
            }
        }

        /// <summary>
        /// 判断是否有鼠标钩子在监听
        /// </summary>
        /// <returns>返回是否有鼠标钩子在监听</returns>
        public bool HasHook() {
            if(_hook == 0) return false;
            else return true;
        }

        /// <summary>
        /// 删除钩子，删除前自行判断是否存在钩子，否则报错
        /// </summary>
        public bool DestroyHook() {
            bool suc = UnhookWindowsHookEx(_hook);
            if(suc) {
                _hook = 0;
                MouseMsgEvent = null;
                return true;
            } else {
                return false;
            }
        }


        public delegate void MouseMsgHandler(byte[] ControlMSG);

        #endregion


    }

    class ControlGetter
    {
        #region 单例类
        class danli
        {
            static internal readonly ControlGetter instance = new ControlGetter();
        }
        private ControlGetter() {
        }
        public static ControlGetter Instance { get { return danli.instance; } }
        #endregion


        private KeybdHook keybdhook = KeybdHook.Instance;
        private MouseHook mousehook = MouseHook.Instance;

        private void SendMsg(byte[] msg) {
            controlMSGEvent(msg);
        }

        #region 对外接口

        public delegate void controlMSGHandler(byte[] controlMSG);
        /// <summary>
        /// 订阅的函数为Test(byte[] msg)。
        /// </summary>
        public event controlMSGHandler controlMSGEvent;

        /// <summary>
        /// 设置钩子，开始监听
        /// </summary>
        /// <returns>返回是否设置钩子成功</returns>
        public bool Start() {
            if(!mousehook.HasHook()) {
                bool has = mousehook.SetHook(new MouseHook.MouseMsgHandler(SendMsg));
                has = has && keybdhook.SetHook(new KeybdHook.KeybdMsgHandler(SendMsg));
                if(!has) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        /// <summary>
        /// 关闭监听并撤回钩子
        /// </summary>
        /// <returns>返回是否成功撤回钩子</returns>
        public bool Close() {
            if(mousehook.HasHook()) {
                bool suc = mousehook.DestroyHook();
                suc = suc && keybdhook.DestroyHook();
                if(!suc) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }

        /// <summary>
        /// 返回是否正在监听
        /// </summary>
        /// <returns></returns>
        public bool IsWorking() {
            return mousehook.HasHook() && keybdhook.HasHook();
        }
        #endregion
    }
}
