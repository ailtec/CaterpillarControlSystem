using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CatControllSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int U, D, L, R;
        double gridHeight, gridWidth;
        DispatcherTimer timer = new DispatcherTimer();
        string start = "S", spice = "$", booster = "B", obstacle = "#";


        public MainWindow()
        {
            InitializeComponent();

            myCanvas.Focus();

            Application.Current.MainWindow.Width = myCanvas.Width + 20;
            Application.Current.MainWindow.Height = myCanvas.Height + 90;

            LoadMap(out gridHeight, out gridWidth);

            Cat.Height = gridHeight; Cat.Width = gridWidth;
        }

        private void BtnStartRide(object sender, RoutedEventArgs e)
        {
            #region validate inputs
            if (!int.TryParse(inpUp.Text, out U))
            {
                Utils.MessageboxAlert("U must be a number >=0");
                inpUp.Clear();
                return;
            }
            if (!int.TryParse(inpDn.Text, out D))
            {
                Utils.MessageboxAlert($"D must be a number >=0");
                inpDn.Clear();
                return;
            }
            if (!int.TryParse(inpLf.Text, out L))
            {
                Utils.MessageboxAlert($"L must be a number >=0");
                inpLf.Clear();
                return;
            }
            if (!int.TryParse(inpRt.Text, out R))
            {
                Utils.MessageboxAlert($"R must be a number >=0");
                inpRt.Clear();
                return;
            }
            #endregion

            #region log rider commands
            Utils.WriteLog($"U {U}");
            Utils.WriteLog($"D {D}");
            Utils.WriteLog($"L {L}");
            Utils.WriteLog($"R {R}");
            #endregion

            #region clear input fields
            inpUp.Clear();
            inpDn.Clear();
            inpLf.Clear();
            inpRt.Clear();
            #endregion

            #region Start timer 
            timer.Interval = TimeSpan.FromSeconds(1); 
            timer.Tick += appTimer;
            timer.Start();
            #endregion
        }

        private void appTimer(object sender, EventArgs e)
        {
            MoveAndCollect();
        }

        private void MoveAndCollect()
        {
            try
            {
                #region move caterpillar
                if (U > 0)//Move Up
                {
                    Canvas.SetTop(Cat, Canvas.GetTop(Cat) - gridHeight);
                    U--;
                    return;
                }
                if (D > 0)//Move down
                {
                    Canvas.SetTop(Cat, Canvas.GetTop(Cat) + gridHeight);
                    D--;
                    return;
                }
                if (L > 0)//Move left
                {
                    Canvas.SetLeft(Cat, Canvas.GetLeft(Cat) - gridWidth);
                    L--;
                    return;
                }
                if (R > 0)//Move right
                { 
                    Canvas.SetLeft(Cat, Canvas.GetLeft(Cat) + gridWidth);
                    R--;
                    return;
                }

                timer.Stop();
                #endregion

            }
            catch (Exception)
            {
            }
        }

        //private void Colide(string dir)
        //{
        //    var Rectangles = myCanvas.Children.OfType<Rectangle>().Where(x => x.Tag != null);

        //    foreach (var rec in Rectangles)
        //    {
        //        if (rec.Tag.Equals("obstacle"))
        //        {
        //            rec.Stroke = Brushes.Black;
        //            Rect catHitBox = new Rect(Canvas.GetLeft(Cat), Canvas.GetTop(Cat), Cat.Width, Cat.Height);
        //            Rect toColide = new Rect(Canvas.GetLeft(rec), Canvas.GetTop(rec), rec.Width, rec.Height);

        //            if (catHitBox.IntersectsWith(toColide))
        //            {
        //                if (dir.Equals("x"))
        //                {
        //                    Canvas.SetLeft(Cat, Canvas.GetLeft(Cat) - SpeedX);
        //                    SpeedX = 0;
        //                }
        //                else
        //                {
        //                    Canvas.SetTop(Cat, Canvas.GetTop(Cat) + SpeedY);
        //                    SpeedY = 0;
        //                }

        //            }
        //        }
        //    }

        //}

        private void LoadMap(out double gridHeight, out double gridWidth)
        {
            gridHeight = gridWidth = 0;
            string mapDir = @"C:\\Users\\ALI\\source\\repos\\CaterpillarControlSystem\\CatControllSystem\\Maps\\Map.ini";

            if (!File.Exists(mapDir))
            {
                Utils.MessageboxAlert($"Map file not found: {mapDir}");
                return;
            }

            string[] lines = File.ReadAllLines(mapDir).Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();

            if (lines.Length == 0)
            {
                Utils.MessageboxAlert($"No data on");
                return;
            }

            int Columns = lines.FirstOrDefault().Length;
            int Rows = lines.Length;

            double canvasHeight = myCanvas.Height;
            double canvasWidth = myCanvas.Width;

            gridHeight = (canvasHeight / Rows);
            gridWidth = (canvasWidth / Columns);

            double y = 0;

            foreach (string line in lines)//Rows
            {
                double x = 0;

                for (int i = 0; i < line.Length; i++)//Cellss
                {
                    string item = line[i].ToString();

                    if (item.Equals(start, StringComparison.OrdinalIgnoreCase))
                    {
                        Canvas.SetLeft(Cat, x);
                        Canvas.SetTop(Cat, y);
                    }
                    else
                    {
                        Label label = new Label
                        {
                            Tag = item,
                            FontSize = 10,
                            Content = item,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                        };

                        // Set   position 
                        Canvas.SetLeft(label, x);
                        Canvas.SetTop(label, y);
                        myCanvas.Children.Add(label);
                    }

                    x += gridWidth;
                }
                y += gridHeight;

            }
        }
    }
}
