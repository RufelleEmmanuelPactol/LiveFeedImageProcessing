using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace DIP2
{
    public partial class Form1 : Form
    {
        private Mode currMode = Mode.COPY;
        private bool cameraMode = false;
        
        private bool capture = false;
        private enum Mode
        {
            COPY, INVERT, SEPIA, GREYSCALE, SUBTRACTION, HISTOGRAM
        }

        private bool run = false;
        
        
        
        public Form1()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            currMode = Mode.COPY;
            




        }

        private async Task start()
        {
            if (run) return;
            run = true;

            try
            {
                while (run) // Use 'run' as the condition to continue the loop
                {
                     // Non-blocking delay

                    switch (currMode)
                    {
                        case Mode.COPY:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                basicCopy();
                            });
                            break;
                        case Mode.GREYSCALE:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                greyscale();
                            });
                            break;
                        case Mode.INVERT:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                invert();
                            });
                            break;
                        case Mode.SEPIA:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                turnSepia();
                            });
                            break;
                        case Mode.HISTOGRAM:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                histogram();
                            });
                            break;
                        case Mode.SUBTRACTION:
                            
                            // Ensure thread-safe call to UI
                            Invoke((MethodInvoker)delegate
                            {
                                subtract();
                            });
                            break;
                        // Add other cases as required
                    } await Task.Delay(30);

                    // No need for Application.DoEvents();
                }
            }
            finally
            {
                run = false;
            }
        }
        
        private void histogram()
        {
            Bitmap img1;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            var bits = getBitMap(img1);

            // Normalization factor
            float scaleFactor = 255.0f / bits.Max();

            Bitmap img2 = new Bitmap(255, 255);  // Fixed height at 255

            for (int x = 0; x < 255; x++)
            {
                // Scale the intensity to fit in the range [0, 255]
                int scaledIntensity = (int)(bits[x] * scaleFactor);

                // Draw the histogram
                for (int y = 255 - 1; scaledIntensity > 0; y--, scaledIntensity--)
                {
                    img2.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                }
            }

            pictureBox3.Image = img2;
        }
        private int[] getBitMap(Bitmap img1)
        {
            int[] bits = new int[256];
            for (int x = 0; x < img1.Width; x++)
            {
                for (int y = 0; y < img1.Height; y++)
                {
                    Color c = img1.GetPixel(x, y);
                    int value = 255 - ((c.R + c.G + c.B) / 3); // greyscale histogram
                    bits[value]++;
                }
                
            }

            return bits;
        }
        
        
        private void turnSepia()
        { Bitmap img1;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;

            Bitmap first = img1;
            Bitmap second = new Bitmap(first.Width, first.Height);

            for (int x = 0; x < first.Width; x++)
            {
                for (int y = 0; y < first.Height; y++)
                {
                    var pix = first.GetPixel(x, y);
                    int gray = (pix.B + pix.R + pix.G) / 3;
                    int red = Math.Min(255, (int)(gray * 0.393 + gray * 0.769 + gray * 0.189));
                    int green = Math.Min(255, (int)(gray * 0.349 + gray * 0.686 + gray * 0.168));
                    int blue = Math.Min(255, (int)(gray * 0.272 + gray * 0.534 + gray * 0.131));

                    // Set the new pixel
                    second.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }

            pictureBox3.Image = second;

        }
        
        private void invert()
        {
            Bitmap img1;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            var img2 = new Bitmap(img1.Width, img1.Height);
            for (int x = 0; x < img1.Width; x++)
            {
                for (int y = 0; y < img1.Height; y++)
                {
                    Color c = img1.GetPixel(x, y);
                    Color greyscale = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
                    img2.SetPixel(x, y, greyscale);
                    

                }
                
            }

            pictureBox3.Image = img2;
        }

        
        
        
        
        
        
        private void basicCopy()
        {
            Bitmap img1;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            
             
            var img2 = new Bitmap(img1.Width, img1.Height);
           
            for (int i = 0; i < img1.Height; i++)
            {
                for (int j = 0; j < img1.Width; j++)
                {
                    Color c = img1.GetPixel(j, i);
                    img2.SetPixel(j, i, c);
                }
            }

            pictureBox3.Image = img2;
        }
        
        private void greyscale()
        {
            Bitmap img1;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            var img2 = new Bitmap(img1.Width, img1.Height);
            for (int x = 0; x < img1.Width; x++)
            {
                for (int y = 0; y < img1.Height; y++)
                {
                    Color c = img1.GetPixel(x, y);
                    int value = (c.R + c.G + c.B) / 3;
                    Color greyscale = Color.FromArgb(value, value, value);
                    img2.SetPixel(x, y, greyscale);
                    


                }
                
            }

            pictureBox3.Image = img2;
            
        }
        

        private void loadImage1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (camera != null)
            camera.Stop();
            
            IDataObject data;
         
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Bitmap map = new Bitmap(dlg.FileName);
                pictureBox1.Image = map;
            }

            start();

        }

        private void loadIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Bitmap map = new Bitmap(dlg.FileName);
                pictureBox2.Image = map;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
     
        }

        private void label4_Click(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Bitmap img1 = null;
            
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            
            if (pictureBox1.Image == null) return;
            var rpx  = pictureBox1.PointToClient(Cursor.Position);
            float xRatio = (float)pictureBox1.Image.Width / pictureBox1.Width;
            float yRatio = (float)pictureBox1.Image.Height / pictureBox1.Height;
            int clickedX = (int)(rpx.X * xRatio);
            int clickedY = (int)(rpx.Y * yRatio); 
            Color c = ((Bitmap)img1).GetPixel(clickedX, clickedY);
            pictureBox4.BackColor = c;
            _screenColor = c;
        }

        private Color _screenColor = DefaultBackColor;

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap img1 = null;
            
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } else img1 = (Bitmap)pictureBox1.Image;
            
            if (pictureBox1.Image == null) return;
            var rpx  = pictureBox1.PointToClient(Cursor.Position);
            float xRatio = (float)pictureBox1.Image.Width / pictureBox1.Width;
            float yRatio = (float)pictureBox1.Image.Height / pictureBox1.Height;
            int clickedX = (int)(rpx.X * xRatio);
            int clickedY = (int)(rpx.Y * yRatio); 
            Color c = ((Bitmap)img1).GetPixel(clickedX, clickedY);
            pictureBox4.BackColor = c;
            _screenColor = c;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Click on foreground image to modify the screen color.\n" +
                            "Click on 'Image Mode' to change to camera mode.\n" +
                            "Click on 'Camera Mode' to change to image mode.\n" +
                            "To select the green screen color, you can click on the foreground image.\n" +
                            "However, if you are on camera mode, you can click on automatic color capture to automatically select the green screen color.\n" +
                            "In this case, after automatic color capture, you will need to click on the 'Process Image' button to reflect the changes.");
        }

        private bool UnstrictMatch(Color c)
        {
            // bypass dominance checks
            bool dGreen, dBlue, dRed;
            dRed = dGreen = dBlue = false;
            if (c.R > c.G)
            {
                if (c.R > c.B) dRed = true;
                else dBlue = true;
            }
            else
            {
                if (c.G > c.B) dGreen = true;
                else dBlue = true;
            }
            
            int strictness_policy = 110;
            int dominance_policy = 255;

            if (dBlue)
            {
                if (c.R > _screenColor.R + strictness_policy || c.R < _screenColor.R - strictness_policy) return false;
                if (c.G > _screenColor.G + strictness_policy || c.G < _screenColor.G - strictness_policy) return false;
                if (c.B > _screenColor.B + dominance_policy || c.B < _screenColor.B - dominance_policy) return false;
            } else if (dRed)
            {
                if (c.R > _screenColor.R + dominance_policy || c.R < _screenColor.R - dominance_policy) return false;
                if (c.G > _screenColor.G + strictness_policy || c.G < _screenColor.G - strictness_policy) return false;
                if (c.B > _screenColor.B + strictness_policy || c.B < _screenColor.B - strictness_policy) return false;
            }
            else
            {
                if (c.R > _screenColor.R + strictness_policy || c.R < _screenColor.R - strictness_policy) return false;
                if (c.G > _screenColor.G + dominance_policy || c.G < _screenColor.G - dominance_policy) return false;
                if (c.B > _screenColor.B + strictness_policy || c.B < _screenColor.B - strictness_policy) return false;
            }
            
           
            return true;
        }
        private Bitmap StretchImage(Bitmap originalImage, int desiredWidth, int desiredHeight)
        {
            // Create a new Bitmap with the desired size
            Bitmap stretchedImage = new Bitmap(desiredWidth, desiredHeight);

            // Create a Graphics object from the new Bitmap
            using (Graphics g = Graphics.FromImage(stretchedImage))
            {
                // Draw the original image onto the new Bitmap, stretching it to fit
                g.DrawImage(originalImage, 0, 0, desiredWidth, desiredHeight);
            }

            return stretchedImage;
        }
    

        private void subtract()
        {
            Bitmap img1 = (Bitmap)pictureBox1.Image;
            ;
            if (cameraMode)
            {
                
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            }  
            if (pictureBox2.Image == null || img1 == null)
            {
                MessageBox.Show("Load images properly.");
                currMode = Mode.COPY;
                return;
            }
            Bitmap source = (Bitmap)pictureBox1.Image;
            

            source = img1;
            
            Bitmap generated = new Bitmap(source.Width, source.Height);
            Bitmap background = StretchImage((Bitmap)pictureBox2.Image, source.Width, source.Height);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color curr = source.GetPixel(x, y);
                    if (!UnstrictMatch(curr))
                    {
                        generated.SetPixel(x, y, curr);
                    }
                    else
                    {
                        generated.SetPixel(x, y, background.GetPixel(x, y));
                    }
                }
            }

            pictureBox3.Image = generated;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            subtract();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            subtract();
        }

        private Device camera;
        private PictureBox storedPictureBox;
        


        private void button3_Click(object sender, EventArgs e)
        {
            
            if (button3.Text == "Camera Mode")
            {
                cameraMode = false;
                camera.Stop();
                pictureBox1 = storedPictureBox;
                button3.Text = "Image Mode";
            }
            else
            {
                cameraMode = true;
                button3.Text = "Camera Mode";
                camera = DeviceManager.GetAllDevices()[0];
                storedPictureBox = pictureBox1;
                camera.ShowWindow(pictureBox1);
            }
            start();
        }

        private void basicCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.COPY;
        }

        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.GREYSCALE;
        }

        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.INVERT;
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.SEPIA;
        }

        private void subtractionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.SUBTRACTION;
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currMode = Mode.HISTOGRAM;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap img1;
            if (cameraMode)
            {
                camera.Sendmessage();
                var data = Clipboard.GetDataObject();
                img1 = new Bitmap((Image)data.GetData("System.Drawing.Bitmap", true));
            } 
            else 
            {
                img1 = (Bitmap)pictureBox1.Image;
            }

            if (img1 == null) return;

            Dictionary<Color, int> colorCount = new Dictionary<Color, int>();

            // Analyze each pixel
            for (int x = 0; x < img1.Width; x++)
            {
                for (int y = 0; y < img1.Height; y++)
                {
                    Color pixelColor = img1.GetPixel(x, y);

                    if (colorCount.ContainsKey(pixelColor))
                    {
                        colorCount[pixelColor]++;
                    }
                    else
                    {
                        colorCount[pixelColor] = 1;
                    }
                }

                
            }

            // Find the most dominant color
            Color dominantColor = colorCount.OrderByDescending(kvp => kvp.Value).First().Key;

            // Display the result
            pictureBox4.BackColor = dominantColor;
            MessageBox.Show("Automatic Color Capture Done.")
                ;
        }

// Convert hue to Color
        private Color ColorFromHue(float hue)
        {
            // Hue to RGB conversion
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            byte v = 255;
            byte p = Convert.ToByte(v * (1 - 1));
            byte q = Convert.ToByte(v * (1 - f));
            byte t = Convert.ToByte(v * (1 - (1 - f)));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "jpg";
            sfd.AddExtension = true;
            sfd.Filter = "Image (*.jpg) | ";
            if (sfd.ShowDialog() != DialogResult.OK) return;
            pictureBox3.Image.Save(sfd.FileName);
            MessageBox.Show("Image file saved!");
        }
    }
}