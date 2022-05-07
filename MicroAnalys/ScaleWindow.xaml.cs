using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MicroAnalys
{
    /// <summary>
    /// Логика взаимодействия для scaleWindow.xaml
    /// </summary>
    public partial class ScaleWindow : Window, INotifyPropertyChanged
    {
        public string result { get; set; } = "нет";
        public double scale = 1;
        public ScaleWindow()
        {
            InitializeComponent();
        }

        public ScaleWindow(ImageSource source)
        {
            InitializeComponent();
            imgBox.Source = source;
            imgBox.Stretch = Stretch.None;
            var matrix = Matrix.Identity;
            matrix.Scale(2, 2);
            matrix.Translate(-200, -500);
            imgBox.RenderTransform = new MatrixTransform (matrix);
            canvass.Select(new List<UIElement>() { Oleg });
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double wid = Oleg.Width;
            scale = wid * imgBox.Source.Width / (2 * imgBox.ActualWidth);
            result = scale.ToString();
            this.Close();
        }
    }
}
