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

            //hstCustom.readHistogramHSV(imgHSV);
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