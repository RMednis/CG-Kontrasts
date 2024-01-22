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

namespace kontrasta_izlabosana
{
    public partial class Form1 : Form
    {
        //field for storing the current color format
        private string currentColorFormat = "RGB";

        private float contrastFactor = 1.0f;
        private bool isContrastAdjustmentEnabled = false;
        private int initialContrastValue = 0;

        public ImageClass imageClass = new ImageClass();
        public Form1()
        {
            InitializeComponent();
            radioButton1.Enabled = false;
            radioButton2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            trackBar1.Enabled = false;

            int middleValue = 0;
            trackBar1.Value = middleValue;
            initialContrastValue = middleValue;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
                imageClass.ReadImage(bmp); //reading an image and loading it into the ImageClass
                pictureBox2.Image = imageClass.DrawImage(imageClass.imgOriginal, "RGB"); //displaying the original image in PictureBox2
                imageClass.hstCustom.drawHistogramm(chart1);

                imageClass.RefillArraysFillHistogram((Bitmap)pictureBox2.Image);
                imageClass.hstCustom.drawHistogramm(chart2);
                imageClass.RefillArraysFillHistogram((Bitmap)pictureBox1.Image);

                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
            }
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

        private void buttonReset_Click(object sender, EventArgs e)
        {
            pictureBox2.Image = imageClass.DrawImage(imageClass.imgOriginal, "RGB");

            //setting radioButtons inactive
            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            //setting track bars to default value
            trackBar1.Value = 0;
            trackBar_contrastfactor.Value = trackBar_contrastfactor.Maximum / 2;
            trackBar_contrastthreshold.Value = trackBar_contrastthreshold.Maximum / 2;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            isContrastAdjustmentEnabled = radioButton3.Checked;

            if (isContrastAdjustmentEnabled)
            {
                trackBar1.Value = initialContrastValue;
                trackBar1.Enabled = true;
                trackBar1.ValueChanged += TrackBar1_ValueChanged;
                trackBar1.Minimum = -128;
                trackBar1.Maximum = 128;
                trackBar1.TickFrequency = 5;
            }
            else
            {
                trackBar1.Enabled = false;
                trackBar1.ValueChanged -= TrackBar1_ValueChanged;
            }
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            int contrastValue = trackBar1.Value;
            contrastFactor = CalculateContrastFactor(contrastValue);
            UpdateContrastImageAndHistogram();
        }

        private float CalculateContrastFactor(int contrast)
        {
            return (259 * (contrast + 255f)) / (255 * (259 - contrast));
        }

        private void UpdateContrastImageAndHistogram()
        {
            UpdateContrastImage();
            imageClass.RefillArraysFillHistogram((Bitmap)pictureBox2.Image);
            imageClass.hstCustom.drawHistogramm(chart2, "RGB");
        }

        private void UpdateContrastImage()
        {
            if (pictureBox1.Image != null && isContrastAdjustmentEnabled)
            {
                Bitmap originalImage = new Bitmap(pictureBox1.Image);
                Bitmap contrastImage = AdjustContrast(originalImage);
                pictureBox2.Image = contrastImage;
            }
        }

        //Third contrast correction method
        //Ksenia Danilets(deadunicorn2029 and Lithadian(my computer in a service, so im working from other person computer))
        private Bitmap AdjustContrast(Bitmap originalImage)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    Color originalColor = originalImage.GetPixel(x, y);

                    int newRed = Truncate(contrastFactor * (originalColor.R - 128) + 128);
                    int newGreen = Truncate(contrastFactor * (originalColor.G - 128) + 128);
                    int newBlue = Truncate(contrastFactor * (originalColor.B - 128) + 128);

                    Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    adjustedImage.SetPixel(x, y, newColor);
                }
            }

            return adjustedImage;
        }

        private int Truncate(float value)
        {
            return Math.Max(0, Math.Min(255, (int)value));
        }


        // Kontrasta uzlabošana ar Sigmoid funkciju
        // Reinis Gunārs Mednis

        private int alpha = 50;
        private int beta = 1;

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                trackBar_contrastfactor.Enabled = true;
                trackBar_contrastthreshold.Enabled = true;
            }
            else
            {
                trackBar_contrastfactor.Enabled = false;
                trackBar_contrastthreshold.Enabled = false;
            }
        }

        private void trackBar_contrastthreshold_ValueChanged(object sender, EventArgs e)
        {
            alpha = trackBar_contrastthreshold.Value;
            label_contrastthreshold.Text = "Contrast Threshold:" + alpha.ToString();
            updateSigmoidContrastImage();

        }

        private void trackBar_contrastfactor_ValueChanged(object sender, EventArgs e)
        {
            beta = trackBar_contrastfactor.Value;
            label_contrasstfactor.Text = "Contrast Factor:" + beta.ToString();
            updateSigmoidContrastImage();
        }

        private void updateSigmoidContrastImage()
        {
            if (pictureBox1.Image != null)
            {
                Bitmap originalImage = new Bitmap(pictureBox1.Image);
                Bitmap bitmap = SigmoidContrast.SigmoidContrastAdjust(originalImage, alpha, beta);
                
                pictureBox2.Image = bitmap;

                imageClass.RefillArraysFillHistogram((Bitmap)pictureBox2.Image);
                imageClass.hstCustom.drawHistogramm(chart2, "RGB");
            }
        }
    }
}