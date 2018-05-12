using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundAnalyzer
{
    class Fourier
    {
        private static double[] A;
        private static int masterInd = 0;
        static private object threadLock = new object();

        /// <summary>
        /// Discrete fourier.
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public static double[] DFT(double[] S)
        {
            A = new double[S.Length];
            double[] tempreal = new double[S.Length];
            double[] tempimag = new double[S.Length];
            double N = S.Length, real, imag;
            masterInd = 0;
            int f = masterInd;

            while (f <= N - 1)
            {
                lock (threadLock)
                    if (masterInd <= N - 1)
                        f = masterInd++;
                    else break;
                real = 0;
                imag = 0;
                for (int t = 0; t <= N - 1; t++)
                {
                    real += S[t] * Math.Cos(2 * Math.PI * t * f / N);
                    imag -= S[t] * Math.Sin(2 * Math.PI * t * f / N);
                    tempreal[t] = real;
                    tempimag[t] = imag;
                }
                A[f] = Math.Sqrt(real * real + imag * imag) / N;
            }
            return A;
        }

        /// <summary>
        /// Inverse fourier.
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public static double[] IDFT(double[] A)
        {
            double[] S = new double[A.Length];
            double N = A.Length, real, imag;

            for (int t = 0; t < N; t++)
            {
                real = 0;
                imag = 0;
                for (int f = 0; f < N; f++)
                {
                    real += A[f] * Math.Cos(2 * Math.PI * t * f / N);
                    imag += A[f] * Math.Sin(2 * Math.PI * t * f / N);
                }
                S[t] = (real - imag) / N;
            }
            return S;
        }

        /// <summary>
        /// Windowing.
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double[] Windowing(List<DataPoint> w)
        {
            double N = w.Count, weight, yvalue;
            double[] result = new double[(int)N];

            for (int n = 0; n < N; n++)
            {
                weight = (2 / N) * (N / 2 - Math.Abs(n - (N - 1) / 2));
                yvalue = w[n].YValues[0];
                result[n] = weight * yvalue;
            }
            return result;
        }

        /// <summary>
        /// Convert DataPoints to double array.
        /// </summary>
        /// <param name="selectedPoints"></param>
        /// <returns></returns>
        public static double[] convertPoints(List<DataPoint> selectedPoints)
        {
            double[] convertedPoints = new double[selectedPoints.Count];

            for (int i = 0; i < selectedPoints.Count; i++)
            {
                convertedPoints[i] = selectedPoints.ElementAt(i).YValues[0];
            }
            return convertedPoints;
        }

        /// <summary>
        /// Filters the selected points.
        /// </summary>
        public static void filter(Chart chart1, List<DataPoint> selectedPoints, List<DataPoint> selectedPoints2, float[] dataArray)
        {
            int maxLength = selectedPoints.Count;
            int maxLength2 = selectedPoints2.Count;
            double[] filterFrequencyD = new double[maxLength];
            double[] filterTimeD;
            double[] filteredData;

            for (int i = 0; i < maxLength; i++)
            {
                if (i < maxLength2 || (maxLength - maxLength2) < i)
                {
                    filterFrequencyD[i] = 1;
                }
                else
                {
                    filterFrequencyD[i] = 0;
                }
            }
            filterTimeD = IDFT(filterFrequencyD);
            filteredData = convolution(filterTimeD, dataArray);

            for (int i = 0; i < filteredData.Length; i++)
            {
                dataArray[i] = (float)filteredData[i];
            }
            WaveReader.FillChart(dataArray, chart1);
        }

        /// <summary>
        /// Applies convolution on the array.
        /// </summary>
        /// <param name="filterTimeD"></param>
        /// <returns></returns>
        private static double[] convolution(double[] filterTimeD, float[] dataArray)
        {
            int maxLength = dataArray.Length;
            int maxLength2 = filterTimeD.Length;
            double[] filteredData = new double[maxLength];
            double temp = 0;
            int counter = 0;

            for (int i = 0; i < maxLength; i++)
            {
                for (int j = 0; j < maxLength2; j++)
                {
                    if (i + maxLength2 < maxLength)
                    {
                        temp += dataArray[i + j] * filterTimeD[j];
                    }
                    else
                    {
                        temp += dataArray[i + j] * filterTimeD[j];
                        maxLength2 -= 1;
                    }
                    counter++;
                }
                filteredData[i] = temp;
            }
            return filteredData;
        }
    }
}
