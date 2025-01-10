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
        double indicePearson;

        private void Form1_Load(object sender, EventArgs e)
        {
            //DirectoryInfo dr = new DirectoryInfo(Directory.GetCurrentDirectory());
            //string path = dr.FullName + "//output.txt";
            //File.WriteAllText(path, Weather.GenerateResultsAt(lat, lon));
            //System.Diagnostics.Process.Start("notepad.exe", path);

            CWeatherReturn weather = JsonConvert.DeserializeObject<CWeatherReturn>(Weather.GenerateResultsAt(lat, lon));
            CPollutionReturn pollution = JsonConvert.DeserializeObject<CPollutionReturn>(Pollution.GenerateResultsAt(lat, lon));
            CreateChart(weather, pollution);

            if (intervallo != "mensile")
                MessageBox.Show($"Indice di Pearson: {indicePearson}.");
        }

        private void CreateChart(CWeatherReturn weather, CPollutionReturn pollution)
        {
            List<float> dataTemp = new List<float>();
            List<float> dataPoll = new List<float>();

            // Create the PlotModel
            PlotModel plotModel = new PlotModel { Title = $"Media {intervallo} temperatura e presenza particelle pm10 a lat: {lat}; lon: {lon}" };

            // Create a LineSeries and add data points
            LineSeries lineSeries = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.Cross,
                MarkerSize = 0.001,
                Color = OxyColor.FromRgb(255,0,0),
            };

            LineSeries lineSeriesP = new LineSeries
            {
                Title = "Line Series",
                MarkerType = MarkerType.Cross,
                MarkerSize = 0.001,
                Color = OxyColor.FromRgb(0, 0, 128),
            };


            CategoryAxis categoryAxis = new CategoryAxis
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


                    // Retrieve the amount of time needed to get the monthly average (1-28/29/30/31)
                    int avanza = getAvanza(mese, anno);

                    // Add the average to the lineSeries
                    float averageTemp = getAverage(weather.hourly.temperature_2m, i, i + avanza);
                    lineSeries.Points.Add(new DataPoint(j, averageTemp));
                    dataTemp.Add(averageTemp);

                    if (i + avanza <= pollution.hourly.pm10.Count)
                    {
                        float averagePoll = getAverage(pollution.hourly.pm10, i, i + avanza);
                        lineSeriesP.Points.Add(new DataPoint(j, averagePoll));
                        dataPoll.Add(averagePoll);
                    }

                    i += avanza;
                }
            } else 
            {
                int j = 0;
                for (int i = 0; i < weather.hourly.time.Count; j++)
                {
                    int anno = int.Parse(weather.hourly.time[i].Substring(0, 4));
                    categoryAxis.Labels.Add($"{anno}");

                    // Retrieve the amount of time needed to get the yearly average (1-365/366)
                    int avanza = getAvanza(anno);

                    // Add the average to the lineSeries
                    float averageTemp = getAverage(weather.hourly.temperature_2m, i, i + avanza);
                    lineSeries.Points.Add(new DataPoint(j, averageTemp));
                    dataTemp.Add(averageTemp);

                    if (i + avanza <= pollution.hourly.pm10.Count)
                    {
                        float averagePoll = getAverage(pollution.hourly.pm10, i, i + avanza);
                        lineSeriesP.Points.Add(new DataPoint(j, averagePoll));
                        dataPoll.Add(averagePoll);
                    }

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

            // Computes Pearson index
            indicePearson = CIndicePearson.CalcolaIndicePearson(dataTemp.ToArray(), dataPoll.ToArray());
        }

        // Returns the interval of time needed to take count of each month
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
                    if (isBisestile(anno))
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

        // Returns the interval of time needed to take count of each year
        int getAvanza(int anno)
        {
            if (isBisestile(anno))
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

        bool isBisestile(int anno)
        {
            return (anno % 4 == 0 && (anno % 100 != 0 || anno % 400 == 0));
        }
    }
}
