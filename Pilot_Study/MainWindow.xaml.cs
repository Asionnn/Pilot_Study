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
using System.Windows.Media.Animation;

namespace Pilot_Study
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


        /*Notes*/
        //2 per min 20 seconds in between
        //user does not press a key at all, assign answer as ? 2 seconds before next alert
        //capture angle when choice is made
        //use txt files instead of sql tables now


    
    public partial class MainWindow : Window
    {
        int userRight;
        int userWrong;
        int systemRight;
        int systemWrong;

        

        bool interrupted;
        private static readonly Random getrandom = new Random();
        int angle;
        private int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        private int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;

        //images
        Image airspeed, attitude_outer, attitude_inner, red_arrow, altimeter;

        //used to rotate image
        Storyboard sb;

        DoubleAnimation [] rotations = new DoubleAnimation[8];
        DoubleAnimation prevRotation;

        public MainWindow()
        {

            angle = 0;
            interrupted = false;

            WindowState = WindowState.Maximized;
            InitializeComponent();
            
            //create images
            airspeed = new Image();
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/Airspeed_Indicator.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            airspeed.Source = bi;
            airspeed.Width = 400;
            airspeed.Height = 400;

            attitude_outer = new Image();
            BitmapImage bi2 = new BitmapImage();
            bi2.BeginInit();
            bi2.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/attitude_outer.png", UriKind.RelativeOrAbsolute);
            bi2.EndInit();
            attitude_outer.Source = bi2;
            attitude_outer.Width = 400;
            attitude_outer.Height = 400;

            attitude_inner = new Image() {
                RenderTransform = new RotateTransform(0,200,200)
            };
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri("C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/attitude_inner.png", UriKind.RelativeOrAbsolute);
            bi3.EndInit();
            attitude_inner.Source = bi3;
            attitude_inner.Width = 400;
            attitude_inner.Height = 400;
    

            red_arrow = new Image();
            BitmapImage bi4 = new BitmapImage();
            bi4.BeginInit();
            bi4.UriSource = new Uri("C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/red_arrow.png", UriKind.RelativeOrAbsolute);
            bi4.EndInit();
            red_arrow.Source = bi4;
            red_arrow.Width = 400;
            red_arrow.Height = 400;


            altimeter = new Image();
            BitmapImage bi5 = new BitmapImage();
            bi5.BeginInit();
            bi5.UriSource = new Uri(@"C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/Altimeter.png", UriKind.RelativeOrAbsolute);
            bi5.EndInit();
            altimeter.Source = bi5;
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
            canvas.Children.Add(red_arrow);
            canvas.Children.Add(altimeter);

            //set locations of meters
            Canvas.SetTop(airspeed, screenHeight - airspeed.Height-100);
            Canvas.SetLeft(airspeed, 10);

            Canvas.SetTop(attitude_inner, screenHeight - attitude_inner.Height - 100);
            Canvas.SetLeft(attitude_inner, screenWidth / 2 - attitude_inner.Width/2);

            Canvas.SetTop(red_arrow, screenHeight - red_arrow.Height - 100);
            Canvas.SetLeft(red_arrow, screenWidth / 2 - red_arrow.Width / 2);

            Canvas.SetTop(attitude_outer, screenHeight - attitude_outer.Height - 100);
            Canvas.SetLeft(attitude_outer, screenWidth / 2 - attitude_outer.Width / 2);

            Canvas.SetTop(altimeter, screenHeight - altimeter.Height - 100);
            Canvas.SetRight(altimeter, 10);

            //create timer
            dt.Tick += new EventHandler(dt_Tick);
            dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dt.Start();
            stopWatch.Start();

            sb = new Storyboard();

            sb.Duration = new Duration(TimeSpan.FromSeconds(5));


            //create default rotations
            DoubleAnimation rotate_0_10 = new DoubleAnimation()
            {
                From = 0,
                To = 90,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_20 = new DoubleAnimation()
            {
                From = 0,
                To = -90,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_30 = new DoubleAnimation()
            {
                From = 0,
                To = 30,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_60 = new DoubleAnimation()
            {
                From = 0,
                To = 60,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_10_n = new DoubleAnimation()
            {
                From = 0,
                To = -10,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_20_n = new DoubleAnimation()
            {
                From = 0,
                To = -20,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_30_n = new DoubleAnimation()
            {
                From = 0,
                To = -30,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_0_60_n = new DoubleAnimation()
            {
                From = 0,
                To = -60,
                Duration = sb.Duration
            };

            //fill array with default roatations
            rotations[0] = rotate_0_10;
            rotations[1] = rotate_0_20;
            rotations[2] = rotate_0_30;
            rotations[3] = rotate_0_60;
            rotations[4] = rotate_0_10_n;
            rotations[5] = rotate_0_20_n;
            rotations[6] = rotate_0_30_n;
            rotations[7] = rotate_0_60_n;

            Resources.Add("Storyboard", sb);
       
            //handle key presses
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                //balance meter to right
                interrupted = true;

                RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
                
                //set new duration so it isnt too slow when angle is smaller
                Duration newDuration;
                if(currentAngle.Angle > 0 && currentAngle.Angle < 45)
                {
                    newDuration = new Duration(TimeSpan.FromSeconds(1));
                }
                else
                {
                    newDuration = new Duration(TimeSpan.FromMilliseconds(2500));
                }
                DoubleAnimation rotateBack = new DoubleAnimation()
                {
                    From = currentAngle.Angle,
                    To = 0,
                    Duration = newDuration
                };
          
                if(currentAngle.Angle > 0)
                {
                    Storyboard.SetTarget(rotateBack, attitude_inner);
                    Storyboard.SetTargetProperty(rotateBack, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                    sb.Children.Add(rotateBack);
                    ((Storyboard)Resources["Storyboard"]).Begin();
                
                }
                
            }
            else if(e.Key == Key.Left)
            {
                //balance meter to left
                interrupted = true;

                RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;

                //set new duration so it isnt too slow when angle is smaller
                Duration newDuration;
                if (currentAngle.Angle < 0 && currentAngle.Angle > -45)
                {
                    newDuration = new Duration(TimeSpan.FromSeconds(1));
                }
                else
                {
                    newDuration = new Duration(TimeSpan.FromMilliseconds(2500));
                }

                DoubleAnimation rotateBack = new DoubleAnimation()
                {
                    From = currentAngle.Angle,
                    To = 0,
                    Duration = newDuration
                };

                if (currentAngle.Angle < 0)
                {
                    Storyboard.SetTarget(rotateBack, attitude_inner);
                    Storyboard.SetTargetProperty(rotateBack, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                    sb.Children.Add(rotateBack);
                    ((Storyboard)Resources["Storyboard"]).Begin();
                }
                
               
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
            
            if(currentAngle.Angle == 0)
            {
                interrupted = false;
            }

            TimeSpan ts = stopWatch.Elapsed;
            currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            if(ts.Milliseconds % 2 == 0 && !interrupted)
            {
                interrupted = true;
                int index = GetRandomNumber(0, 2);
                Storyboard.SetTarget(rotations[index], attitude_inner);
                Storyboard.SetTargetProperty(rotations[index], new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                sb.Children.Add(rotations[index]);
                ((Storyboard)Resources["Storyboard"]).Begin();
            }
        }

        //generates random number use 1-10
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }


    }
}
