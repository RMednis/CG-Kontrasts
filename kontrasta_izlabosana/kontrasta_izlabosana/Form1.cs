using kontrasta_izlabosana;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace kontrasta_izlabosana
{
    public partial class Form1 : Form
    {
        //field for storing the current color format
        private string currentColorFormat = "RGB";

        public ImageClass imageClass = new ImageClass();
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPEG Image|*.jpg";
            //setting a filter to save an image in JPEG format
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string savePath = saveFileDialog1.FileName;

                //check that the image in pictureBox2 exists
                if (pictureBox2.Image != null)
                {
                    //save the image in the selected folder in JPEG format
                    pictureBox2.Image.Save(savePath, ImageFormat.Jpeg);
                }
                else
                {
                    MessageBox.Show("no image to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
                imageClass.ReadImage(bmp); //reading an image and loading it into the ImageClass
                pictureBox2.Image = imageClass.DrawImage(imageClass.imgOriginal, "RGB"); //displaying the original image in PictureBox2
            }
        }

        private Point ConvertXY(int x, int y, PictureBox pb)
        {
            Point p = new Point();

            //converting mouse position to X and Y coordinates within PictureBox1
            double kx = (double)pb.Image.Width / pb.Width;
            double ky = (double)pb.Image.Height / pb.Height;

            double k = Math.Max(kx, ky);

            double nobideX = (pb.Width * k - pb.Image.Width) / 2f;
            double nobideY = (pb.Height * k - pb.Image.Height) / 2f;

            p.X = Math.Min(pb.Image.Width, Math.Max(0, (int)(x * k - nobideX)));
            p.Y = Math.Min(pb.Image.Height, Math.Max(0, (int)(y * k - nobideY)));

            return p;
        }


        private void buttonReset_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = imageClass.DrawImage(imageClass.imgOriginal, "RGB");
        }
    }
}