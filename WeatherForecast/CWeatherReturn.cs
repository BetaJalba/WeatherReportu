using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast
{
    public class CWeatherReturn
    {
        public float latitude, longitude, generationtime_ms, elevation;
        public int utc_offset_seconds;
        public string timezone, timezone_abbreviation;
        public CHourlyUnits hourly_units;
        public CHourly hourly;
    
        public CWeatherReturn() 
        {

        }
    }

    public class CHourly 
    {
        public List<string> time;
        public List<float> temperature_2m;

        public CHourly()
        {

        }
    }

    public class CHourlyUnits
    {
        public string time, temperature_2m;

        public CHourlyUnits() 
        {
        }
    }

    public class CPollutionReturn
    {
        public float latitude, longitude, generationtime_ms, elevation;
        public int utc_offset_seconds;
        public string timezone, timezone_abbreviation;
        public CHourlyUnitsP hourly_units;
        public CHourlyP hourly;

        public CPollutionReturn()
        {

        }
    }

    public class CHourlyP
    {
        public List<string> time;
        public List<float> pm10;

        public CHourlyP()
        {

        }
    }

    public class CHourlyUnitsP
    {
        public string pm10;

        public CHourlyUnitsP()
        {
        }
    }
}
