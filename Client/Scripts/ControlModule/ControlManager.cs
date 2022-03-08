using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    enum InputEnum
    {
        //鼠标左键,
        VK_LBUTTON = 1,
        //鼠标右键,
        VK_RBUTTON = 2,
        // Cancel
        KB_CANCEL = 3,
        //鼠标中键
        VK_MBUTTON = 4,
        VK_XBUTTON1 = 5,
        VK_XBUTTON2 = 6,
        VK_BACK = 8, //Backspace
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
    class ControlManager
    {
        #region 单例类构造函数
        class Nested
        {
            internal static readonly ControlManager instance = new ControlManager();
        }
        private ControlManager() { }
        public static ControlManager Instance { get { return Nested.instance; } }
        #endregion
    }
}
