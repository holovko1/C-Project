using Domain.Entities;
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
using WpfAppThread.Data;
using Faker;

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
            using (DatabaseContext context = new DatabaseContext())
            {
                UserEntity user = new UserEntity();
                user.FirstName = Faker.Name.First();
                user.LastName = Faker.Name.Last();
                user.Email = Faker.Internet.Email();
                user.Phone = Faker.Phone.Number().ToString();
                //context.Add(user);
                //context.SaveChanges();
            }
        }

        private void InsertItems(double count)
        {
            Dispatcher.Invoke(() => { pbStatus.Maximum = count; });
            for (int i = 0; i < count; i++)
            {
                Dispatcher.Invoke(() => { pbStatus.Value = i + 1; });
                while (isPaused)
                {
                    Thread.Sleep(100);
                }
                if (isClosing)
                {
                    Dispatcher.Invoke(() => { pbStatus.Value = 0; });
                    MessageBox.Show("Скасовано");
                    break;
                }    
                Thread.Sleep(100);
            }
            Dispatcher.Invoke(() => { btnRun.IsEnabled = true; });
            if (!isClosing)
                    MessageBox.Show("Виконано");
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