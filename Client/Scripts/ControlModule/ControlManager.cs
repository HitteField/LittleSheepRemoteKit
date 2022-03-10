using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep.ControlModule
{
    /// <summary>
    /// 控制键位
    /// https://docs.microsoft.com/zh-cn/windows/win32/inputdev/virtual-key-codes
    /// </summary>
    enum ControlCode
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

    /// <summary>
    /// 控制键位状态
    /// </summary>
    enum ControlStatus
    {
        DOWN = 0,
        UP = 2,
    }

    class ControlMSG
    {
        /// <summary>
        /// 构造控制信息
        /// </summary>
        /// <param name="bVk">枚举类型ControlCode，表示控制的键位</param>
        /// <param name="stutus">枚举类型ControlStatus,表示控制键位的状态</param>
        /// <returns>返回要发送的控制信息字节流,两个字节，第一个字节表示键位，第二字节表示状态</returns>
        public static byte[] GetControlMSG(ControlCode bVk, ControlStatus stutus) {
            byte[] msg = new byte[2];
            msg[0] = (byte)bVk;
            msg[1] = (byte)stutus;
            return msg;
        }
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

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        ///第一个参数(bVk)是键码(InputCode)，第二个和第四个填0即可，第三个代表是按下按键还是松开，0表示按下，2表示松开(InputStatus)。
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /// <summary>
        /// 设置传入的控制信息
        /// </summary>
        /// <param name="controlMSG">两个字节的控制信息</param>
        /// <returns>true：操作成功, false：控制信息有误</returns>
        public static bool ControlInput(byte[] controlMSG) {
            if(controlMSG.Length == 2 && controlMSG[0] >= 1 && controlMSG[0] <= 254 && (controlMSG[1] == 0 || controlMSG[1] == 2)) {
                ControlCode cCode = (ControlCode)controlMSG[0];
                ControlStatus cStatus = (ControlStatus)controlMSG[1];
                keybd_event((byte)cCode, 0, (byte)cStatus, 0);
                return true;
            } else {
                return false;
            }
        }

        public delegate void ControlMsgHandler(byte[] msg);

        public event ControlMsgHandler ControlEvent;

        /// <summary>
        /// 开始监听，订阅事件
        /// </summary>
        public void start() {

        }

    }

    /// <summary>
    /// 用来监听鼠标事件
    /// </summary>
    class MouseHook
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
        public int hHook { get { return _hook; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nCode">挂钩过程用于确定如何处理消息的代码。如果小于零，则挂钩过程必须将消息传递给 CallNextHookEx 函数而不进行进一步处理，
        /// 并且应该返回 CallNextHookEx 返回的值。若为0，则wParam和lParameter包含鼠标的信息。</param>
        /// <param name="wParam">鼠标的识别信息，包含WM_LBUTTONDOWN, WM_LBUTTONUP, WM_MOUSEMOVE, 
        /// WM_MOUSEWHEEL, WM_MOUSEHWHEEL, WM_RBUTTONDOWN, or WM_RBUTTONUP.</param>
        /// <param name="lParam">指向一个MSLLHOOKSTRUCT结构的指针.</param>
        /// <returns>如果nCode小于零，则挂钩程序必须返回 CallNextHookEx返回的值。
        /// 如果nCode大于等于0，并且hook过程没有处理该消息，强烈建议调用CallNextHookEx并返回它返回的值；
        /// 否则，其他安装了WH_MOUSE_LL挂钩的应用程序将不会收到挂钩通知，因此可能会出现错误行为。
        /// 如果钩子过程处理了消息，它可能会返回一个非零值，以防止系统将消息传递给钩子链的其余部分或目标窗口过程。</returns>
        public delegate int LowLevelMouseHookProcHandler(int nCode, UIntPtr wParam, IntPtr lParam);
        //定义钩子句柄
        //定义钩子类型
        private const int WH_MOUSE_LL = 14;// 低级鼠标钩子


        /// <summary>
        /// 安装钩子开始监听
        /// </summary>
        /// <param name="idHook">低级鼠标钩子14</param>
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
        private static extern int SetWindowsHookEx(int idHook, LowLevelMouseHookProcHandler lpfn, IntPtr hmod, int dwThreadId);


        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="idHook">钩子对应的序号</param>
        /// <returns>返回是否成功销毁</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHook">忽略</param>
        /// <param name="nCode">传递给当前钩子过程的钩子代码。下一个钩子函数使用此代码来确定如何处理钩子信息。</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, UIntPtr wParam, IntPtr lParam);


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

        enum MOUSEGETEVENT
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

        private int MouseHookProc(int nCode, UIntPtr wParam, IntPtr lParam) {
            MSLLHOOKSTRUCT MyMouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            MOUSEGETEVENT mouseEventIdenti = (MOUSEGETEVENT)wParam;
            if(nCode < 0) {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            } else {
                switch(mouseEventIdenti) {
                    case MOUSEGETEVENT.WM_LBUTTONDOWN:
                        // 鼠标左键按下
                        MouseLButtonDown();
                        break;
                    case MOUSEGETEVENT.WM_LBUTTONUP:
                        // 鼠标左键松开
                        MouseLButtonUp();
                        break;
                    case MOUSEGETEVENT.WM_MOUSEWHEEL:
                        // 滚轮移动
                        MouseWheel(MyMouseHookStruct.mouseData);
                        break;
                    case MOUSEGETEVENT.WM_MOUSEHWHEEL:
                        // 滚轮水平移动
                        MouseHWheel();
                        break;
                    case MOUSEGETEVENT.WM_MOUSEMOVE:
                        // 鼠标移动
                        MouseMove(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                        break;
                    case MOUSEGETEVENT.WM_RBUTTONDOWN:
                        // 鼠标右键按下
                        MouseRButtonDown();
                        break;
                    case MOUSEGETEVENT.WM_RBUTTONUP:
                        //鼠标右键松开
                        MouseRButtonUp();
                        break;
                }


                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }


        #region 不同鼠标事件处理函数
        private void MouseLButtonDown() {
            ConstructMsg("左键按下\r\n");
        }

        private void MouseLButtonUp() {
            ConstructMsg("左键释放\r\n");
        }

        /// <summary>
        /// 滚轮
        /// </summary>
        /// <param name="wheelData">正值表示滚轮向前旋转，远离用户；负值表示轮子向后旋转，朝向用户。
        /// 滚轮点击定义为WHEEL_DELTA，即 120。</param>
        private void MouseWheel(int wheelData) {
            wheelData = wheelData >> 16;
            // 滚动数值
            ConstructMsg($"滚轮滑动{wheelData}\r\n");
        }

        private void MouseHWheel() {
            // 暂不支持
        }

        /// <summary>
        /// 鼠标在x,y坐标，可能为负值，类似于在屏幕边界处冲出去还没有被拉回来的瞬间
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        private void MouseMove(int x, int y) {
            // 鼠标在x,y坐标，可能为负值，类似于在屏幕边界处冲出去还没有被拉回来的瞬间
            ConstructMsg($"鼠标移动{x},{y}\r\n");
        }

        private void MouseRButtonDown() {
            ConstructMsg("右键按下\r\n");
        }

        private void MouseRButtonUp() {
            ConstructMsg("右键松开\r\n");
        }

        private void ConstructMsg(string str) {
            MouseMsgEvent(str);
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
                _hook = SetWindowsHookEx(WH_MOUSE_LL, new LowLevelMouseHookProcHandler(this.MouseHookProc),
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
                return true;
            } else {
                return false;
            }
        }


        public delegate void MouseMsgHandler(string str);

        #endregion
    }


    class KeybdHook
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
        public delegate int LowLevelKeyboardProcHandler(int nCode, UIntPtr wParam, IntPtr lParam);
        //定义钩子句柄
        public static int _hook = 0;
        //定义钩子类型
        private const int WH_KEYBOARD_LL = 13;// 低级键盘钩子


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
        public static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcHandler lpfn, IntPtr hmod, int dwThreadId);


        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="idHook">钩子对应的序号</param>
        /// <returns>返回是否成功销毁</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHook">忽略</param>
        /// <param name="nCode">传递给当前钩子过程的钩子代码。下一个钩子函数使用此代码来确定如何处理钩子信息。</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, UIntPtr wParam, IntPtr lParam);


        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }


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
        public int KaybdHookProc(int nCode, UIntPtr wParam, IntPtr lParam) {
            KBDLLHOOKSTRUCT MyKeybdHookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
            if(nCode < 0) {
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            } else {
                KEYBDEVENT keybdEventIdenti = (KEYBDEVENT)wParam;
                switch(keybdEventIdenti) {
                    case KEYBDEVENT.WM_KEYDOWN:
                        // 键盘键按下
                        KeyndMsgEvent($"{MyKeybdHookStruct.vkCode}被按下");
                        break;
                    case KEYBDEVENT.WM_SYSKEYDOWN:
                        // 键盘系统键按下
                        KeyndMsgEvent($"{MyKeybdHookStruct.vkCode}被按下");
                        break;
                    case KEYBDEVENT.WM_KEYUP:
                        // 键盘键松开
                        KeyndMsgEvent($"{MyKeybdHookStruct.vkCode}被松开");
                        break;
                    case KEYBDEVENT.WM_SYSKEYUP:
                        // 键盘系统键松开
                        KeyndMsgEvent($"{MyKeybdHookStruct.vkCode}被松开");
                        break;
                }

                return CallNextHookEx(_hook, nCode, wParam, lParam);
            }
        }
        #endregion

        #region 对外接口

        /// <summary>
        /// 设置钩子开是监听鼠标事件
        /// </summary>
        /// <param name="handler">若订阅的函数为Test()，则传入new MouseHook.MouseMsgHandler(test)。</param>
        /// <returns>返回是否设置成功</returns>
        public bool SetHook(KeybdMsgHandler handler) {
            if(_hook == 0) {
                _hook = SetWindowsHookEx(WH_KEYBOARD_LL, new LowLevelKeyboardProcHandler(this.KaybdHookProc),
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if(_hook != 0) {
                    this.KeyndMsgEvent += handler;
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
                return true;
            } else {
                return false;
            }
        }

        public delegate void KeybdMsgHandler(string msg);
        public event KeybdMsgHandler KeyndMsgEvent;
        #endregion
    }

}
