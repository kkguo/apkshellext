/*
  This file contains interfaces needs to be implement for 
  Shell extension
  
  Copyright(c) Guo Jingang KK,guo.jingang@gmail.com
*/
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace KKHomeProj.ShellExtInts
{
    
    #region Enum & Struct
    [Flags]
    public enum ExtractIconOptions
    {
        OpenIcon = 0x0001,
        ForShell = 0x0002,
        Async = 0x0020,
        DefaultIcon = 0x0040,
        ForShortcut = 0x0080
    }

    [Flags]
    public enum ExtractIconFlags
    {
        None = 0x0000,
        SimulateDoc = 0x0001,	// simulate this document icon for this	}
        PerInstance = 0x0002,	// icons from this class are per instance (each file has its own)}
        PerClass = 0x0004,	// icons from this class per class (shared for all files of this type)
        NotFilename = 0x0008,	// location is not a filename, must call ::ExtractIcon
        DontCache = 0x0010	// this icon should not be cached
    }

    internal enum CLIPFORMAT : uint
    {
        CF_TEXT = 1,
        CF_BITMAP = 2,
        CF_METAFILEPICT = 3,
        CF_SYLK = 4,
        CF_DIF = 5,
        CF_TIFF = 6,
        CF_OEMTEXT = 7,
        CF_DIB = 8,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_RIFF = 11,
        CF_WAVE = 12,
        CF_UNICODETEXT = 13,
        CF_ENHMETAFILE = 14,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CF_MAX = 17,

        CF_OWNERDISPLAY = 0x0080,
        CF_DSPTEXT = 0x0081,
        CF_DSPBITMAP = 0x0082,
        CF_DSPMETAFILEPICT = 0x0083,
        CF_DSPENHMETAFILE = 0x008E,

        CF_PRIVATEFIRST = 0x0200,
        CF_PRIVATELAST = 0x02FF,

        CF_GDIOBJFIRST = 0x0300,
        CF_GDIOBJLAST = 0x03FF
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MENUITEMINFO
    {
        public uint cbSize;
        public MIIM fMask;
        public MFT fType;
        public MFS fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public UIntPtr dwItemData;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;
    }

    [Flags]
    internal enum MIIM : uint
    {
        MIIM_STATE = 0x00000001,
        MIIM_ID = 0x00000002,
        MIIM_SUBMENU = 0x00000004,
        MIIM_CHECKMARKS = 0x00000008,
        MIIM_TYPE = 0x00000010,
        MIIM_DATA = 0x00000020,
        MIIM_STRING = 0x00000040,
        MIIM_BITMAP = 0x00000080,
        MIIM_FTYPE = 0x00000100
    }

    internal enum MFT : uint
    {
        MFT_STRING = 0x00000000,
        MFT_BITMAP = 0x00000004,
        MFT_MENUBARBREAK = 0x00000020,
        MFT_MENUBREAK = 0x00000040,
        MFT_OWNERDRAW = 0x00000100,
        MFT_RADIOCHECK = 0x00000200,
        MFT_SEPARATOR = 0x00000800,
        MFT_RIGHTORDER = 0x00002000,
        MFT_RIGHTJUSTIFY = 0x00004000
    }

    internal enum MFS : uint
    {
        MFS_ENABLED = 0x00000000,
        MFS_UNCHECKED = 0x00000000,
        MFS_UNHILITE = 0x00000000,
        MFS_GRAYED = 0x00000003,
        MFS_DISABLED = 0x00000003,
        MFS_CHECKED = 0x00000008,
        MFS_HILITE = 0x00000080,
        MFS_DEFAULT = 0x00001000
    }

    [Flags]
    internal enum CMF : uint
    {
        CMF_NORMAL = 0x00000000,
        CMF_DEFAULTONLY = 0x00000001,
        CMF_VERBSONLY = 0x00000002,
        CMF_EXPLORE = 0x00000004,
        CMF_NOVERBS = 0x00000008,
        CMF_CANRENAME = 0x00000010,
        CMF_NODEFAULT = 0x00000020,
        CMF_INCLUDESTATIC = 0x00000040,
        CMF_ITEMMENU = 0x00000080,
        CMF_EXTENDEDVERBS = 0x00000100,
        CMF_DISABLEDVERBS = 0x00000200,
        CMF_ASYNCVERBSTATE = 0x00000400,
        CMF_OPTIMIZEFORINVOKE = 0x00000800,
        CMF_SYNCCASCADEMENU = 0x00001000,
        CMF_DONOTPICKDEFAULT = 0x00002000,
        CMF_RESERVED = 0xFFFF0000
    }

    [Flags]
    public enum MFMENU : uint
    {
        MF_UNCHECKED = 0,
        MF_STRING = 0,
        MF_ENABLED = 0,
        MF_BYCOMMAND = 0,
        MF_GRAYED = 1,
        MF_DISABLED = 0x00000002,
        MF_CHECKED = 0x00000008,
        MF_POPUP = 0x00000010,
        MF_HILITE = 0x00000080,
        MF_BYPOSITION = 0x00000400,
        MF_SEPARATOR = 0x00000800,
    }

    internal enum GCS : uint
    {
        GCS_VERBA = 0x00000000,
        GCS_HELPTEXTA = 0x00000001,
        GCS_VALIDATEA = 0x00000002,
        GCS_VERBW = 0x00000004,
        GCS_HELPTEXTW = 0x00000005,
        GCS_VALIDATEW = 0x00000006,
        GCS_VERBICONW = 0x00000014,
        GCS_UNICODE = 0x00000004
    }

    [Flags]
    internal enum CMIC : uint
    {
        CMIC_MASK_ICON = 0x00000010,
        CMIC_MASK_HOTKEY = 0x00000020,
        CMIC_MASK_NOASYNC = 0x00000100,
        CMIC_MASK_FLAG_NO_UI = 0x00000400,
        CMIC_MASK_UNICODE = 0x00004000,
        CMIC_MASK_NO_CONSOLE = 0x00008000,
        CMIC_MASK_ASYNCOK = 0x00100000,
        CMIC_MASK_NOZONECHECKS = 0x00800000,
        CMIC_MASK_FLAG_LOG_USAGE = 0x04000000,
        CMIC_MASK_SHIFT_DOWN = 0x10000000,
        CMIC_MASK_PTINVOKE = 0x20000000,
        CMIC_MASK_CONTROL_DOWN = 0x40000000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CMINVOKECOMMANDINFO
    {
        public uint cbSize;
        public CMIC fMask;
        public IntPtr hwnd;
        public IntPtr verb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string parameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string directory;
        public int nShow;
        public uint dwHotKey;
        public IntPtr hIcon;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CMINVOKECOMMANDINFOEX
    {
        public uint cbSize;
        public CMIC fMask;
        public IntPtr hwnd;
        public IntPtr verb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string parameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string directory;
        public int nShow;
        public uint dwHotKey;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string title;
        public IntPtr verbW;
        public string parametersW;
        public string directoryW;
        public string titleW;
        POINT ptInvoke;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    public struct HMenu
    {
        public HMenu(IntPtr x)
        {
            handle = x;
        }
        public IntPtr handle;
    }
    #endregion

    #region IExtractIcon

    [ComVisible(true), ComImport,
    Guid("000214eb-0000-0000-c000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IExtractIcon
    {
        [PreserveSig]
        uint GetIconLocation([In] ExtractIconOptions uFlags,
            [In] IntPtr szIconFile,
            [In] uint cchMax,
            [Out] out int piIndex,
            [Out] out ExtractIconFlags pwFlags);

        [PreserveSig]
        uint Extract([In, MarshalAs(UnmanagedType.LPWStr)] string pszFile,
            uint nIconIndex,
            [Out] out IntPtr phiconLarge,
            [Out] out IntPtr phiconSmall,
            [In] uint nIconSize);
    }

    #endregion

    #region IShellExtInit
    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    [Guid("000214e8-0000-0000-c000-000000000046")]
    public interface IShellExtInit
    {
        void Initialize(
            IntPtr /*LPCITEMIDLIST*/ pidlFolder,
            IntPtr /*LPDATAOBJECT*/ pDataObj,
            IntPtr /*HKEY*/ hKeyProgID);
    }

    #endregion

    #region IConteextMenu
    [ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    [Guid("000214e4-0000-0000-c000-000000000046")]
    public interface IContextMenu
    {
        [PreserveSig]
        int QueryContextMenu(
            IntPtr /*HMENU*/ hMenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            uint uFlags);

        void InvokeCommand(IntPtr pici);

        void GetCommandString(
            UIntPtr idCmd,
            uint uFlags,
            IntPtr pReserved,
            StringBuilder pszName,
            uint cchMax);
    }
    #endregion

    #region IQueryInfo

    [ComImport(), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), GuidAttribute("00021500-0000-0000-c000-000000000046")]
    public interface IQueryInfo
    {
        [PreserveSig]
        uint GetInfoTip(uint dwFlags, out IntPtr pszInfoTip);

        [PreserveSig]
        uint GetInfoFlags(out uint dwFlags);
    }

    #endregion

    #region NativeMethod
    internal class NativeMethods
    {
        /// <summary>
        /// Retrieve the names of dropped files that result from a successful drag-
        /// and-drop operation.
        /// </summary>
        /// <param name="hDrop">
        /// Identifier of the structure that contains the file names of the dropped 
        /// files.
        /// </param>
        /// <param name="iFile">
        /// Index of the file to query. If the value of this parameter is 0xFFFFFFFF, 
        /// DragQueryFile returns a count of the files dropped. 
        /// </param>
        /// <param name="pszFile">
        /// The address of a buffer that receives the file name of a dropped file 
        /// when the function returns.
        /// </param>
        /// <param name="cch">
        /// The size, in characters, of the pszFile buffer.
        /// </param>
        /// <returns>A non-zero value indicates a successful call.</returns>
        [DllImport("shell32", CharSet = CharSet.Unicode)]
        public static extern uint DragQueryFile(
            IntPtr hDrop,
            uint iFile,
            StringBuilder pszFile,
            int cch);
        /// <summary>
        /// Free the specified storage medium.
        /// </summary>
        /// <param name="pmedium">
        /// Reference of the storage medium that is to be freed.
        /// </param>
        [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
        public static extern void ReleaseStgMedium(ref STGMEDIUM pmedium);

        /// <summary>
        /// Insert a new menu item at the specified position in a menu.
        /// </summary>
        /// <param name="hMenu">
        /// A handle to the menu in which the new menu item is inserted. 
        /// </param>
        /// <param name="uItem">
        /// The identifier or position of the menu item before which to insert the 
        /// new item. The meaning of this parameter depends on the value of 
        /// fByPosition.
        /// </param>
        /// <param name="fByPosition">
        /// Controls the meaning of uItem. If this parameter is false, uItem is a 
        /// menu item identifier. Otherwise, it is a menu item position. 
        /// </param>
        /// <param name="mii">
        /// A reference of a MENUITEMINFO structure that contains information about 
        /// the new menu item.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is true.
        /// </returns>
        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InsertMenuItem(
            IntPtr hMenu,
            uint uItem,
            [MarshalAs(UnmanagedType.Bool)]bool fByPosition,
            ref MENUITEMINFO mii);

        /// <summary>
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, 
        /// region, or palette, freeing all system resources associated with the 
        /// object. After the object is deleted, the specified handle is no longer 
        /// valid.
        /// </summary>
        /// <param name="hObject">
        /// A handle to a logical pen, brush, font, bitmap, region, or palette.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is true.
        /// </returns>
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32")]
        public static extern bool AppendMenu(HMenu hmenu, MFMENU uflags, IntPtr uIDNewItemOrSubmenu, string text);

        [DllImport("user32")]
        public static extern bool InsertMenu(HMenu hmenu, int position, MFMENU uflags, IntPtr uIDNewItemOrSubmenu, string text);

        [DllImport("user32")]
        public static extern HMenu CreatePopupMenu();

        [DllImport("user32")]
        public static extern int SetMenuItemBitmaps(HMenu hmenu, int nPosition, MFMENU uflags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);

        public static int HighWord(int number)
        {
            return ((number & 0x80000000) == 0x80000000) ?
                (number >> 16) : ((number >> 16) & 0xffff);
        }

        public static int LowWord(int number)
        {
            return number & 0xffff;
        }
    }

    internal static class WinError
    {
        public const int S_OK = 0x0000;
        public const int S_FALSE = 0x0001;
        public const int E_FAIL = -2147467259;
        public const int E_INVALIDARG = -2147024809;
        public const int E_OUTOFMEMORY = -2147024882;
        public const int STRSAFE_E_INSUFFICIENT_BUFFER = -2147024774;

        public const uint SEVERITY_SUCCESS = 0;
        public const uint SEVERITY_ERROR = 1;

        public const uint E_PENDING = 0x8000000A;
        public const uint E_NOTIMPL = 0x80004001;

        /// <summary>
        /// Create an HRESULT value from component pieces.
        /// </summary>
        /// <param name="sev">The severity to be used</param>
        /// <param name="fac">The facility to be used</param>
        /// <param name="code">The error number</param>
        /// <returns>A HRESULT constructed from the above 3 values</returns>
        public static int MAKE_HRESULT(uint sev, uint fac, uint code)
        {
            return (int)((sev << 31) | (fac << 16) | code);
        }
    }

    #endregion
}
