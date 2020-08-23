using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SoundAnalyzer
{
    public partial class Form1 : Form
    {
        //GUI variables
        private Panel panel;
        private CustomButton closeForm;
        private CustomButton maxForm;
        private CustomButton minForm;
        private Color themeColor;
        private Color themeBackgroundColor;
        private TextBox title;
        public Chart chart1;
        public Chart chart2;
        public Chart chart3;
        private ChartArea chartArea1;
        private ChartArea chartArea2;
        private ChartArea chartArea3;
        private Legend legend1;
        private Legend legend2;
        private Legend legend3;
        private Series series1;
        private Series series2;
        private Series series3;

        //Custom GUI offset
        private const float BUTTON_FONT_SIZE = 8f;
        private const int CLOSE_FORM_HORZ_OFFSET = 30;
        private const int PANEL_VERT_OFFSET = 25;
        private const int MAX_FORM_HORZ_OFFSET = 60;
        private const int MIN_FORM_HORZ_OFFSET = 90;
        private const int LEFT_OFFSET = 14;

        //For overriding movement of window
        private const int WM_NCHITTEST = 0x84;
        private const int HT_CLIENT = 0x1;
        private const int HT_CAPTION = 0x2;

        //variables related to audio
        static List<DataPoint> selectedPoints = null;
        static List<DataPoint> selectedPoints2 = null;
        static List<DataPoint> copiedPoints = null;
        static float[] dataArray;
        static Point mdown = Point.Empty;
        public static int paste = 2;
        private bool zoom = false;

        public Form1()
        {
            this.themeColor = Color.FromArgb(200, 220, 220, 220);
            this.themeBackgroundColor = Color.FromArgb(175, 0, 0, 0);
            InitializeComponent();
            customDesign();
            CustomizeMenuStrip(menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        public void customDesign()
        {
            //setting background color
            BackColor = Color.FromArgb(35, 35, 35);

            //panel 1
            panel = new Panel();
            panel.BackColor = System.Drawing.Color.Transparent;
            panel.Controls.Add(this.menuStrip1);
            panel.Location = new System.Drawing.Point(0, 25);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(this.Width, this.Height - 25);

            //close button
            closeForm = new CustomButton();
            closeForm.ForeColor = themeColor;
            closeForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            closeForm.FlatAppearance.BorderSize = 0;
            closeForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            closeForm.Location = new System.Drawing.Point(this.Width - 45, 0);
            closeForm.Name = "closeForm";
            closeForm.Size = new System.Drawing.Size(30, 25);
            closeForm.TabIndex = 6;
            closeForm.Text = "X";
            closeForm.UseVisualStyleBackColor = true;
            closeForm.Click += new System.EventHandler(closeForm_Click);

            //maximize button
            maxForm = new CustomButton();
            maxForm.ForeColor = themeColor;
            maxForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            maxForm.FlatAppearance.BorderSize = 0;
            maxForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            maxForm.Location = new System.Drawing.Point(this.Width - 75, 0);
            maxForm.Name = "maxForm";
            maxForm.Size = new System.Drawing.Size(30, 25);
            maxForm.TabIndex = 5;
            maxForm.TabStop = false;
            maxForm.Text = "⎕";
            maxForm.UseMnemonic = false;
            maxForm.UseVisualStyleBackColor = true;
            maxForm.Click += new System.EventHandler(maxForm_Click);

            //minimize button
            minForm = new CustomButton();
            minForm.ForeColor = themeColor;
            minForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            minForm.FlatAppearance.BorderSize = 0;
            minForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            minForm.Location = new System.Drawing.Point(this.Width - 105, 0);
            minForm.Name = "minForm";
            minForm.Size = new System.Drawing.Size(30, 25);
            minForm.TabIndex = 4;
            minForm.TabStop = false;
            minForm.Text = "_";
            minForm.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            minForm.UseMnemonic = false;
            minForm.UseVisualStyleBackColor = true;
            minForm.Click += new System.EventHandler(minForm_Click);

            //title
            title = new TextBox();
            title.BackColor = System.Drawing.Color.FromArgb(35, 35, 35);
            title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            title.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F,
                System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title.Location = new System.Drawing.Point(LEFT_OFFSET, 4);
            title.Name = "Title";
            title.Size = new System.Drawing.Size(163, 16);
            title.ReadOnly = true;
            title.Enabled = false;
            title.TabStop = false;
            title.ForeColor = themeColor;
            title.Text = "Sound Analyzer";
            
            // chart1
            chartArea1 = new ChartArea();
            legend1 = new Legend();
            series1 = new Series();
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(12, 31);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(776, 216);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.BackColor = Color.White;
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDown);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseUp);

            // chart2
            chartArea2 = new ChartArea();
            legend2 = new Legend();
            series2 = new Series();
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(12, 252);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(375, 251);
            this.chart2.TabIndex = 2;
            this.chart2.Text = "chart2";
            this.chart2.BackColor = Color.White;
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chart2_MouseDown);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chart2_MouseUp);

            // chart3
            chartArea3 = new ChartArea();
            legend3 = new Legend();
            series3 = new Series();
            chartArea3.Name = "ChartArea1";
            this.chart3.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chart3.Legends.Add(legend3);
            this.chart3.Location = new System.Drawing.Point(408, 253);
            this.chart3.Name = "chart3";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart3.Series.Add(series3);
            this.chart3.Size = new System.Drawing.Size(380, 251);
            this.chart3.TabIndex = 3;
            this.chart3.Text = "chart3";
            this.chart3.BackColor = Color.White;

            //Controls
            Resize += new System.EventHandler(this.WindowResize);
            Controls.Add(panel);
            Controls.Add(closeForm);
            Controls.Add(maxForm);
            Controls.Add(minForm);
            Controls.Add(title);

            //add to panel
            panel.Controls.Add(chart1);
            panel.Controls.Add(chart2);
            panel.Controls.Add(chart3);
        }

        /// <summary>
        /// Redraw form when resized
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event</param>
        private void WindowResize(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                Control control = (Control)sender;
                int w = control.Size.Width;
                int h = control.Size.Height;
                closeForm.Location = new Point(w - CLOSE_FORM_HORZ_OFFSET, 0);
                panel.Size = new Size(w, h - PANEL_VERT_OFFSET);
                maxForm.Location = new Point(w - MAX_FORM_HORZ_OFFSET, 0);
                minForm.Location = new Point(w - MIN_FORM_HORZ_OFFSET, 0);
            }
        }

        /// <summary>
        /// Customize menuStrip's color
        /// </summary>
        /// <param name="menuStrip">Menustrip object</param>
        private void CustomizeMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.Renderer = new MyRenderer(themeBackgroundColor);
            menuStrip.BackColor = Color.Transparent;
            fileToolStripMenuItem.ForeColor = themeColor;
            editToolStripMenuItem.ForeColor = themeColor;
            openToolStripMenuItem.ForeColor = themeColor;
            cutToolStripMenuItem.ForeColor = themeColor;
            copyToolStripMenuItem.ForeColor = themeColor;
            pasteToolStripMenuItem.ForeColor = themeColor;
            recordToolStripMenuItem.ForeColor = themeColor;
            recorderToolStripMenuItem.ForeColor = themeColor;
            graphToolStripMenuItem.ForeColor = themeColor;
            modifyToolStripMenuItem.ForeColor = themeColor;
            fourierToolStripMenuItem.ForeColor = themeColor;
            inverseFourierToolStripMenuItem.ForeColor = themeColor;
            windowingToolStripMenuItem.ForeColor = themeColor;
            filterToolStripMenuItem.ForeColor = themeColor;
            zoomToolStripMenuItem.ForeColor = themeColor;
            toggleToolStripMenuItem.ForeColor = themeColor;
            zoomOutToolStripMenuItem.ForeColor = themeColor;
        }

        /// <summary>
        /// Closes the form when closeForm button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeForm_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Maximize the form when maxForm button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maxForm_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        /// <summary>
        /// Minimizes the form when minForm button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minForm_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)(HT_CAPTION);
            }
        }

        /// <summary>
        /// Custom tool strip renderer
        /// </summary>
        private class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer(Color themeBackgroundColor) : base(new MyColors(themeBackgroundColor)) { }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                var toolStripMenuItem = e.Item as ToolStripMenuItem;
                if (toolStripMenuItem != null)
                {
                    e.ArrowColor = Color.FromArgb(200, 144, 238, 144);
                }
                base.OnRenderArrow(e);
            }
        }

        /// <summary>
        /// Class that override specific form item color
        /// </summary>
        private class MyColors : ProfessionalColorTable
        {
            private Color themeBackgroundColor;

            public MyColors(Color themeBackgroundColor)
            {
                this.themeBackgroundColor = themeBackgroundColor;
            }

            public override Color MenuItemSelected
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonSelectedGradientMiddle
            {
                get { return Color.Transparent; }
            }

            public override Color ButtonSelectedHighlight
            {
                get { return Color.Transparent; }
            }

            public override Color ButtonCheckedGradientBegin
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return themeBackgroundColor; }
            }
            public override Color ButtonSelectedBorder
            {
                get { return Color.FromArgb(200, 144, 238, 144); }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color CheckSelectedBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return themeBackgroundColor; }
            }
            public override Color MenuItemBorder
            {
                get { return Color.Black; }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.Transparent; }
            }
            public override Color CheckBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color CheckPressedBackground
            {
                get { return themeBackgroundColor; }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return Color.Transparent; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return Color.Transparent; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return Color.Transparent; }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.Transparent; }
            }
        }

        /// <summary>
        /// Opens the selected audio file and graphs it on the the main graph.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float[] leftArray;
            float[] rightArray;

            string file = "NULL";
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                }
                catch (IOException)
                {
                }
            }
            Console.WriteLine(file);

            if (String.ReferenceEquals(file, "NULL"))
            {
                Console.WriteLine("...Failed to load note");
            }
            else if (WaveReader.readWav(file, out leftArray, out rightArray))
            {
                dataArray = new float[leftArray.Length];
                for (int i = 0; i < leftArray.Length; i++)
                {
                    dataArray[i] = leftArray[i];
                }
                WaveReader.FillChart(leftArray, chart1);
            }
        }
        
        /// <summary>
        /// Mouse down event for chart1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            mdown = e.Location;
            selectedPoints = new List<DataPoint>();

            Axis ax = chart1.ChartAreas[0].AxisX;
            Axis ay = chart1.ChartAreas[0].AxisY;
            int miny = (int)ay.Minimum;
            int maxy = (int)ay.Maximum;

            chart1.Refresh();
        }

        /// <summary>
        /// Mouse up event for chart1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            Axis ax = chart1.ChartAreas[0].AxisX;
            Axis ay = chart1.ChartAreas[0].AxisY;
            int miny = (int)ay.Minimum;
            int maxy = (int)ay.Maximum;

            Rectangle rect = GetRectangle(mdown, e.Location);

            if (selectedPoints != null)
            {
                foreach (DataPoint dp in chart1.Series[0].Points)
                {
                    int x = (int)ax.ValueToPixelPosition(dp.XValue);
                    int y = (int)ay.ValueToPixelPosition(dp.YValues[0]);
                    if (rect.Contains(new Point(x, y))) selectedPoints.Add(dp);
                }

                // optionally color the found datapoints:
                foreach (DataPoint dp in chart1.Series[0].Points)
                    dp.Color = selectedPoints.Contains(dp) ? Color.Red : Color.Black;
            }
        }

        /// <summary>
        /// Mouse down event for chart2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chart2_MouseDown(object sender, MouseEventArgs e)
        {
            mdown = e.Location;
            selectedPoints2 = new List<DataPoint>();

            Axis ax = chart2.ChartAreas[0].AxisX;
            Axis ay = chart2.ChartAreas[0].AxisY;
            int miny = (int)ay.Minimum;
            int maxy = (int)ay.Maximum;

            chart2.Refresh();
        }

        /// <summary>
        /// Mouse up event for chart2.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void chart2_MouseUp(object sender, MouseEventArgs e)
        {
            Axis ax = chart2.ChartAreas[0].AxisX;
            Axis ay = chart2.ChartAreas[0].AxisY;
            int miny = (int)ay.Minimum;
            int maxy = (int)ay.Maximum;
            int endPoint = (int)chart2.ChartAreas[0].CursorX.SelectionEnd;
            int startPoint = (int)chart2.ChartAreas[0].CursorX.SelectionStart;

            if (startPoint > endPoint)
            {
                endPoint = startPoint;
            }

            foreach (DataPoint dp in chart2.Series[0].Points)
            {
                dp.Color = Color.Black;
            }

            for (int i = 0; i < endPoint; i++)
            {
                selectedPoints2.Add(chart2.Series[0].Points[i]);
                chart2.Series[0].Points[i].Color = Color.Red;
            }
        }

        /// <summary>
        /// Rectangle drawer for chart1.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), 15,
                Math.Abs(p1.X - p2.X), 125);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paste = 1;
            EditChart.deletePoints(chart1, selectedPoints, ref copiedPoints, ref dataArray);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copiedPoints = selectedPoints;
            paste = 0;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (copiedPoints != null)
            {
                if (paste == 0)
                {
                    EditChart.copyPaste(chart1, selectedPoints, ref copiedPoints, dataArray);
                }
                else if (paste == 1)
                {
                    EditChart.cutPaste(chart1, selectedPoints, ref copiedPoints, dataArray);
                }
            }
        }

        private void recorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recorder.DlgBox();
        }

        private void graphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IntPtr winData = Recorder.getRecorded();
            uint size = Recorder.getSize();
            byte[] convData = new byte[size];
            Marshal.Copy(winData, convData, 0, (int)size);

            float[] convData2 = new float[size];
            for (uint i = 0; i < size; i++)
            {
                convData2[i] = convData[i] - (byte)128;
            }

            dataArray = new float[convData2.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataArray[i] = convData2[i];
            }
            WaveReader.FillChart(convData2, chart1);
        }

        private void fourierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WaveReader.FillChartDouble(Fourier.DFT(EditChart.convertPoints(selectedPoints)), chart2);
        }

        private void inverseFourierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WaveReader.FillChartDouble(Fourier.IDFT(EditChart.convertPoints(selectedPoints)), chart3);
        }

        private void windowingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[] windowingPoints;
            windowingPoints = Fourier.Windowing(selectedPoints);
            WaveReader.FillChartDouble(Fourier.DFT(windowingPoints), chart3);
        }

        private void filterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Fourier.filter(chart1, selectedPoints, selectedPoints2, dataArray);
        }

        private void toggleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zoom = !zoom;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = zoom;
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
        }
    }

    /// <summary>
    /// Custom button that override Button
    /// </summary>
    public class CustomButton : Button
    {
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }
}
