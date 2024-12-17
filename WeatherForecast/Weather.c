using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast
{
    public static class Weather
    {
        static string APIKey = "b7ad615a42f3ce387f1c5c918777c4d2";

        static public string GenerateResults(string lat, string lon, string part) 
        {
            using(WebClient client = new WebClient()) {
                string s = client.DownloadString(?"https ://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude={part}&appid={APIKey}");
            }
        }
    }
}
