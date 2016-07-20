using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace image
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Image file;
        Image cropFile;
        //Bitmap bmp;
        MemoryStream ms = new MemoryStream();
        ImageService.ImageServiceClient client = new ImageService.ImageServiceClient();
        //ImgService.ImageServiceClient client = new ImgService.ImageServiceClient();


        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                file = Image.FromFile(openFileDialog1.FileName);
                //bmp = new Bitmap(file, 600, 600);
                pictureBox1.Image = file; 
            }
        }



        //private void button2_Click(object sender, EventArgs e)
        //{
        //    DialogResult result = saveFileDialog1.ShowDialog();

        //    if (result == DialogResult.OK)
        //    {
        //        if (file != null)
        //        {
        //            if (saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.Length - 3).ToLower() == "jpg")
        //            {
        //                file.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
        //                //bmp.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
        //            }
        //        }
        //    }
        //}

        private void button3_Click(object sender, EventArgs e)
        {
            file.Save(ms, ImageFormat.Jpeg); // сохраняем в поток памяти, типо в оперативу фотку ложим, в байтах.
            //bmp.Save(ms, ImageFormat.Jpeg);
            byte[] arr = ms.ToArray(); // - это массив байтов. В байтах можно что угодно передовать.
            // c48VT0UCvUmfJL3VkXHB4A - токен доступа для id = 4
            // NameOfFile - сюда изначальное имя файла. Хотя на серве имя генериться другое
            // стоит ли в базу писать изначальное имя фотки ??

            MessageBox.Show(client.UploadImage(arr, "NameOfFile", 4, "c48VT0UCvUmfJL3VkXHB4A==").ToString());
        }

        private bool _selecting;
        private Rectangle _selection;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Starting point of the selection:
            if (e.Button == MouseButtons.Left)
            {
                _selecting = true;
                _selection = new Rectangle(new Point(e.X, e.Y), new Size());
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Update the actual size of the selection:
            if (_selecting)
            {
                _selection.Width = e.X - _selection.X;
                _selection.Height = e.Y - _selection.Y;

                // Redraw the picturebox:
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_selecting)
            {
                // Draw a rectangle displaying the current selection
                Pen pen = Pens.GreenYellow;
                e.Graphics.DrawRectangle(pen, _selection);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _selecting = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pictureBox1.Image = Crop(pictureBox1.Image, _selection);
            cropFile = pictureBox1.Image;
            //действия с cropFile по пересылке на серв
            //можно локально сохранять все фотки пользователя, его авы.
            //что бы не грузить лишний раз. Когда удалит - удаляем с телефона.
            //Если не со своего устройства, проверку делать. Нету на телефоне - грузим с инета
        }

        private Image Crop(Image image, Rectangle selection)
        {
            Bitmap bmp = image as Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("No valid bitmap");

            // Crop the image:
            Bitmap cropBmp = bmp.Clone(selection, bmp.PixelFormat);

            // Release the resources:
            image.Dispose();

            return cropBmp;
        }
    }
}
