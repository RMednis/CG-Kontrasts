using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Kontrasta uzlabošana ar Sigmoid funkciju
// Reinis Gunārs Mednis
// Avots: https://www.imagemagick.org/Usage/color_mods/#sigmoidal
// Otrs avots: https://farid.berkeley.edu/downloads/tutorials/fip.pdf - 44 lpp 

namespace kontrasta_izlabosana
{
    public class SigmoidContrast
    {
        public static Bitmap SigmoidContrastAdjust(Bitmap originalImage, int alpha, int beta)
        {
            Bitmap adjustedImage = new Bitmap(originalImage.Width, originalImage.Height);

            double alpha_perc = alpha / 100; // Pārveidojam alfa vērtību no procentiem uz 0-1

            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    // Iegūstam krāsu no oriģinālā attēla
                    Color originalColor = originalImage.GetPixel(x, y);

                    // Apstrādājam katru krāsu komponenti atsevišķi, izmantojot Sigmoid funkciju
                    int red = TransformColor(originalColor.R, alpha_perc, beta);
                    int green = TransformColor(originalColor.G, alpha_perc, beta);
                    int blue = TransformColor(originalColor.B, alpha_perc, beta);

                    // Pārveidojam krāsu atpakaļ uz Color objektu
                    Color newColor = Color.FromArgb(red, green, blue);

                    // Uzstādām jauno krāsas vērtību kā attēla pikseli
                    adjustedImage.SetPixel(x, y, newColor);

                }
            }

            return adjustedImage;
        }

        private static int TransformColor(int colorComponent, double alpha, double beta)
        {
            // Pārveidojam krāsu no 0-255 uz 0-1
            double u = colorComponent / 255.0;
            beta = beta * -1;

            double result = 0;

            if (alpha == 0)
            {
                alpha = double.Epsilon;
            }

            if (beta == 0)
            {                 
                beta = double.Epsilon;
            }
            

            result = (beta * alpha - Math.Log((1 / ((u / (1 + Math.Exp(beta * alpha - beta))) - (u / (1 + Math.Exp(beta * alpha))) + (1 / (1 + Math.Exp(beta * alpha))))) - 1)) / beta;
            
            // Pārveidojam krāsu atpakaļ uz 0-255
            return (int)Math.Max(Math.Min(result * 255, 255), 0);
        }


    }
}
