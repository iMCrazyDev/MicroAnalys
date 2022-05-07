using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using static MicroAnalys.PythonExecutor;

namespace MicroAnalys
{
    public class MainViewModel : BindableBase
    {

        public static Settings settings = Settings.LoadSettings();
        public ICommand selectScaleClick
        {
            get
            {
                return new RelayCommand(o => {
                    try { ScaleWindow.Close(); } catch { }
                    if (imgBox == null) { MessageBox.Show("Изображение не загружено"); return; }
                    ScaleWindow = new ScaleWindow(imgBox);
                    ScaleWindow.Show();
                });
            }
        }

        public ScaleWindow ScaleWindow { get; set; } = new ScaleWindow();
        public ImageSource imgBox { get; 
            set; }
        private static Dictionary<string, int> PrefixToScale;
        public IEnumerable<KeyValuePair<string, int>> PrefixData
        {
            get
            {
                return PrefixToScale.ToList();
            }
        }

        public String scaleGetter => ScaleWindow?.result.ToString() ?? "Не установлено";
        public bool ShowAnalys;


        public PlotModel MyModel { get; private set; }
        public RectangleBarSeries barSeries;
        public MainViewModel()
        {
            InitPrefixes();
            ShowAnalys = true;

            MyModel = new PlotModel { Title = "Распределение частиц по размеру" };

            barSeries = new RectangleBarSeries()
            {
                TextColor = OxyColor.FromRgb(0, 255, 0),
                FillColor = OxyColor.FromRgb(100, 100, 255)
            };

            MyModel.Series.Add(barSeries);
            //barSeries.Items.Add(new RectangleBarItem(1, 0, 10, 5)
            //{
            //    Title = $"123"
            //});
        }

        public KeyValuePair<string, int> Exp
        {
            get { return PrefixToScale.First(x => x.Value == settings.Exp); }
            set
            {
                settings.Exp = value.Value;
            }
        }

        public bool? DontShowAnalys
        {
            get { return !ShowAnalys; }
            set
            {
                ShowAnalys = !((value == null) ? false : true);
            }
        }

        public string Scale
        {
            get { return settings.Scale.ToString(); }
            set
            {
                int res;
                if (int.TryParse(value, out res))
                {
                    settings.Scale = res; 
                }
            }
        }

        public string GraphSize
        {
            get { return settings.GraphSize.ToString(); }
            set
            {
                int res;
                if (int.TryParse(value, out res))
                {
                    settings.GraphSize = res;
                }
            }
        }

        public string Offset
        {
            get { return settings.SearchParams.Offset.ToString(); }
            set
            {
                int res;
                if (int.TryParse(value, out res))
                {
                    settings.SearchParams.Offset = res;
                }
            }
        }

        public string Param1
        {
            get { return settings.SearchParams.Param1.ToString(); }
            set
            {
                double res;
                if (double.TryParse(value, out res))
                {
                    settings.SearchParams.Param1 = res;
                }
            }
        }

        public string Param2
        {
            get { return settings.SearchParams.Param2.ToString(); }
            set
            {
                double res;
                if (double.TryParse(value, out res))
                {
                    settings.SearchParams.Param2 = res;
                }
            }
        }

        public string MinDistance
        {
            get { return settings.SearchParams.MinDist.ToString(); }
            set
            {
                double res;
                if (double.TryParse(value, out res))
                {
                    settings.SearchParams.MinDist = res;
                }
            }
        }

        private void InitPrefixes()
        {
             PrefixToScale = new Dictionary<string, int>()
                {
                    { "нм", -9 },
                    { "мкм", -6 }
                };
        }

    }
}
