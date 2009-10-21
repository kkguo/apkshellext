/*
  This file contains interfaces needs to be implement for 
  Shell extension
  
  Copyright(c) Guo Jingang KK,guo.jingang@gmail.com
*/
using System;
using System.Runtime.InteropServices;

namespace KKHomeProj.ShellExtInts
{
    
    #region Enum
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

    #endregion
        
    #region IPersistFile

    [ComImport(), ComVisible(true),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    GuidAttribute("0000010b-0000-0000-C000-000000000046")]
    public interface IPersistFile
    {
        [PreserveSig]
        uint GetClassID(out Guid pClassID);

        [PreserveSig]
        uint IsDirty();

        [PreserveSig]
        uint Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In] uint dwMode);

        [PreserveSig]
        uint Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In] bool fRemember);

        [PreserveSig]
        uint SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig]
        uint GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
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


}
