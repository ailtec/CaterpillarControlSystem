using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            Cat.Height = gridHeight;
            Cat.Width = gridWidth;
        }

        private void BtnStartRide(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnStart.Content = "Moving";

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
                    Colide(Direction.UP);
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
                #endregion

                timer.Stop();
                btnStart.IsEnabled = true;
                btnStart.Content = "Start";
            }
            catch (Exception)
            {
            }
        }

        private void Colide(Direction dir)
        {
            try
            {
                switch (dir)
                {
                    case Direction.UP:
                        break;
                    case Direction.DOWN:
                        break;
                    case Direction.LEFT:
                        break;
                    case Direction.RIGHT:
                        break;
                }

                IEnumerable<Grid> grids = myCanvas.Children.OfType<Grid>().Where(x => x.Tag != null);

                foreach (var grid in grids)
                {
                    Rect catHitBox = new Rect(Canvas.GetLeft(Cat), Canvas.GetTop(Cat), Cat.Width, Cat.Height);
                    Rect toColide = new Rect(Canvas.GetLeft(grid), Canvas.GetTop(grid), grid.Width, grid.Height);

                    if (catHitBox.IntersectsWith(toColide))
                    {
                        string currItem = grid.Tag.ToString();
                        Console.WriteLine(currItem);
                        if (currItem.Equals(start, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Hit   {grid.Tag}");
                        }
                        else if (currItem.Equals(spice, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Hit   {grid.Tag}");
                        }
                        else if (currItem.Equals(booster, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Hit   {grid.Tag}");
                        }
                        else if (currItem.Equals(obstacle, StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Hit   {grid.Tag}");
                        }

                    }
                }
            }
            catch (Exception)
            {
            }

        }

        private void LoadMap(out double gridHeight, out double gridWidth)
        {
            gridHeight = gridWidth = 0;
            string mapDir = @"C:\\Users\\ALI\\source\\repos\\CaterpillarControlSystem\\CatControllSystem\\Maps\\Map.ini";

            if (!File.Exists(mapDir))
            {
                Utils.MessageboxAlert($"Map file not found: {mapDir}");
                Application.Current.Shutdown();
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

                for (int i = 0; i < line.Length; i++)//Cells
                {
                    string item = line[i].ToString().Trim();

                    if (item.Equals(start, StringComparison.OrdinalIgnoreCase))
                    {
                        Canvas.SetLeft(Cat, x);
                        Canvas.SetTop(Cat, y);
                    }
                    else
                    {
                        Grid grid = new Grid()
                        {
                            Height = gridHeight,
                            Width = gridWidth,
                            Tag = item,
                        };
                        grid.Children.Add(new Rectangle()
                        {
                            Stroke = Brushes.Black,
                            Height = gridHeight,
                            Width = gridWidth,
                            StrokeThickness = 0.1,
                            Tag = item

                        });
                        grid.Children.Add(new TextBlock()
                        {
                            Tag = item,
                            Text = item,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        });

                        // Set   position 
                        Canvas.SetLeft(grid, x);
                        Canvas.SetTop(grid, y);
                        myCanvas.Children.Add(grid);
                    }

                    x += gridWidth;
                }
                y += gridHeight;

            }
        }

        private enum Direction
        {
            UP, DOWN, LEFT, RIGHT
        }
    }
}
