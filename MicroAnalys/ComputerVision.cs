using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace MicroAnalys
{
    public class ComputerVision
    {
        public struct SearchParams
        {
            public double Param1;
            public double Param2;
            public double Dp;
            public double MinDist;
            public int Offset;

            public static SearchParams GenerateParams()
            {
                var searchParams = new SearchParams();
                searchParams.Param1 = 1;
                searchParams.Param2 = 0.4;
                searchParams.Dp = 1;
                searchParams.MinDist = 10;
                searchParams.Offset = 5;

                return searchParams;
            }
        }

        public static CircleF[] RunAnalys(string path, SearchParams searchParams, int radius, double scalePixel, int scale)
        {
            double nmInPixel = scale / scalePixel; // сколько нанометров в одноим пискеле
            int minr = (int)Math.Round((double)(radius * nmInPixel - searchParams.Offset) / nmInPixel);
            int maxr = (int)Math.Round((double)(radius * nmInPixel + searchParams.Offset) / nmInPixel); 

            var img = CvInvoke.Imread(path, Emgu.CV.CvEnum.ImreadModes.AnyColor);
            Mat finalMat = new Mat(img.Rows, img.Cols, Emgu.CV.CvEnum.DepthType.Cv8U, 4);
            CvInvoke.CvtColor(img, finalMat, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            var circles = CvInvoke.HoughCircles(finalMat, Emgu.CV.CvEnum.HoughModes.GradientAlt, searchParams.Dp,
                searchParams.MinDist / nmInPixel, searchParams.Param1, searchParams.Param2, minr, maxr);

            return circles;
        }
    }
}
