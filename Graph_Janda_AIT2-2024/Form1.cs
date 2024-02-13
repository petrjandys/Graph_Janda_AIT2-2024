using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Graph_Janda_AIT2_2024
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();

        }

        private void chart1_Click(object sender, EventArgs e)
        {
            

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGraph();
        }
        public void UpdateGraph()
        {
            if (datovyBodBindingSource1.DataSource == null)
            {
                return;
            }

            cartesianChart1.Series.Clear();
            SeriesCollection series = new SeriesCollection();
            var years = (from o in datovyBodBindingSource1.DataSource as List<DatovyBod> select new { Year = o.year }).Distinct();

            foreach(var year in years)
            {
                List<double> values = new List<double>();
               for(int month =1; month <=12;month++)
                {
                    double value = 0;
                    var data = from o in datovyBodBindingSource1.DataSource as List<DatovyBod>
                               where o.year.Equals(year.Year) && o.month.Equals(month)
                               orderby o.month ascending
                               select new { o.value, o.month };

                    if(data.SingleOrDefault() != null)
                    {
                        value = data.SingleOrDefault().value;
                        values.Add(value);
                    }
                }

                series.Add(new LineSeries()
                {
                    Title = year.Year.ToString(),
                    Values = new ChartValues<double>(values)

                });

            }
            cartesianChart1.Series = series;


        }
        private void Form1_Load(object sender, EventArgs e)
        {
            datovyBodBindingSource1.DataSource = new List<DatovyBod>();
            cartesianChart1.AxisX.Add(new Axis
            {
               Title = "Months",
                Labels = new[] { "leden", "únor", "březen", "duben", "květen", "červen", "červenec", "srpen", "září", "říjen", "listopad", "prosinec" }
            }); ;

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Value",
            }); 

            cartesianChart1.LegendLocation = LegendLocation.Right;

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)
        {
            UpdateGraph();
        }
        private void ExportDataToCSV()
        {         
                var data = (List<DatovyBod>)datovyBodBindingSource1.DataSource;
                if (data == null || data.Count == 0)
                {
                    MessageBox.Show("No data to export.");
                    return;
                }              
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Export to CSV",
                 
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {                
                    string filePath = saveFileDialog.FileName;                   
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {                       
                        sw.WriteLine("Year,Month,Value");                       
                        foreach (var item in data)
                        {
                            sw.WriteLine($"{item.year},{item.month},{item.value}");
                        }
                    }

                    MessageBox.Show($"Data exported successfully to {filePath}", "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ExportDataToCSV();
        }
        private void ExportChartToPNG()
        {                
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PNG files (*.png)|*.png",
                    Title = "Export Chart",                   
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK){
                 
                    string filePath = saveFileDialog.FileName;                    
                    Bitmap chartImage = new Bitmap(cartesianChart1.Width, cartesianChart1.Height);
                    cartesianChart1.DrawToBitmap(chartImage, new Rectangle(0, 0, chartImage.Width, chartImage.Height));
                                       
                    chartImage.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                    MessageBox.Show($"Chart exported successfully to {filePath}", "Export PNG", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
           
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ExportChartToPNG();
        }

        public void ImportFromCVS()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import CSV",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                List<DatovyBod> importedData = new List<DatovyBod>();
                using (StreamReader sr = new StreamReader(filePath))
                {           
                    sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string[] values = sr.ReadLine().Split(',');
                        if (values.Length == 3 && int.TryParse(values[0], out int year) && int.TryParse(values[1], out int month) && int.TryParse(values[2], out int dataValue))
                        {
                            importedData.Add(new DatovyBod { year = year, month = month, value = dataValue });
                        }
                        else
                        {
                            MessageBox.Show("Invalid data format in CSV file.", "Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                datovyBodBindingSource1.DataSource = importedData;
                UpdateGraph();

                MessageBox.Show($"Data imported successfully" , "Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            ImportFromCVS();
        }
    }
}
