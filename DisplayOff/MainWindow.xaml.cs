using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DisplayOff.Services;


namespace DisplayOff
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("powrprof.dll", SetLastError = true)]
        static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        // Import the LockWorkStation function from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool LockWorkStation();

        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        private const int MonitorPowerOff = 2;
        private SystemTrayManager _systemTrayManager;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus(); // Set focus to the window when it is loaded
        }

        // Turn off the display
        private void TurnOffDisplay_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(GetWindowHandle(), WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MonitorPowerOff);
        }

        // Sleep the computer
        private void Sleep_Click(object sender, RoutedEventArgs e)
        {
            SetSuspendState(false, true, true);
        }

        // Lock the screen
        private void LockScreen_Click(object sender, EventArgs e)
        {
            if (!LockWorkStation())
            {
                // Handle the error if LockWorkStation fails
                MessageBox.Show("Unable to lock the workstation.", "Error");
            }
            else
            {
                LockWorkStation();
            }
        }

        // Toggle Wifi
        private void ToggleWifi_Click(object sender, RoutedEventArgs e)
        {
            ToggleWifi();
        }

        private void ToggleWifi()
        {
            try
            {
                // Run netsh command to toggle Wi-Fi
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface set interface \"Wi-Fi\" admin=ENABLED");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process process = Process.Start(psi);
                if (process != null)
                {
                    process.WaitForExit();
                    if (process.ExitCode == 0)
                    {
                        // Wi-Fi toggled successfully
                        MessageBox.Show("Wi-Fi Enabled", "Wi-Fi", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to toggle Wi-Fi. Please ensure the application has sufficient permissions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to start the process. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error toggling Wi-Fi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Minimize to system tray
        private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }   

        // Get the window handle
        private IntPtr GetWindowHandle()
        {
            var interopHelper = new System.Windows.Interop.WindowInteropHelper(this);
            return interopHelper.Handle;
        }

        // Handle key events
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {    
            switch(e.Key)
            {
                case Key.F1:
                    TurnOffDisplay_Click(sender, e);
                    break;
                case Key.F2:
                    LockScreen_Click(sender, e);
                    break;
                case Key.F3:
                    Sleep_Click(sender, e);
                    break;
                
                case Key.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}