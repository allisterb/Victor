using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using static Emgu.CV.CvInvoke;

namespace Victor.Vision.EmguCV
{
    public class DocumentScanner
    {
        public static void PreProcess(Mat src, ref Mat dst)
        {
            Mat imageGrayed = new Mat();
            Mat imageOpen = new Mat();
            Mat imageClosed = new Mat();
            Mat imageBlurred = new Mat();

            CvtColor(src, imageGrayed, ColorConversion.Bgr2Gray);

            Mat structuringElmt = GetStructuringElement(ElementShape.Ellipse,  new Size(4, 4), new Point(-1, -1));
            MorphologyEx(imageGrayed, imageOpen, MorphOp.Open, structuringElmt, new Point(-1, -1), 1, 0, new MCvScalar(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue));
            MorphologyEx(imageOpen, imageClosed, MorphOp.Close, structuringElmt, new Point(-1, -1), 1, 0, new MCvScalar(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue));

            GaussianBlur(imageClosed, imageBlurred, new Size(7, 7), 0);
            Canny(imageBlurred, dst, 75, 100);
        }
    }
}
