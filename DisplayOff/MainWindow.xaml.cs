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

namespace DisplayOff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        private const int MonitorPowerOff = 2;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus(); // Set focus to the window when it is loaded
        }

        // Turn off the lcd
        private void TurnOffLCD_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(GetWindowHandle(), WM_SYSCOMMAND, (IntPtr)SC_MONITORPOWER, (IntPtr)MonitorPowerOff);
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
            if (e.Key == Key.F1)
            {
                TurnOffLCD_Click(sender, e);
            }
            else
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}