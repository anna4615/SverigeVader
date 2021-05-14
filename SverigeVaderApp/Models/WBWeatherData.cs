using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SverigeVaderApp.Models
{
    public class WBWeatherData
    {
        public Data[] data { get; set; }
        public int count { get; set; }
    }

    public class Data
    {
        public float rh { get; set; }
        public string pod { get; set; }
        public string timezone { get; set; }
        public string ob_time { get; set; }
        public string country_code { get; set; }
        public int clouds { get; set; }
        public int ts { get; set; }
        public float solar_rad { get; set; }
        public string state_code { get; set; }
        public string city_name { get; set; }
        public float wind_spd { get; set; }
        public string wind_cdir_full { get; set; }
        public string wind_cdir { get; set; }
        public float vis { get; set; }
        public float h_angle { get; set; }
        public string sunset { get; set; }
        public float snow { get; set; }
        public float uv { get; set; }
        public float precip { get; set; }
        public float wind_dir { get; set; }
        public string sunrise { get; set; }
        public Weather weather { get; set; }
        public string datetime { get; set; }
        public float temp { get; set; }
        public string station { get; set; }
        public float elev_angle { get; set; }
        public float app_temp { get; set; }
    }

    public class Weather
    {
        public string icon { get; set; }
        public int code { get; set; }
        public string description { get; set; }
    }
}
