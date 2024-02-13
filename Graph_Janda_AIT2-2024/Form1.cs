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
    }
}
