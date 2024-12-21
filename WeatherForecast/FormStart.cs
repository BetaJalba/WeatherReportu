using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherForecast
{
    public partial class FormStart : Form
    {
        public FormStart()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float lon = (float)numericUpDown1.Value;
            float lat = (float)numericUpDown2.Value;
            string intervallo = comboBox1.Text.ToLower();

            Form1 temperatura = new Form1(lat, lon, intervallo);
            temperatura.ShowDialog();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void FormStart_Load_1(object sender, EventArgs e)
        {
            comboBox1.Text = "Mensile";
            comboBox1.Items.Add("Mensile");
            comboBox1.Items.Add("Annuale");
        }
    }
}
