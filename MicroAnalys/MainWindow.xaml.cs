using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroAnalys
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string ImagePath { get; private set; }
        private Dictionary<Brush, List<Emgu.CV.Structure.CircleF>> colorCircles = new Dictionary<Brush, List<Emgu.CV.Structure.CircleF>>();
        private List<Brush> usedBrushes = new List<Brush>();
        private List<Ellipse> lastEllipseList = new List<Ellipse>();
        private Ellipse lastEllipse { get => (lastEllipseList.Count > 0) ? lastEllipseList.Last() : null; }
        private List<Brush> brushes;
        private Random rand = new Random();
        internal static Brush lastColor;


        private void LoadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Изображения (.bmp)|*.bmp"
            };
            ofd.InitialDirectory = System.IO.Path.GetDirectoryName(MainViewModel.settings.LastFileName);

            if (ofd.ShowDialog() == true)
            {
                ImagePath = ofd.FileName;
                imgBox.Source = new BitmapImage(new Uri(ImagePath));
                MainViewModel.settings.LastFileName = ImagePath;
            }
        }

        public MainWindow()
        {
            InitBrushes();
            InitializeComponent();
        }

        private void imgBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Cross;
        }

        private void imgBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }
        private void SearchSpheres(object sender, RoutedEventArgs e)
        {
            if (lastEllipse == null)
            {
                MessageBox.Show("Поисковая сфера не поставлена");
                return;
            }

            if (ImagePath == null || ImagePath.Length == 0)
            {
                MessageBox.Show("Изображение не выбрано");
                return;
            }

            var dataContext = (this.DataContext as MainViewModel);

            if (dataContext.ScaleWindow == null || dataContext.ScaleWindow.result == "нет")
            {
                MessageBox.Show("Пиксельный масштаб не установлен");
                return;
            }

            int r = (int)Math.Round((lastEllipse.Width + lastEllipse.Height) / 4);
            var res = ComputerVision.RunAnalys(ImagePath, MainViewModel.settings.SearchParams, r, dataContext.ScaleWindow.scale, MainViewModel.settings.Scale);
            double k = imgBox.Source.Width / imgBox.ActualWidth;

            foreach (var t in res)
            {
                if (!colorCircles.ContainsKey(lastColor))
                {
                    colorCircles.Add(lastColor, new List<Emgu.CV.Structure.CircleF>() { t });
                }
                else
                {
                    colorCircles[lastColor].Add(t);
                }
                CreateEllipse(k, t);
            }
        }
        private void InitBrushes()
        {
            brushes = new List<Brush>();
            var props = typeof(Brushes).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var propInfo in props)
            {
                brushes.Add((Brush)propInfo.GetValue(null, null));
            }
        }
        private Brush GetRandomBrush()
        {
            var res = brushes[rand.Next(brushes.Count)];

            if (usedBrushes.Contains(res) || res == Brushes.Black)
            {
                return GetRandomBrush();
            }
            else
            {
                usedBrushes.Add(res);
                return res;
            }
        }

        private Ellipse CreateEllipse(double k, Emgu.CV.Structure.CircleF t, bool isFirst = false)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = t.Radius / k * 2; // k = источник/оригинал
            ellipse.Height = t.Radius / k * 2;
            ellipse.StrokeThickness = 4;
            ellipse.Fill = Brushes.Transparent;
            ellipse.Stroke = lastColor;


            canvass.Children.Add(ellipse);
            if (isFirst)
            {
                InkCanvas.SetLeft(ellipse, t.Center.X / k);
                InkCanvas.SetTop(ellipse, t.Center.Y / k);
            }
            else
            {
                InkCanvas.SetLeft(ellipse, t.Center.X / k - ellipse.Width / 2);
                InkCanvas.SetTop(ellipse, t.Center.Y / k - ellipse.Height / 2);
            }
            return ellipse;
        }

        private void CreateSphere(object sender, RoutedEventArgs e)
        {
            lastColor = GetRandomBrush();
            ChangeColorToLast();
            lastEllipseList.Add(CreateEllipse(1, new Emgu.CV.Structure.CircleF() { Radius = 100 }, true));
        }

        private void CreateSphereWithoutColorChange(object sender, RoutedEventArgs e)
        {
            CreateEllipse(1, new Emgu.CV.Structure.CircleF() { Radius = 100 }, true);
        }


        private void ChangeColorToLast()
        {
            SphereColor.Background = lastColor;
            SphereColor2.Background = lastColor;
        }

        private void RemoveLastSphere(object sender, RoutedEventArgs e)
        {
            if (usedBrushes.Count == 0)
            {
                return;
            }
            var clr = usedBrushes.Last();
            usedBrushes.RemoveAt(usedBrushes.Count - 1);

            if (usedBrushes.Count != 0)
            {
                lastColor = usedBrushes.Last();
                ChangeColorToLast();
            }
            if (lastEllipseList.Count > 0)
            {
                lastEllipseList.RemoveAt(lastEllipseList.Count - 1);
            }


            for (int i = 0; i < canvass.Children.Count; i++)
            {
                if (canvass.Children[i] is Ellipse ellipse && ellipse.Stroke == clr)
                {
                    canvass.Children.RemoveAt(i);
                    i--;
                }
            }
            if (colorCircles.ContainsKey(clr))
            {
                colorCircles.Remove(clr);
            }
        }

        private void RemoveAllSpheres(object sender, RoutedEventArgs e)
        {
            if (usedBrushes.Count == 0)
            {
                return;
            }
            usedBrushes.Clear();
            colorCircles.Clear();

            for (int i = 0; i < canvass.Children.Count; i++)
            {
                if (canvass.Children[i] is Ellipse)
                {
                    canvass.Children.RemoveAt(i);
                    i--;
                }
            }
        }

        private void DoAnalys(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.settings.GraphSize == 0)
            {
                MessageBox.Show("Цена деления на графике не может быть равна нулю");
                return;
            }

            var dataContext = (this.DataContext as MainViewModel);

            if (dataContext.ScaleWindow == null || dataContext.ScaleWindow.result == "нет")
            {
                MessageBox.Show("Пиксельный масштаб не установлен");
                return;
            }
            double nmInPixel = MainViewModel.settings.Scale / dataContext.ScaleWindow.scale; // сколько нанометров в одноим пискеле

            MainViewModel.settings.SaveSettings();

            var circles = new List<Emgu.CV.Structure.CircleF>();
            colorCircles.Values.ToList().ForEach(x => circles.AddRange(x));
            var circlesRadiues = circles.Select(x => x.Radius * 2 * nmInPixel).ToList();

            double min = circlesRadiues.Min();
            double max = circlesRadiues.Max();
            int minInt = (int)min;
            int maxInt = (int)Math.Ceiling(max);
            dataContext.barSeries.Items.Clear();
            int start = (minInt / MainViewModel.settings.GraphSize) * MainViewModel.settings.GraphSize;
            while (start <= maxInt)
            {
                var items = circlesRadiues.Count(x => (start <= x) && (x < (start + MainViewModel.settings.GraphSize)));

                if (items > 0)
                {
                    float percent = (float)items / circlesRadiues.Count;
                    dataContext.barSeries.Items.Add(new RectangleBarItem(start, 0, start + MainViewModel.settings.GraphSize, percent)
                    {
                        Title = $"{percent}%\n{start}-{start + MainViewModel.settings.GraphSize} {prefixList.Text},"
                    });
                }
                start += MainViewModel.settings.GraphSize;
            }
            dataContext.MyModel.InvalidatePlot(true);
            WriteCsv(circlesRadiues);
        }

        private void WriteCsv(List<double> r)
        {
            string time = DateTime.Now.ToString("dd MMMM (HH;mm;ss)");
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Результаты\\" + time + "\\";
            Directory.CreateDirectory(path);
            File.Copy(ImagePath, path + "image" + System.IO.Path.GetExtension(ImagePath));
            File.WriteAllText(path + "Результат.csv", "Диаметры сфер\n", Encoding.UTF8);
            File.AppendAllLines(path + "Результат.csv", r.Select(x => x.ToString()));
            Process.Start(path);
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked.Value)
            {
                foreach (var t in canvass.Children)
                {
                    if (t is Ellipse ellipse)
                    {
                        ellipse.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                foreach (var t in canvass.Children)
                {
                    if (t is Ellipse ellipse)
                    {
                        ellipse.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var frmScale = (this.DataContext as MainViewModel).ScaleWindow;
            frmScale?.Close();
        }
    }
}
