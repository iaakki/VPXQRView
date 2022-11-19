using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //make borderless
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            //manifest makes this dpiaware, make sure the size is correct
            this.Size = new Size(390, 390);

            //make image to disappear after 5 seconds
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            //argument option vars
            string QRPath = "";
            Boolean bInvert = false;

            //parse arguments
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                MessageBox.Show("QRView.exe \"Path to QR-image\" <-i>\n\n -i\tInvert image colors");
                Application.Exit();
            }

            if (args.Length == 2)
            {
                if (File.Exists(args[1]))
                {
                    QRPath = args[1];
                } else
                {
                    MessageBox.Show("File not found! Exiting..");
                    Application.Exit();
                }
            }
            if (args.Length > 2)
            {
                if (args[2] == "-i")
                {
                    bInvert = true;
                    if (File.Exists(args[1]))
                    {
                        QRPath = args[1];
                    }
                    else
                    {
                        MessageBox.Show("File not found! Exiting..");
                        Application.Exit();
                    }
                } 
                else if (args[1] == "-i")
                {
                    bInvert = true;
                    if (File.Exists(args[2]))
                    {
                        QRPath = args[2];
                    }
                    else
                    {
                        MessageBox.Show("File not found! Exiting..");
                        Application.Exit();
                    }
                }
                else
                {
                    bInvert = false;
                }
            }

            //orig image var
            Image myimage = new Bitmap(QRPath);

            myimage = ResizeImage(myimage, 390, 390);

            //invert image is requested
            if (bInvert)
            {
                Bitmap bmpDest = null;
                using (Bitmap bmpSource = new Bitmap(myimage))
                {
                    bmpDest = new Bitmap(bmpSource.Width, bmpSource.Height);

                    for (int x = 0; x < bmpSource.Width; x++)
                    {
                        for (int y = 0; y < bmpSource.Height; y++)
                        {

                            Color clrPixel = bmpSource.GetPixel(x, y);

                            clrPixel = Color.FromArgb(255 - clrPixel.R, 255 -
                               clrPixel.G, 255 - clrPixel.B);

                            bmpDest.SetPixel(x, y, clrPixel);
                        }
                    }
                }
                //show inverted image
                this.BackgroundImage = bmpDest;
            }
            else
            {
                //show orig image
                this.BackgroundImage = myimage;
            }

            
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Application.Exit(); //this probably must be removed here
        }
        private static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Application.Exit();
        }
    }

}
