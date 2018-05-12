using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundAnalyzer
{
    class EditChart
    {
        /// <summary>
        /// Copies the selected graphs onto copiedPoints variable and deletes from graph.
        /// </summary>
        public static void deletePoints(Chart chart1, List<DataPoint> selectedPoints, ref List<DataPoint> copiedPoints, ref float[] dataArray)
        {
            if (selectedPoints == null)
            {
                return;
            }

            float[] newArray = new float[dataArray.Length - selectedPoints.Count + 1];
            double[] deleteData = convertPoints(selectedPoints);
            copiedPoints = selectedPoints;

            int startPoint = (int)selectedPoints[0].XValue;
            int endPoint = (int)selectedPoints[selectedPoints.Count - 1].XValue;

            for (int i = 0; i < startPoint; i++)
            {
                newArray[i] = dataArray[i];
            }

            for (int i = endPoint; i < dataArray.Length; i++)
            {
                newArray[startPoint] = dataArray[i];
                startPoint++;
            }

            byte[] updatedArray = new byte[newArray.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                updatedArray[i] = (byte)(newArray[i] + (byte)128);
            }
            updateAudio(updatedArray);
            dataArray = newArray;
            WaveReader.FillChart(newArray, chart1);
        }

        /// <summary>
        /// Paste copied graph.
        /// </summary>
        public static void copyPaste(Chart chart1, List<DataPoint> selectedPoints, ref List<DataPoint> copiedPoints, float[] dataArray)
        {
            float[] newArray = new float[dataArray.Length + copiedPoints.Count + 1];
            double[] copyData = convertPoints(copiedPoints);

            int startPoint = (int)selectedPoints[0].XValue;
            int endPoint = (int)selectedPoints[selectedPoints.Count - 1].XValue;

            for (int i = 0; i < startPoint; i++)
            {
                newArray[i] = dataArray[i];
            }

            for (int i = startPoint, j = 0; j < copyData.Length; i++, j++)
            {
                newArray[i] = (float)copyData[j];
            }

            for (int i = copyData.Length + startPoint, j = startPoint; j < dataArray.Length; i++, j++)
            {
                newArray[i] = dataArray[j];
            }

            byte[] updatedArray = new byte[newArray.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                updatedArray[i] = (byte)(newArray[i] + (byte)128);
            }
            updateAudio(updatedArray);
            dataArray = newArray;
            WaveReader.FillChart(newArray, chart1);
        }

        /// <summary>
        /// Paste for when cut is called.
        /// </summary>
        public static void cutPaste(Chart chart1, List<DataPoint> selectedPoints, ref List<DataPoint> copiedPoints, float[] dataArray)
        {
            if (selectedPoints == null)
            {
                return;
            }

            float[] newArray = new float[dataArray.Length + copiedPoints.Count - selectedPoints.Count + 1];

            int startPoint = (int)selectedPoints[0].XValue;
            int endPoint = (int)selectedPoints[selectedPoints.Count - 1].XValue;

            for (int i = 0; i < startPoint; i++)
            {
                newArray[i] = dataArray[i];
            }

            for (int i = startPoint, j = 0; j < copiedPoints.Count; j++, i++)
            {
                newArray[i] = (float)copiedPoints.ElementAt(j).YValues[0];
            }

            for (int i = endPoint, j = copiedPoints.Count + startPoint; i < dataArray.Length; i++, j++)
            {
                newArray[j] = dataArray[i];
            }

            byte[] updatedArray = new byte[newArray.Length];
            for (int i = 0; i < newArray.Length; i++)
            {
                updatedArray[i] = (byte)(newArray[i] + (byte)128);
            }
            Form1.paste = 2;
            updateAudio(updatedArray);
            dataArray = newArray;
            WaveReader.FillChart(newArray, chart1);
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
        /// Updates the audio when it is modified.
        /// </summary>
        /// <param name="updateRecord"></param>
        private static void updateAudio(byte[] updateRecord)
        {
            uint dllDataSize = (uint)updateRecord.Length;
            Recorder.SetDataSize(dllDataSize);
            IntPtr dllData = Recorder.getRecorded();
            Marshal.Copy(updateRecord, 0, dllData, (int)dllDataSize);
        }
    }
}
