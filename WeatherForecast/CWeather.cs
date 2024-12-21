using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherForecast
{
    public static class Weather
    {
        static public string GenerateResultsAt(double lat, double lon)
        {
            using (WebClient client = new WebClient())
            {
                return System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(client.DownloadString($"https://archive-api.open-meteo.com/v1/archive?latitude={lat}&longitude={lon}&start_date=2010-01-01&end_date=2024-12-01&hourly=temperature_2m")), new JsonSerializerOptions() { WriteIndented = true }); ;
            }
        }
    }

    public static class Pollution
    {
        static public string GenerateResultsAt(double lat, double lon)
        {
            using (WebClient client = new WebClient())
            {
                return System.Text.Json.JsonSerializer.Serialize(JsonDocument.Parse(client.DownloadString($"https://air-quality-api.open-meteo.com/v1/air-quality?latitude={lat}&longitude={lon}&hourly=pm10&start_date=2013-01-01&end_date=2024-12-01")), new JsonSerializerOptions() { WriteIndented = true }); ;
            }
        }
    }
}
