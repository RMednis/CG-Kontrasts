using kontrasta_izlabosana;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace kontrasta_izlabosana
{
    public class HistogrammClass
    {
        //for RGB
        public int[] hR;
        public int[] hG;
        public int[] hB;
        public int[] hI;

        public HistogrammClass()
        {
            //for RGB
            hR = new int[256];
            hI = new int[256];
            hG = new int[256];
            hB = new int[256];
        }

        //function to clear the histogram
        public void eraseHistogramm()
        {
            //reset the histograms for each of the 256 intensity levels
            for (int i = 0; i < 256; i++)
            {
                hR[i] = 0;
                hI[i] = 0;
                hG[i] = 0;
                hB[i] = 0;
            }
        }

        public void eraseHistogrammfull()
        {
            //reset the histograms for each of the 256 intensity levels
            for (int i = 0; i < 256; i++)
            {
                hR[i] = 0;
                hI[i] = 0;
                hG[i] = 0;
                hB[i] = 0;
            }
        }

        public void readHistogram(PixelRGB[,] img)
        {
            eraseHistogramm();
            for (int x = 0; x < img.GetLength(0); x++)
            {
                for (int y = 0; y < img.GetLength(1); y++)
                {
                    hR[img[x, y].R]++;
                    hI[img[x, y].I]++;
                    hG[img[x, y].G]++;
                    hB[img[x, y].B]++;
                }
            }
        }

        //function to draw a histogram on a given Chart control
        public void drawHistogramm(Chart chart)
        {
            //clearing all existing series in Chart
            chart.Series.Clear();

            //clearing all existing chart areas and adding a new "ChartArea"
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add("ChartArea");

            //setting the minimum and maximum values for the x
            chart.ChartAreas["ChartArea"].AxisX.Minimum = 0;
            chart.ChartAreas["ChartArea"].AxisX.Maximum = 255;

            //adding series for each color channel (R, G, B, I) indicating colors
            chart.Series.Add("R");
            chart.Series["R"].Color = Color.Red;
            chart.Series.Add("G");
            chart.Series["G"].Color = Color.Green;
            chart.Series.Add("B");
            chart.Series["B"].Color = Color.Blue;
            chart.Series.Add("I");
            chart.Series["I"].Color = Color.Black;

            //filling the histogram data for each intensity level
            for (int i = 0; i < 256; i++)
            {
                chart.Series["R"].Points.AddXY(i, hR[i]);
                chart.Series["G"].Points.AddXY(i, hG[i]);
                chart.Series["B"].Points.AddXY(i, hB[i]);
                chart.Series["I"].Points.AddXY(i, hI[i]);
            }
        }

        //function for drawing selected color channels of a histogram
        public void drawHistogramm(Chart chart, string color)
        {
            //clearing all existing series
            chart.Series.Clear();
            //clearing all existing chart areas and adding a new "ChartArea"
            chart.ChartAreas.Clear();
            chart.ChartAreas.Add("ChartArea");

            //setting the minimum and maximum values for the x
            chart.ChartAreas["ChartArea"].AxisX.Minimum = 0;
            chart.ChartAreas["ChartArea"].AxisX.Maximum = 255;

            //adding series for selected color channels
            if (color.Contains("R"))
            {
                chart.Series.Add("R");
                chart.Series["R"].Color = Color.Red;
            }
            if (color.Contains("G"))
            {
                chart.Series.Add("G");
                chart.Series["G"].Color = Color.Green;
            }
            if (color.Contains("B"))
            {
                chart.Series.Add("B");
                chart.Series["B"].Color = Color.Blue;
            }
            if (color.Contains("RGB") || color == "I")
            {
                chart.Series.Add("I");
                chart.Series["I"].Color = Color.Black;
            }

            //populate histogram data for selected color channels
            for (int i = 0; i < 256; i++)
            {
                if (color.Contains("R")) chart.Series["R"].Points.AddXY(i, hR[i]);
                if (color.Contains("G")) chart.Series["G"].Points.AddXY(i, hG[i]);
                if (color.Contains("B")) chart.Series["B"].Points.AddXY(i, hB[i]);
                if (color.Contains("I") || color.Contains("RGB")) chart.Series["I"].Points.AddXY(i, hI[i]);
            }
        }
    }
}
