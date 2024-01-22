using kontrasta_izlabosana;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace kontrasta_izlabosana
{
    public class ImageClass
    {
        //arrays for storing pixels in different color models
        public PixelRGB[,] imgOriginal;
        public PixelRGB[,] imgCustom;

        public HistogrammClass hstOriginal;
        public HistogrammClass hstCustom;
        public ImageClass()
        {
            hstOriginal = new HistogrammClass();
            hstCustom = new HistogrammClass();
        }

        public void ReadImage(Bitmap bmp)
        {
            //initializing arrays to store pixels in different color models
            imgOriginal = new PixelRGB[bmp.Width, bmp.Height];
            imgCustom = new PixelRGB[bmp.Width, bmp.Height];

            //receive image data and lock it
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr ptr = IntPtr.Zero;
            int pixelComponents;

            //determining the number of color components in an image
            if (bmpData.PixelFormat == PixelFormat.Format24bppRgb) pixelComponents = 3;
            else if (bmpData.PixelFormat == PixelFormat.Format32bppArgb) pixelComponents = 4;
            else pixelComponents = 0;

            var row = new byte[bmp.Width * pixelComponents];

            for (int y = 0; y < bmp.Height; y++)
            {
                ptr = bmpData.Scan0 + y * bmpData.Stride; //stride - skenesanas platums
                Marshal.Copy(ptr, row, 0, row.Length);

                for (int x = 0; x < bmp.Width; x++)
                {
                    //filling pixel arrays in different color models
                    imgOriginal[x, y] = new PixelRGB(row[pixelComponents * x + 2], row[pixelComponents * x + 1], row[pixelComponents * x]);

                    imgCustom[x, y] = new PixelRGB(row[pixelComponents * x + 2], row[pixelComponents * x + 1], row[pixelComponents * x]);
                }
            }
            bmp.UnlockBits(bmpData);//unlocking image data

            hstOriginal.readHistogram(imgOriginal);
            hstCustom.readHistogram(imgCustom);
        }

        public void RefillArraysFillHistogram(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            IntPtr ptr = IntPtr.Zero;
            int pixelComponents;

            //determining the number of color components in an image
            if (bmpData.PixelFormat == PixelFormat.Format24bppRgb) pixelComponents = 3;
            else if (bmpData.PixelFormat == PixelFormat.Format32bppArgb) pixelComponents = 4;
            else pixelComponents = 0;

            var row = new byte[bmp.Width * pixelComponents];

            for (int y = 0; y < bmp.Height; y++)
            {
                ptr = bmpData.Scan0 + y * bmpData.Stride; //stride - skenesanas platums
                Marshal.Copy(ptr, row, 0, row.Length);

                for (int x = 0; x < bmp.Width; x++)
                {
                    //filling pixel arrays in different color models
                    //imgOriginal[x, y] = new PixelRGB(row[pixelComponents * x + 2], row[pixelComponents * x + 1], row[pixelComponents * x]);

                    imgCustom[x, y] = new PixelRGB(row[pixelComponents * x + 2], row[pixelComponents * x + 1], row[pixelComponents * x]);
                }
            }
            bmp.UnlockBits(bmpData);
            hstCustom.readHistogram(imgCustom);
        }

        public void EnhanceLocalContrast(float k)//dima
        {

            int windowSize = 3;
            int border = windowSize / 2;
            int width = imgOriginal.GetLength(0);
            int height = imgOriginal.GetLength(1);


            PixelRGB[,] tempImg = new PixelRGB[width, height];// Create a temporary array to store contrast-enhanced pixels

            for (int x = border; x < width - border; x++)
            {
                for (int y = border; y < height - border; y++)
                {
                    float sum = 0f;
                    float sumSq = 0f;
                    int count = 0;

                    for (int i = -border; i <= border; i++)
                    {
                        for (int j = -border; j <= border; j++)
                        {

                            sum += imgOriginal[x + i, y + j].I; // Summarizing pixel intensity
                            sumSq += imgOriginal[x + i, y + j].I * imgOriginal[x + i, y + j].I; // Summation of intensity squares
                            count++;// Increasing the pixel counter

                        }
                    }
                    float mean = sum / count;// Calculation of average intensity in a "window"

                    float std = (float)Math.Sqrt((sumSq / count) - (mean * mean));// Getting the current pixel from the original image



                    PixelRGB originalPixel = imgOriginal[x, y];
                    // Creating a new pixel with improved intensity
                    tempImg[x, y] = new PixelRGB(
                        ClampToByte(mean + k * (originalPixel.R - mean)),
                        ClampToByte(mean + k * (originalPixel.G - mean)),
                        ClampToByte(mean + k * (originalPixel.B - mean))
                    );
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    imgCustom[x, y] = tempImg[x, y] ?? imgOriginal[x, y];
                }
            }

            hstOriginal.readHistogram(imgOriginal);
            hstCustom.readHistogram(imgCustom);
        }
        private byte ClampToByte(float value)// diapazon kontrol.
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }

        public Bitmap DrawImage(PixelRGB[,] img, string mode)
        {
            IntPtr ptr = IntPtr.Zero;

            var bmp = new Bitmap(img.GetLength(0), img.GetLength(1), PixelFormat.Format24bppRgb);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);

            var row = new byte[bmp.Width * 3];

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    switch (mode)
                    {
                        case "RGB":
                            {
                                row[3 * x + 2] = img[x, y].R;
                                row[3 * x + 1] = img[x, y].G;
                                row[3 * x] = img[x, y].B;
                                break;
                            }
                        case "R":
                            {
                                row[3 * x + 2] = img[x, y].R;
                                row[3 * x + 1] = 0;
                                row[3 * x] = 0;
                                break;
                            }
                        case "G":
                            {
                                row[3 * x + 2] = 0;
                                row[3 * x + 1] = img[x, y].G;
                                row[3 * x] = 0;
                                break;
                            }
                        case "B":
                            {
                                row[3 * x + 2] = 0;
                                row[3 * x + 1] = 0;
                                row[3 * x] = img[x, y].B;
                                break;
                            }
                        case "I":
                            {
                                row[3 * x + 2] = img[x, y].I;
                                row[3 * x + 1] = img[x, y].I;
                                row[3 * x] = img[x, y].I;
                                break;
                            }
                        
                    }
                }
                ptr = bmpData.Scan0 + y * bmpData.Stride;
                Marshal.Copy(row, 0, ptr, row.Length);
            }
            bmp.UnlockBits(bmpData);
            return bmp;
        }
    }
}