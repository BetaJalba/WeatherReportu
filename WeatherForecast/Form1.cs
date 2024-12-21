using Newtonsoft.Json;
using System;
using System.Net;
using System.Text.Json;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherForecast
{
    public enum Mesi 
    {
        PariPrimaMeta = 30,
        DispariPrimaMeta = 31,
        PariSecondaMeta = 31,
        DispariSecondaMeta = 30,
        Febbraio = 28,
        FebbraioBisestile = 29
    }

    public partial class Form1 : Form
    {
        public Form1(float lon, float lat, string intervallo)
        {
            this.lat = lat;
            this.lon = lon;
            this.intervallo = intervallo;
            InitializeComponent();
        }

        float lat, lon;
        string intervallo;

        private void Form1_Load(object sender, EventArgs e)
        {
            //DirectoryInfo dr = new DirectoryInfo(Directory.GetCurrentDirectory());
            //string path = dr.FullName + "//output.txt";
            //File.WriteAllText(path, Weather.GenerateResultsAt(lat, lon));
            //System.Diagnostics.Process.Start("notepad.exe", path);

            CWeatherReturn weather = JsonConvert.DeserializeObject<CWeatherReturn>(Weather.GenerateResultsAt(lat, lon));
            CPollutionReturn pollution = JsonConvert.DeserializeObject<CPollutionReturn>(Pollution.GenerateResultsAt(lat, lon));
            CreateChart(weather, pollution);
            MessageBox.Show(CalcolaIndicePearson(weather.hourly.temperature_2m.ToArray(), pollution.hourly.pm10.ToArray()).ToString());
        }

        private void CreateChart(CWeatherReturn weather, CPollutionReturn pollution)
        {
            // Create the PlotModel
            var plotModel = new PlotModel { Title = $"Media {intervallo} temperatura e presenza particelle pm10 a lat: {lat}; lon: {lon}" };

            // Create a LineSeries and add data points
            var lineSeries = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.Cross,
                MarkerSize = 0.001,
                Color = OxyColor.FromRgb(255,0,0),
            };

            var lineSeriesP = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.Cross,
                MarkerSize = 0.001,
                Color = OxyColor.FromRgb(0, 0, 128),
            };


            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom, // Place the category axis at the bottom
                Key = "CategoryAxis",
                MajorStep = (intervallo == "mensile") ? 12 : 1,
            };

            if (intervallo == "mensile") 
            {
                int j = 0;
                for (int i = 0; i < weather.hourly.time.Count; j++)
                {
                    int anno = int.Parse(weather.hourly.time[i].Substring(0, 4));
                    int mese = int.Parse(weather.hourly.time[i].Substring(5, 2));
                    categoryAxis.Labels.Add($"{anno}-{mese}");

                    int avanza = getAvanza(mese, anno);
                    lineSeries.Points.Add(new DataPoint(j, getAverage(weather.hourly.temperature_2m, i, i + avanza)));
                    lineSeriesP.Points.Add(new DataPoint(j, getAverage(pollution.hourly.pm10, i, i + avanza)));
                    i += avanza;
                }
            } else 
            {
                int j = 0;
                for (int i = 0; i < weather.hourly.time.Count; j++)
                {
                    int anno = int.Parse(weather.hourly.time[i].Substring(0, 4));
                    categoryAxis.Labels.Add($"{anno}");

                    int avanza = getAvanza(anno);
                    lineSeries.Points.Add(new DataPoint(j, getAverage(weather.hourly.temperature_2m, i, i + avanza)));
                    lineSeriesP.Points.Add(new DataPoint(j, getAverage(pollution.hourly.pm10, i, i + avanza)));
                    i += avanza + 1;
                }
            }

            

            plotModel.Axes.Add(categoryAxis);

            // Add the LineSeries to the PlotModel
            plotModel.Series.Add(lineSeries);
            plotModel.Series.Add(lineSeriesP);

            // Create the PlotView and set the PlotModel
            var plotView = new OxyPlot.WindowsForms.PlotView
            {
                Dock = DockStyle.Fill,
                Model = plotModel
            };

            // Add the PlotView to the Form's Controls
            this.Controls.Add(plotView);
        }

        int getAvanza(int mese, int anno)
        {
            if (mese <= 7)
            {
                if (mese % 2 != 0)
                {
                    return 24 * (int)Mesi.DispariPrimaMeta;
                }
                else if (mese == 2)
                {
                    if (IsBisestile(anno))
                        return 24 * (int)Mesi.FebbraioBisestile;
                    else
                        return 24 * (int)Mesi.Febbraio;
                }
                else
                {
                    return 24 * (int)Mesi.PariPrimaMeta;
                }
            }
            else
            {
                if (mese % 2 != 0)
                {
                    return 24 * (int)Mesi.DispariSecondaMeta;
                }
                else
                {
                    return 24 * (int)Mesi.PariSecondaMeta;
                }
            }
        }

        int getAvanza(int anno)
        {
            if (IsBisestile(anno))
                return 24 * 366;
            else
                return 24 * 365;
        }

        private float getAverage(List<float> list, int start, int end)
        {
            var sum = 0f;
            var count = 0;

            for (int i = start; i < end && i < list.Count; i++, count++) 
            {
                sum += list[i];
            }

            return sum / (float)count;
        }

        bool IsBisestile(int anno)
        {
            return (anno % 4 == 0 && (anno % 100 != 0 || anno % 400 == 0));
        }

        double CalcolaIndicePearson(float[] X, float[] Y)
        {
            if (X.Length != Y.Length)
            {
                Array.Resize(ref Y, X.Length);
            }

            int n = X.Length;

            // Calcolo delle somme necessarie
            double sommaX = X.Sum();
            double sommaY = Y.Sum();
            double sommaXY = X.Zip(Y, (x, y) => x * y).Sum();
            double sommaX2 = X.Select(x => x * x).Sum();
            double sommaY2 = Y.Select(y => y * y).Sum();

            // Calcolo dell'indice di Pearson
            double numeratore = (n * sommaXY) - (sommaX * sommaY);
            double denominatore = Math.Sqrt((n * sommaX2 - Math.Pow(sommaX, 2)) * (n * sommaY2 - Math.Pow(sommaY, 2)));

            if (denominatore == 0)
            {
                return 0; // Se il denominatore è 0, la correlazione non è definita (avviso: divisione per zero)
            }

            return numeratore / denominatore;
        }
    }
}
