using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundAnalyzer
{
    class WaveReader
    {
        static int endPointY = 0;

        /// <summary>
        /// Opening the file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="L"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static bool readWav(string filename, out float[] L, out float[] R)
        {
            L = R = null;
            //float [] left = new float[1];

            //float [] right;
            try
            {
                using (FileStream fs = File.Open(filename, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    // chunk 0
                    int chunkID = reader.ReadInt32();
                    int fileSize = reader.ReadInt32();
                    int riffType = reader.ReadInt32();

                    // chunk 1
                    int fmtID = reader.ReadInt32();
                    int fmtSize = reader.ReadInt32(); // bytes for this chunk
                    int fmtCode = reader.ReadInt16();
                    int channels = reader.ReadInt16();
                    int sampleRate = reader.ReadInt32();
                    int byteRate = reader.ReadInt32();
                    int fmtBlockAlign = reader.ReadInt16();
                    int bitDepth = reader.ReadInt16();

                    Console.WriteLine("Chunk ID: " + chunkID);
                    Console.WriteLine("fileSize: " + fileSize);
                    Console.WriteLine("riffType: " + riffType);
                    Console.WriteLine("fmtID: " + fmtID);
                    Console.WriteLine("fmtSize: " + fmtSize);
                    Console.WriteLine("fmtCode: " + fmtCode);
                    Console.WriteLine("channels: " + channels);
                    Console.WriteLine("sampleRate: " + sampleRate);
                    Console.WriteLine("byteRate: " + byteRate);
                    Console.WriteLine("fmtBlockAlign: " + fmtBlockAlign);
                    Console.WriteLine("bitDepth: " + bitDepth);

                    if (fmtSize == 18)

                    {
                        // Read any extra values
                        int fmtExtraSize = reader.ReadInt16();
                        reader.ReadBytes(fmtExtraSize);
                    }

                    // chunk 2
                    int dataID = reader.ReadInt32();
                    int bytes = reader.ReadInt32();

                    // DATA!
                    byte[] byteArray = reader.ReadBytes(bytes);

                    int bytesForSamp = bitDepth / 8;
                    int samps = bytes / bytesForSamp;


                    float[] asFloat = null;
                    switch (bitDepth)
                    {
                        case 64:
                            double[]
                            asDouble = new double[samps];
                            Buffer.BlockCopy(byteArray, 0, asDouble, 0, bytes);
                            asFloat = Array.ConvertAll(asDouble, e => (float)e);
                            break;
                        case 32:
                            asFloat = new float[samps];
                            Buffer.BlockCopy(byteArray, 0, asFloat, 0, bytes);
                            break;
                        case 16:
                            Int16[]
                            asInt16 = new Int16[samps];
                            Buffer.BlockCopy(byteArray, 0, asInt16, 0, bytes);
                            asFloat = Array.ConvertAll(asInt16, e => e / (float)Int16.MaxValue);
                            break;
                        default:
                            return false;
                    }

                    switch (channels)
                    {
                        case 1:
                            L = asFloat;
                            R = asFloat;
                            return true;
                        case 2:
                            L = new float[samps / 2];
                            R = new float[samps / 2];
                            for (int i = 0, s = 0; i < samps / 2; i++)
                            {
                                L[i] = asFloat[s++];
                                R[i] = asFloat[s++];
                            }
                            return true;
                        default:
                            return false;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("...Failed to load note: " + filename);
                return false;
                //left = new float[ 1 ]{ 0f };
            }
        }

        /// <summary>
        /// Drawing the chart with left array on the main Chart1.
        /// </summary>
        /// <param name="left">wave file into a float array</param>
        public static void FillChart(float[] left, Chart chart1)
        {
            int blockSize = 3000;

            // clear the chart
            chart1.Series.Clear();

            // fill the chart
            var series = chart1.Series.Add("My Series");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Int32;
            for (int i = 0; i < left.Length; i++)
                series.Points.AddXY(i, left[i]);
            var chartArea = chart1.ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = left.Length;
            double test = chartArea.AxisY.Maximum;
            endPointY = (int)chartArea.AxisY.Maximum;

            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            int position = 0;
            int size = blockSize;
            chartArea.AxisX.ScaleView.Zoom(position, size);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change to blockSize (e.g. 100)
            chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;

            chart1.ChartAreas["ChartArea1"].CursorX.IsUserEnabled = true; //creates a line when user clicks
            chart1.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true; //scrolls the scrollbar outside
            chart1.ChartAreas["ChartArea1"].AxisX.ScrollBar.IsPositionedInside = true;
        }

        /// <summary>
        /// Drawing the chart with copied points on chart2.
        /// </summary>
        /// <param name="left"></param>
        public static void FillChartDouble(double[] left, Chart chart)
        {
            int blockSize = 1000;

            // clear the chart
            chart.Series.Clear();

            // fill the chart
            var series = chart.Series.Add("My Series");
            series.ChartType = SeriesChartType.Column;
            series.XValueType = ChartValueType.Int32;
            for (int i = 0; i < left.Length; i++)
                series.Points.AddXY(i, left[i]);
            var chartArea = chart.ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = left.Length;

            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            int position = 0;
            int size = blockSize;
            chartArea.AxisX.ScaleView.Zoom(position, size);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change to blockSize (e.g. 100)
            chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;

            chart.ChartAreas["ChartArea1"].CursorX.IsUserEnabled = true; //creates a line when user clicks
            chart.ChartAreas["ChartArea1"].CursorX.IsUserSelectionEnabled = true; //scrolls the scrollbar outside
            chart.ChartAreas["ChartArea1"].AxisX.ScrollBar.IsPositionedInside = true;
        }
    }
}
