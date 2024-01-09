using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kontrasta_izlabosana
{
    public class PixelRGB
    {
        public byte R;
        public byte G;
        public byte B;
        public byte I;

        public PixelRGB()
        {
            R = 0;
            G = 0;
            B = 0;
            I = 0;
        }

        public PixelRGB(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            I = (byte)Math.Round(0.073f * b + 0.715 * g + 0.212f * r);
        }

        public PixelRGB hsvToRGB(int h, byte s, byte v)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;

            int Hi = Convert.ToInt32(h / 60);
            byte Vmin = Convert.ToByte((255 - s) * v / 255);
            int a = Convert.ToInt32((v - Vmin) * (h % 60) / 60);
            byte Vinc = Convert.ToByte(Vmin + a);
            byte Vdec = Convert.ToByte(v - a);

            switch (Hi)
            {
                case 0: { r = v; g = Vinc; b = Vmin; break; }
                case 1: { r = Vdec; g = v; b = Vmin; break; }
                case 2: { r = Vmin; g = v; b = Vinc; break; }
                case 3: { r = Vmin; g = Vdec; b = v; break; }
                case 4: { r = Vinc; g = Vmin; b = v; break; }
                case 5: { r = v; g = Vmin; b = Vdec; break; }
            }

            PixelRGB rgbPix = new PixelRGB(r, g, b);
            return rgbPix;
        }

        //public PixelRGB cmykToRGB(c, m, y, k)
        public PixelRGB cmykToRGB(double c, double m, double y, double k)
        {
            byte r = (byte)(255 * (1 - c / 100) * (1 - k / 100));
            byte g = (byte)(255 * (1 - m / 100) * (1 - k / 100));
            byte b = (byte)(255 * (1 - y / 100) * (1 - k / 100));

            PixelRGB rgbPix = new PixelRGB(r, g, b);
            return rgbPix;
        }

        //public PixelRGB yuvToRGB(y, u, v)
        public PixelRGB yuvToRGB(double y1, double u, double v1)
        {
            double r = (y1 + 1.13983 * (v1 - 128));
            double g = (y1 - 0.39465 * (u - 128) - 0.5860 * (v1 - 128));
            double b = (y1 + 2.03211 * (u - 128));

            PixelRGB rgbPix = new PixelRGB((byte)r, (byte)g, (byte)b);
            return rgbPix;
        }

    }
}
