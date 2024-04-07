using Infrastraction.Services;
using System.Windows;

namespace WpfAppThread
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Thread thread;
        private bool isPaused = false;
        private volatile bool isClosing = false;

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            isClosing = false;
            btnRun.IsEnabled = false;
            double count = double.Parse(txtCount.Text);
            thread = new Thread(() => 
                InsertItems(count));
            thread.Start();
        }

        private void InsertItems(double count)
        {
            UserService userService = new UserService();
            userService.InsertUserEvent += UserService_InsertUserEvent;
            userService.InsertRandomUser((int)count);
            Dispatcher.Invoke(() => { btnRun.IsEnabled = true; });
        }

        private void UserService_InsertUserEvent(int count)
        {
            Dispatcher.Invoke(() => { pbStatus.Value = count; });
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            btnPause.Content = isPaused ? "Продовжити" : "Пауза";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            isClosing = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isClosing = true;
            if (thread.IsAlive)
            {
                thread.Join();
            }
        }
    }
}