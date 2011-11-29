namespace WindowMonitor
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Utility class to monitor the currently active window.
    /// </summary>
    public class WindowMonitor
    {
        /// <summary>
        /// Gets a pointer to the currently active window.
        /// </summary>
        /// <returns>
        /// A pointer to the active window.
        /// </returns>
        public IntPtr GetActiveWindowHandle()
        {
            return GetForegroundWindow();
        }

        /// <summary>
        /// Gets the title of the window with the given handle.
        /// </summary>
        /// <param name="handle">
        /// The handle of the window to look up.
        /// </param>
        /// <returns>
        /// The textual title of the given window.
        /// </returns>
        public string GetWindowTitle(IntPtr handle)
        {
            var length = GetWindowTextLength(handle);
            var buffer = new StringBuilder(length + 1);
            GetWindowText(handle, buffer, length + 1);
            return buffer.ToString();
        }

        /// <summary>
        /// Gets the full command line of the process that owns the window
        /// with the given handle.
        /// </summary>
        /// <param name="handle">
        /// The handle of the window to look up.
        /// </param>
        /// <returns>
        /// The command line of the owning process, or <c>string.Empty</c> if
        /// it couldn't be found.
        /// </returns>
        public string GetWindowFileName(IntPtr handle)
        {
            int processId;
            GetWindowThreadProcessId(handle, out processId);

            var wmiQuery = string.Format("select CommandLine from Win32_Process where ProcessId={0}", processId);
            var searcher = new ManagementObjectSearcher(wmiQuery);
            var resultEnumerator = searcher.Get().GetEnumerator();

            if (resultEnumerator.MoveNext())
            {
                return resultEnumerator.Current["CommandLine"].ToString();
            }

            return string.Empty;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr handle, StringBuilder text, int count);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
    }
}
