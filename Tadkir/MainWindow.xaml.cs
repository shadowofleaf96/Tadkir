using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ContextMenuStrip = System.Windows.Forms.ContextMenuStrip;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Tadkir
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        readonly Image _quranImages = new Image();
        DispatcherTimer _dispatcherTimer;
        
        public MainWindow ()
        {
            SizeChanged += (_, _) =>
            {
                var r = SystemParameters.WorkArea;
                Left = r.Right - ActualWidth;
                Top = r.Bottom - ActualHeight;
            };
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InstalledUICulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InstalledUICulture;
            InitializeComponent();
            NotifyIcon notify = new NotifyIcon();
            notify.Icon = System.Drawing.Icon.ExtractAssociatedIcon(
                Assembly.GetEntryAssembly()?.ManifestModule.Name!);
            notify.ContextMenuStrip = new ContextMenuStrip();
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("Tadkir.Resources.Resources", Assembly.GetExecutingAssembly());
            notify.ContextMenuStrip.Items.Add(rm.GetString("Hide"), null, Hide_Click);
            notify.ContextMenuStrip.Items.Add(rm.GetString("Exit"), null,Exit_Click);
            notify.Visible = true;
            notify.DoubleClick += 
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                };
            // Create Image and set its width and height  
            // Create a BitmapSource  
            BitmapImage bitmap = new BitmapImage();  
            bitmap.BeginInit();
            var rand = new Random();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+ @"\Resources\images", "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            var enumerable = files as string[] ?? files.ToArray();
            var images = enumerable[rand.Next(enumerable.Length)];
            var imagesUri = new Uri(images, UriKind.Absolute);
            bitmap.UriSource = imagesUri;  
            bitmap.EndInit();

            // Set Image.Source  
            _quranImages.Source = bitmap;
            Layout.Children.Add(_quranImages);
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += checkForTime_Elapsed;
            _dispatcherTimer.Interval = new TimeSpan(2, 40, 0);
            _dispatcherTimer.Start();
            EndApp();
        }
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();

            base.OnStateChanged(e);
        }
        void Hide_Click(object? sender, EventArgs e)
        {
            Hide();
        }
        void Exit_Click(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
        private void EndApp()
        {
            _dispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(300)};
            _dispatcherTimer.Tick += delegate
            {
                _dispatcherTimer.Stop();
                Hide();
                _dispatcherTimer.Start();
            };
            _dispatcherTimer.Start();
            InputManager.Current.PostProcessInput += delegate(object _,  ProcessInputEventArgs r)
            {
                if (r.StagingItem.Input is MouseButtonEventArgs || r.StagingItem.Input is KeyEventArgs)
                    _dispatcherTimer.Interval = TimeSpan.FromSeconds(300);
            };        
        }

        void checkForTime_Elapsed(object? sender, EventArgs e)
        {
            Layout.Children.Remove(_quranImages);
            // Create a BitmapSource  
            BitmapImage bitmap = new BitmapImage();  
            bitmap.BeginInit();
            var rand = new Random();
            var files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+ @"\Resources\images", "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            var enumerable = files as string[] ?? files.ToArray();
            var images = enumerable[rand.Next(enumerable.Length)];
            var imagesUri = new Uri(images, UriKind.Absolute);
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;      
            bitmap.UriSource = imagesUri;  
            bitmap.EndInit();

            // Set Image.Source  
            _quranImages.Source = bitmap;
            Layout.Children.Add(_quranImages);
            UpdateLayout();
            Show();
            Activate(); 
            EndApp();
        }
    }

}