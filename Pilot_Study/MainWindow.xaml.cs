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


        /*Notes/TODO*/
        //2 per min 20 seconds in between
        //create algorithm to assign messages to certain times
        //user does not press a key at all, assign answer as ? 2 seconds before next alert
        //capture angle when choice is made
        //use txt files instead of sql tables now
        //sound


    
    public partial class MainWindow : Window
    {
        int userRight;
        int userWrong;
        int systemRight;
        int systemWrong;
        
        bool interrupted;
        bool isTrial;
        bool alertActive;
        bool startDelay;

        private static readonly Random getrandom = new Random();
       
        private int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        private int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        DispatcherTimer dt = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        string currentTime = string.Empty;

        //images
        Image airspeed, attitude_outer, attitude_inner, red_arrow, altimeter, sky;
        Rectangle meterPanel, panelBorder;

        //buttons
        Button startBtn, trainBtn;

        //used to rotate image
        Storyboard sb;

        //store rotation
        DoubleAnimation [] rotations = new DoubleAnimation[2];
        DoubleAnimation prevRotation;

        //stores all alerts
        AlertMessage[] alertMessages = new AlertMessage[40];

        //stores time(in seconds) the alerts should be played
        //also doubles as the start time
        int [] alertTimes = new int[40];
        int alertPos;

        //stores the end time of the response
        int[] endTimes = new int[40];

        //stores the response time
        int[] reponseTimes = new int[40];

        //stores choice made
        char[] keyPressed = new char[40];

        //stores banking angles
        //FIND A WAY TO DYNAMICALLY STORE ANGLES

        //create labels
        Label alertMessage;
        Label sRight, sWrong, uRight, uWrong;

       

        public MainWindow()
        {
            WindowState = WindowState.Maximized;
            InitializeComponent();

            isTrial = false;
            alertActive = false;
            interrupted = false;
            startDelay = false;

            alertPos = 0;
 
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

            sky = new Image();
            BitmapImage bi6 = new BitmapImage();
            bi6.BeginInit();
            bi6.UriSource = new Uri("C:/Users/colli/source/repos/PSI/PSI/PSI/Pics/clouds.jpg", UriKind.RelativeOrAbsolute);
            bi6.EndInit();
            sky.Source = bi6;
            sky.Width = screenWidth;
            sky.Height = screenHeight/2;
            sky.Stretch = Stretch.Fill;


            //create meterPanel
            SolidColorBrush greyBrush = new SolidColorBrush();
            meterPanel = new Rectangle();
            greyBrush.Color = Colors.Gray;
            meterPanel.Fill = greyBrush;
            meterPanel.Height = screenHeight/2;
            meterPanel.Width = screenWidth;
            Canvas.SetTop(meterPanel, screenHeight / 2);

            SolidColorBrush blackBrush = new SolidColorBrush();
            panelBorder = new Rectangle();
            blackBrush.Color = Colors.Black;
            panelBorder.Fill = blackBrush;
            panelBorder.Height = 5;
            panelBorder.Width = screenWidth;
            Canvas.SetTop(panelBorder, screenHeight / 2);
            
            //add images to canvas
            canvas.Children.Add(meterPanel);
            canvas.Children.Add(panelBorder);
            canvas.Children.Add(airspeed);
            canvas.Children.Add(attitude_inner);
            canvas.Children.Add(attitude_outer);
            canvas.Children.Add(red_arrow);
            canvas.Children.Add(altimeter);
            canvas.Children.Add(sky);

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
            

            sb = new Storyboard();

            sb.Duration = new Duration(TimeSpan.FromSeconds(5));


            //create default rotations
            DoubleAnimation rotate_90 = new DoubleAnimation()
            {
                From = 0,
                To = 90,
                Duration = sb.Duration
            };
            DoubleAnimation rotate_90_n = new DoubleAnimation()
            {
                From = 0,
                To = -90,
                Duration = sb.Duration
            };

            //creat alert message label and set location
            alertMessage = new Label();
            alertMessage.Width = screenWidth;
            alertMessage.Foreground = Brushes.Red;
            alertMessage.FontSize = 40;
            alertMessage.FontWeight = FontWeights.Bold;
            alertMessage.FontFamily = new FontFamily("Verdana");
            alertMessage.HorizontalContentAlignment = HorizontalAlignment.Center;
            canvas.Children.Add(alertMessage);
            Canvas.SetTop(alertMessage, screenHeight / 2 - 100);

            //create right/wrong labels
            sRight = new Label();
            sRight.Content = "#System Right: 0";
            sRight.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(sRight, 0);
            sWrong = new Label();
            sWrong.Content = "#System Wrong: 0";
            sWrong.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(sWrong, 0);
            Canvas.SetTop(sWrong, 50);
            uRight = new Label();
            uRight.Content = "#User Right: 0";
            uRight.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(uRight, 0);
            Canvas.SetTop(uRight, 100);
            uWrong = new Label();
            uWrong.Content = "#User Wrong: 0";
            uWrong.FontWeight = FontWeights.Bold;
            Canvas.SetLeft(uWrong, 0);
            Canvas.SetTop(uWrong, 150);

            canvas.Children.Add(sRight);
            canvas.Children.Add(sWrong);
            canvas.Children.Add(uRight);
            canvas.Children.Add(uWrong);




            //fill array with default roatations
            rotations[0] = rotate_90;
            rotations[1] = rotate_90_n;
 

            Resources.Add("Storyboard", sb);
       
            //handle key presses
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            
            //set all elements invisible
            meterPanel.Visibility = Visibility.Hidden;
            panelBorder.Visibility = Visibility.Hidden;
            airspeed.Visibility = Visibility.Hidden;
            attitude_outer.Visibility = Visibility.Hidden;
            attitude_inner.Visibility = Visibility.Hidden;
            altimeter.Visibility = Visibility.Hidden;
            sky.Visibility = Visibility.Hidden;
            red_arrow.Visibility = Visibility.Hidden;
            alertMessage.Visibility = Visibility.Hidden;
            sRight.Visibility = Visibility.Hidden;
            sWrong.Visibility = Visibility.Hidden;
            uRight.Visibility = Visibility.Hidden;
            uWrong.Visibility = Visibility.Hidden;

            startBtn = new Button()
            {
                Content = "Start Trial",
                Width = 200,
                Height = 100,
                FontSize = 20,
                FontWeight = FontWeights.Bold
            };
            startBtn.Click += startBtn_Click;
            canvas.Children.Add(startBtn);

            trainBtn = new Button()
            {
                Content = "Train",
                Width = 200,
                Height = 100,
                FontSize = 20,
                FontWeight = FontWeights.Bold
            };
            trainBtn.Click += trainBtn_Click;
            canvas.Children.Add(trainBtn);

            Canvas.SetTop(startBtn, screenHeight / 2);
            Canvas.SetLeft(startBtn, screenWidth / 2 - startBtn.Width/2);
            Canvas.SetTop(trainBtn, screenHeight / 2 - trainBtn.Height*2);
            Canvas.SetLeft(trainBtn, screenWidth / 2 - trainBtn.Width/2);

            initMessages();
            generateAlertTimes();
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            isTrial = true;
            //start timing
            dt.Start();
            stopWatch.Start();

            //hide startBtn & trainBtn
            startBtn.Visibility = Visibility.Hidden;
            trainBtn.Visibility = Visibility.Hidden;

            //make everything visible
            meterPanel.Visibility = Visibility.Visible;
            panelBorder.Visibility = Visibility.Visible;
            airspeed.Visibility = Visibility.Visible;
            attitude_outer.Visibility = Visibility.Visible;
            attitude_inner.Visibility = Visibility.Visible;
            altimeter.Visibility = Visibility.Visible;
            sky.Visibility = Visibility.Visible;
            red_arrow.Visibility = Visibility.Visible;
            sRight.Visibility = Visibility.Visible;
            sWrong.Visibility = Visibility.Visible;
            uRight.Visibility = Visibility.Visible;
            uWrong.Visibility = Visibility.Visible;

        }

        private void trainBtn_Click(object sender, RoutedEventArgs e)
        {
            debugger2.Text = "train clicked";
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
            else if(e.Key == Key.A && alertActive)
            {
                //accept
                if (alertMessages[alertPos].isAccurate())
                {
                    systemRight++;
                    userRight++;
                }
                else if (!alertMessages[alertPos].isAccurate())
                {
                    systemWrong++;
                    userWrong++;
                }

                sRight.Content = "#System Right: " + systemRight;
                sWrong.Content = "#System Wrong: " + systemWrong;
                uRight.Content = "#User Right: " + userRight;
                uWrong.Content = "#User Wrong: " + userWrong;
                RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
                TimeSpan ts = stopWatch.Elapsed;
                keyPressed[alertPos] = 'A';
                endTimes[alertPos] = ts.Minutes * 60 + ts.Seconds;
                alertMessage.Visibility = Visibility.Hidden;
                alertPos++;
                alertActive = false;


            }
            else if(e.Key == Key.S && alertActive)
            {
                //deny
                if (alertMessages[alertPos].isAccurate())
                {
                    systemRight++;
                    userWrong++;
                }
                else if (!alertMessages[alertPos].isAccurate())
                {
                    systemWrong++;
                    userRight++;
                }
                sRight.Content = "#System Right: " + systemRight;
                sWrong.Content = "#System Wrong: " + systemWrong;
                uRight.Content = "#User Right: " + userRight;
                uWrong.Content = "#User Wrong: " + userWrong;
                RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
                TimeSpan ts = stopWatch.Elapsed;
                keyPressed[alertPos] = 'S';
                endTimes[alertPos] = ts.Minutes * 60 + ts.Seconds;
                alertMessage.Visibility = Visibility.Hidden;
                alertPos++;
                alertActive = false;

            }
            else if(e.Key == Key.D && alertActive)
            {
                //unsure
                if (alertMessages[alertPos].isAccurate())
                {
                    systemRight++;
                }
                else if (!alertMessages[alertPos].isAccurate())
                {
                    systemWrong++;
                }
                sRight.Content = "#System Right: " + systemRight;
                sWrong.Content = "#System Wrong: " + systemWrong;
                uRight.Content = "#User Right: " + userRight;
                uWrong.Content = "#User Wrong: " + userWrong;
                RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
                TimeSpan ts = stopWatch.Elapsed;
                keyPressed[alertPos] = 'D';
                endTimes[alertPos] = ts.Minutes * 60 + ts.Seconds;
                alertMessage.Visibility = Visibility.Hidden;
                alertPos++;
                alertActive = false;
            }
        }

        void dt_Tick(object sender, EventArgs e)
        {
            int secondsPassed;
            RotateTransform currentAngle = attitude_inner.RenderTransform as RotateTransform;
            
            if(currentAngle.Angle == 0)
            {
                interrupted = false;
            }

            TimeSpan ts = stopWatch.Elapsed;
            currentTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            secondsPassed = ts.Minutes * 60 + ts.Seconds;

            if(ts.Seconds > 1)
            {
                startDelay = true;
            }

            if(startDelay && ts.Milliseconds % 1 == 0 && !interrupted)
            {
                interrupted = true;
                int index = GetRandomNumber(0, 2);
                Storyboard.SetTarget(rotations[index], attitude_inner);
                Storyboard.SetTargetProperty(rotations[index], new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
                sb.Children.Add(rotations[index]);
                ((Storyboard)Resources["Storyboard"]).Begin();
            }

            if(secondsPassed == alertTimes[alertPos])
            {
                alertActive = true;
                alertMessage.Content = alertMessages[alertPos].getMessage();
                alertMessage.Visibility = Visibility.Visible;
            }

            if(keyPressed[alertPos] == 0 && (alertTimes[39] - secondsPassed == 5 ||alertTimes[alertPos+1] - secondsPassed == 5))
            {
                if (alertMessages[alertPos].isAccurate())
                {
                    systemRight++;
                }
                else if (!alertMessages[alertPos].isAccurate())
                {
                    systemWrong++;
                }

                sRight.Content = "#System Right: " + systemRight;
                sWrong.Content = "#System Wrong: " + systemWrong;
                uRight.Content = "#User Right: " + userRight;
                uWrong.Content = "#User Wrong: " + userWrong;

                keyPressed[alertPos] = 'D';
                alertPos++;
                alertActive = false;
                alertMessage.Visibility = Visibility.Hidden;
            }

            
        }


       //initializes all 40 messages and randomizes their positions
       public void initMessages()
        {
            int[] certainties = { 0 ,30, 50, 80 };
            int pos = 0;
            int cpos = 0;

            for(int x = 0; x < 4; x++)
            {
               for(int y = 0; y < 10; y++)
                {
                    if(y <= 6)
                    {
                        if(cpos == 0)
                        {
                            alertMessages[pos] = new AlertMessage(certainties[cpos], "POSSIBLE THREAT", true);
                        }
                        else
                        {
                            alertMessages[pos] = new AlertMessage(certainties[cpos], "POSSIBLE THREAT: "+ certainties[cpos] + "% CERTAINTY", true);
                        }
                        
                    }
                    else
                    {
                        if (cpos == 0)
                        {
                            alertMessages[pos] = new AlertMessage(certainties[cpos], "POSSIBLE THREAT", false);
                        }
                        else
                        {
                            alertMessages[pos] = new AlertMessage(certainties[cpos], "POSSIBLE THREAT: " + certainties[cpos] + "% CERTAINTY", false);
                        }
                    }
                    pos++;
                }
                cpos++;
            }

            //randomize
            for(int x = alertMessages.Length-1;x >= 0; x--)
            {
                int randIndex = getrandom.Next(x + 1);
                AlertMessage temp = alertMessages[randIndex];
                alertMessages[randIndex] = alertMessages[x];
                alertMessages[x] = temp;
            }
        }

        //generate the times the alerts are played
       public void generateAlertTimes()
       {
            int totalTime = 10;
            for(int x = 0; x < 40; x++)
            {
                int offset = GetRandomNumber(1, 6);
                totalTime += offset;

                alertTimes[x] = totalTime;
                totalTime += 28;

            }
       }

        //generates random number between min and max-1 (inclusive)
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }

    }
}
