using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;

namespace Pilot_Study
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    
    public partial class MainWindow : Window
    {
        int userRight;
        int userWrong;
        int systemRight;
        int systemWrong;
        private static readonly Random getrandom = new Random();
        int angle;
        private int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        private int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;


        public MainWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();
            
            //create images
            Image airspeed = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/Airspeed_Indicator.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            airspeed.Source = bi;
            airspeed.Width = 400;
            airspeed.Height = 400;

            Image attitude_outer = new Image();
            BitmapImage bi2 = new BitmapImage();
            bi2.BeginInit();
            bi2.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/attitude_outer.png", UriKind.RelativeOrAbsolute);
            bi2.EndInit();
            attitude_outer.Source = bi2;
            attitude_outer.Width = 400;
            attitude_outer.Height = 400;

            Image attitude_inner = new Image();
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/attitude_inner.png", UriKind.RelativeOrAbsolute);
            bi3.EndInit();
            attitude_inner.Source = bi3;
            attitude_inner.Width = 400;
            attitude_inner.Height = 400;

            Image altimeter = new Image();
            BitmapImage bi4 = new BitmapImage();
            bi4.BeginInit();
            bi4.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/Altimeter.png", UriKind.RelativeOrAbsolute);
            bi4.EndInit();
            altimeter.Source = bi4;
            altimeter.Width = 400;
            altimeter.Height = 400;

            //create meterPanel
            Rectangle meterPanel = new Rectangle();
            SolidColorBrush greyBrush = new SolidColorBrush();
            greyBrush.Color = Colors.Gray;
            meterPanel.Fill = greyBrush;
            meterPanel.Height = screenHeight/2;
            meterPanel.Width = screenWidth;
            Canvas.SetTop(meterPanel, screenHeight / 2);
            
            //add images to canvas
            canvas.Children.Add(meterPanel);
            canvas.Children.Add(airspeed);
            canvas.Children.Add(attitude_inner);
            canvas.Children.Add(attitude_outer);
            canvas.Children.Add(altimeter);

            //set locations of meters
            Canvas.SetTop(airspeed, screenHeight - airspeed.Height-100);
            Canvas.SetLeft(airspeed, 10);
            Canvas.SetTop(attitude_inner, screenHeight - altimeter.Height - 100);
            Canvas.SetLeft(attitude_inner, screenWidth / 2 - attitude_inner.Width/2);
            Canvas.SetTop(attitude_outer, screenHeight - attitude_inner.Height - 100);
            Canvas.SetLeft(attitude_outer, screenWidth / 2 - attitude_outer.Width / 2);
            Canvas.SetTop(altimeter, screenHeight - altimeter.Height - 100);
            Canvas.SetRight(altimeter, 10);

            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dt.Start();
            stopWatch.Start();


        }

        void dt_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = stopWatch.Elapsed;
            currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            debugger2.Text = currentTime;
        }

        //generates random number
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }


    }
}
