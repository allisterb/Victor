using System;
using Emgu.CV;
//using Emgu.CV.UI;
using Emgu.CV.Structure;
using System.Drawing;

namespace Victor.Vision
{
    public class CV
    {
        public static void CaptureCamera()
        {
            VideoCapture c = new VideoCapture();
            var v = c.QueryFrame();
            v.Save("capture.png");
          
            //Emu.CV.Capture capture = new Capture(); //create a camera captue
            //Application.Idle += new EventHandler(delegate (object sender, EventArgs e)
            //{  //run this until application closed (close button click on image viewer)
            //    viewer.Image = capture.QueryFrame(); //draw the image obtained from camera
            //});
            //viewer.ShowDialog(); //show the image viewer
        }
    }
}
