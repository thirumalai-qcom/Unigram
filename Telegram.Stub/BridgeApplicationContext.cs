//
// Copyright Fela Ameghino 2015-2023
//
// Distributed under the GNU General Public License v3.0. (See accompanying
// file LICENSE or copy at https://www.gnu.org/licenses/gpl-3.0.txt)
//
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Telegram.Stub
{
    class BridgeApplicationContext : ApplicationContext
    {
        private AppServiceConnection _connection = null;

        private MenuItem _openMenuItem;
        private MenuItem _exitMenuItem;
        private NotifyIcon _notifyIcon = null;

        private bool _closeRequested = true;
        private int _processId;

        //private InterceptKeys _intercept;

        public BridgeApplicationContext()
        {
            //_intercept = new InterceptKeys();

            SystemEvents.SessionEnded += OnSessionEnded;

            _openMenuItem = new MenuItem("Open Unigram", new EventHandler(OpenApp));
            _exitMenuItem = new MenuItem("Quit Unigram", new EventHandler(Exit));
            _openMenuItem.DefaultItem = true;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Click += OpenApp;
            _notifyIcon.Icon = Properties.Resources.Default;
            _notifyIcon.ContextMenu = new ContextMenu(new MenuItem[] { _openMenuItem, _exitMenuItem });
#if DEBUG
            _notifyIcon.Text = "Telegram";
#else
            _notifyIcon.Text = "Unigram";
#endif

            _notifyIcon.Visible = true;

            try
            {
                var local = ApplicationData.Current.LocalSettings;
                if (local.Values.TryGet("IsLaunchMinimized", out bool minimized) && !minimized)
                {
                    OpenApp(null, null);
                }
                else
                {
                    Connect();
                }

                if (local.Values.ContainsKey("AddLocalhostExemption"))
                {
                    // Already registered
                }
                else
                {
                    AddLocalhostExemption();
                    local.Values.Add("AddLocalhostExemption", true);
                }
            }
            catch
            {
                // Can happen
            }
        }

        private void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            SystemEvents.SessionEnded -= OnSessionEnded;

            if (_connection != null)
            {
                _connection.RequestReceived -= OnRequestReceived;
                _connection.ServiceClosed -= OnServiceClosed;
                _connection.Dispose();
                _connection = null;
            }

            if (_processId != 0)
            {
                try
                {
                    var process = Process.GetProcessById(_processId);
                    process?.Kill();
                }
                catch { }
            }

            _notifyIcon.Dispose();
            Application.Exit();
        }

        private async void OpenApp(object sender, EventArgs e)
        {
            // There's a bug (I guess?) in NotifyIcon that causes Click handler
            // to be fired if user opens the context menu and then dismisses it.
            if (e is MouseEventArgs args && args.Button == MouseButtons.Right)
            {
                return;
            }

            try
            {
                var appListEntries = await Package.Current.GetAppListEntriesAsync();
                await appListEntries.First().LaunchAsync();
            }
            catch { }

            Connect();
        }

        private async void Exit(object sender, EventArgs e)
        {
            if (_connection != null)
            {
                _connection.RequestReceived -= OnRequestReceived;
                _connection.ServiceClosed -= OnServiceClosed;

                try
                {
                    await _connection.SendMessageAsync(new ValueSet { { "Exit", string.Empty } });
                }
                catch
                {

                }
                finally
                {
                    _connection.Dispose();
                    _connection = null;
                }
            }

            _notifyIcon.Dispose();
            Application.Exit();
        }

        private async void Connect()
        {
            if (_connection != null)
            {
                return;
            }

            _connection = new AppServiceConnection
            {
                PackageFamilyName = Package.Current.Id.FamilyName,
                AppServiceName = "org.telegram.bridge"
            };

            _connection.RequestReceived += OnRequestReceived;
            _connection.ServiceClosed += OnServiceClosed;

            await _connection.OpenAsync();
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct FLASHWINFO
        //{
        //    public UInt32 cbSize;
        //    public IntPtr hwnd;
        //    public FlashWindow dwFlags;
        //    public UInt32 uCount;
        //    public UInt32 dwTimeout;
        //}

        //public enum FlashWindow : uint
        //{
        //    /// <summary>
        //    /// Stop flashing. The system restores the window to its original state.
        //    /// </summary>    
        //    FLASHW_STOP = 0,

        //    /// <summary>
        //    /// Flash the window caption
        //    /// </summary>
        //    FLASHW_CAPTION = 1,

        //    /// <summary>
        //    /// Flash the taskbar button.
        //    /// </summary>
        //    FLASHW_TRAY = 2,

        //    /// <summary>
        //    /// Flash both the window caption and taskbar button.
        //    /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        //    /// </summary>
        //    FLASHW_ALL = 3,

        //    /// <summary>
        //    /// Flash continuously, until the FLASHW_STOP flag is set.
        //    /// </summary>
        //    FLASHW_TIMER = 4,

        //    /// <summary>
        //    /// Flash continuously until the window comes to the foreground.
        //    /// </summary>
        //    FLASHW_TIMERNOFG = 12
        //}

        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        //[DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var deferral = args.GetDeferral();
            var response = new ValueSet();

            if (args.Request.Message.TryGet("ProcessId", out int processId))
            {
                _processId = processId;
                response.Add("ProcessId", Process.GetCurrentProcess().Id);
            }

            if (args.Request.Message.TryGet("OpenText", out string openText))
            {
                _openMenuItem.Text = openText;
            }

            if (args.Request.Message.TryGet("ExitText", out string exitText))
            {
                _exitMenuItem.Text = exitText;
            }

            if (args.Request.Message.TryGetValue("FlashWindow", out object flash))
            {
                //#if DEBUG
                //                var handle = FindWindow("ApplicationFrameWindow", "Telegram");
                //#else
                //                var handle = FindWindow("ApplicationFrameWindow", "Unigram");
                //#endif

                //                FLASHWINFO info = new FLASHWINFO();
                //                info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
                //                info.hwnd = handle;
                //                info.dwFlags = FlashWindow.FLASHW_ALL;
                //                info.dwTimeout = 0;
                //                info.uCount = 1;
                //                FlashWindowEx(ref info);
            }

            if (args.Request.Message.TryGet("UnreadCount", out int unreadCount) && args.Request.Message.TryGet("UnreadUnmutedCount", out int unreadUnmutedCount))
            {
                if (unreadCount > 0 || unreadUnmutedCount > 0)
                {
                    _notifyIcon.Icon = unreadUnmutedCount > 0 ? Properties.Resources.Unmuted : Properties.Resources.Muted;
                }
                else
                {
                    _notifyIcon.Icon = Properties.Resources.Default;
                }
            }

            if (args.Request.Message.ContainsKey("LoopbackExempt"))
            {
                AddLocalhostExemption();
            }

            if (args.Request.Message.ContainsKey("CloseRequested"))
            {
                _closeRequested = true;
            }

            if (args.Request.Message.ContainsKey("Exit"))
            {
                _connection.RequestReceived -= OnRequestReceived;
                _connection.ServiceClosed -= OnServiceClosed;
                _connection.Dispose();

                _notifyIcon.Dispose();
                Application.Exit();
            }

            if (args.Request.Message.TryGet("Debug", out string debug))
            {
                MessageBox.Show(debug);
                response.Add("Debug", debug);
            }

            try
            {
                await args.Request.SendResponseAsync(response);
            }
            catch
            {
                // All the remote procedure calls must be wrapped in a try-catch block
            }
            finally
            {
                deferral.Complete();
            }
        }

        private void OnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _connection.RequestReceived -= OnRequestReceived;
            _connection.ServiceClosed -= OnServiceClosed;
            _connection.Dispose();
            _connection = null;

            if (_closeRequested)
            {
                _closeRequested = true;
                Connect();
            }
            else
            {
                _notifyIcon.Dispose();
                Application.Exit();
            }
        }

        private static void AddLocalhostExemption()
        {
            var familyName = Package.Current.Id.FamilyName;
            var info = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "CheckNetIsolation.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "LoopbackExempt -a -n=" + familyName
            };

            try
            {
                Process process = Process.Start(info);
                process.WaitForExit();
                process.Dispose();
            }
            catch { }
        }
    }
}
