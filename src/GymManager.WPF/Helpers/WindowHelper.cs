using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace GymManager.WPF.Helpers;

/// <summary>
/// Helper para aplicar efectos de ventana modernos (Mica, Acrylic)
/// Compatible con Windows 10 y Windows 11
/// </summary>
public static class WindowHelper
{
    #region DWM API

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    [DllImport("dwmapi.dll")]
    private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    [StructLayout(LayoutKind.Sequential)]
    private struct MARGINS
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    // Atributos DWM
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_MICA_EFFECT = 1029;
    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

    // Tipos de backdrop (Windows 11 22H2+)
    private const int DWMSBT_AUTO = 0;
    private const int DWMSBT_DISABLE = 1;
    private const int DWMSBT_MAINWINDOW = 2;      // Mica
    private const int DWMSBT_TRANSIENTWINDOW = 3; // Acrylic
    private const int DWMSBT_TABBEDWINDOW = 4;    // Mica Alt

    #endregion

    /// <summary>
    /// Aplica el efecto Mica a una ventana (Windows 11)
    /// </summary>
    public static void ApplyMica(Window window, bool useDarkMode = true)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();

        // Habilitar modo oscuro
        if (useDarkMode)
        {
            int darkMode = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
        }

        // Intentar Mica primero (Windows 11 22H2+)
        int micaValue = DWMSBT_MAINWINDOW;
        int result = DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref micaValue, sizeof(int));

        // Si falla, intentar método antiguo (Windows 11 21H2)
        if (result != 0)
        {
            int enableMica = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_MICA_EFFECT, ref enableMica, sizeof(int));
        }

        // Extender el marco
        var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }

    /// <summary>
    /// Aplica el efecto Acrylic a una ventana
    /// </summary>
    public static void ApplyAcrylic(Window window, bool useDarkMode = true)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();

        if (useDarkMode)
        {
            int darkMode = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
        }

        int acrylicValue = DWMSBT_TRANSIENTWINDOW;
        DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref acrylicValue, sizeof(int));

        var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }

    /// <summary>
    /// Aplica el efecto Mica Alt (mas opaco) a una ventana
    /// </summary>
    public static void ApplyMicaAlt(Window window, bool useDarkMode = true)
    {
        var hwnd = new WindowInteropHelper(window).EnsureHandle();

        if (useDarkMode)
        {
            int darkMode = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
        }

        int micaAltValue = DWMSBT_TABBEDWINDOW;
        DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref micaAltValue, sizeof(int));

        var margins = new MARGINS { Left = -1, Right = -1, Top = -1, Bottom = -1 };
        DwmExtendFrameIntoClientArea(hwnd, ref margins);
    }
}