using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace WeatherForecast
{
    public static class Weather
    {
        static string APIKey = File.ReadAllText("apiKey.txt");

        static public string GenerateResultsAt(double lat, double lon, int time)
        {
            using (WebClient client = new WebClient())
            {
                return System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(client.DownloadString($"https://api.waqi.info/feed/geo:{lat};{lon}/?token={APIKey}")), new JsonSerializerOptions() { WriteIndented = true }); ;
            }
        }
        static public string GenerateResultsAtSearch(string keyword, int time)
        {
            using (WebClient client = new WebClient())
            {
                return System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(client.DownloadString($"https://api.waqi.info/search/?keyword={keyword}&token={APIKey}")), new JsonSerializerOptions() { WriteIndented = true }); ;
            }
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DirectoryInfo dr = new DirectoryInfo(Directory.GetCurrentDirectory());
            string path = dr.FullName + "//output.txt";
            File.WriteAllText(path, Weather.GenerateResultsAtSearch("padua", 1608204915));
            //File.WriteAllText(path, Weather.GenerateResultsAt(45.549999, 11.550000, 1608204915));
            System.Diagnostics.Process.Start("notepad.exe", path);
        }
    }
}
