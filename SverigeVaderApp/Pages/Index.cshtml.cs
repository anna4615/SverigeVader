using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using SverigeVaderApp.DAL;
using SverigeVaderApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SverigeVaderApp.Pages
{
    public class IndexModel : PageModel
    {
        public List<Measurement> CurrentWeather { get; set; }
        public List<Measurement> HistoricalWeather { get; set; }
        public string[] Cities { get; set; }

        private readonly HttpClient _client;
        private readonly IDataAccess _dataAccess;

        public IndexModel(HttpClient client, IDataAccess dataAcceass)
        {
            _dataAccess = dataAcceass;
            _client = client;
        }


        public async Task OnGetAsync()
        {
            Measurement measurement;

            //data från databas
            //DataAccess collection = new DataAccess();  //Dependency Injection istället
            HistoricalWeather = _dataAccess.GetWeatherDataList().ToList();


            //var client = new HttpClient(); // DI istället

            // sätter kort timespan för att det inte skall ta så lång tid att komma igenom alla städer om Weatherbit inte svarar
            _client.Timeout = TimeSpan.FromSeconds(3);

            Cities = new string[] { "Bromma", "Kiruna", "Östersund", "Göteborg", "Lund", "Södertälje" };
            CurrentWeather = new List<Measurement>();

            foreach (string city in Cities)
            {
                //om en plats misslyckas går ändå programmet vidare med nästa stad
                try
                {
                    Task<string> getWeatherStringTask = _client.GetStringAsync($"https://api.weatherbit.io/v2.0/current?city={city}&country=SE&lang=sv&key=7e4d605f8e3344deb33298601bbd8754");
                    string resultString = await getWeatherStringTask;
                    WBWeatherData weatherData = JsonSerializer.Deserialize<WBWeatherData>(resultString);

                    measurement = new Measurement()
                    {
                        Id = Guid.NewGuid(),
                        City = city,
                        Date = DateTime.UtcNow,
                        Record = new Record()
                        {
                            Icon = weatherData.data[0].weather.icon + ".png",
                            Description = weatherData.data[0].weather.description,
                            Temperature = weatherData.data[0].temp,
                            ApparentTemp = weatherData.data[0].app_temp,
                            WindSpeed = weatherData.data[0].wind_spd,
                            WindDirection = weatherData.data[0].wind_cdir,
                            Clouds = weatherData.data[0].clouds,
                            RelativeHumidity = weatherData.data[0].rh,
                            UvIndex = weatherData.data[0].uv,
                        }
                    };
                }

                catch (Exception)
                {
                    measurement = null;
                }


                if (measurement != null)
                {
                    CurrentWeather.Add(measurement);

                    DateTime lastSavedMeasurement = HistoricalWeather
                                                .Where(m => m.City == city)
                                                .OrderByDescending(m => m.Date)
                                                .FirstOrDefault().Date;

                    //värden sparas bara en gång / timme till databasen för vardera stad
                    if (lastSavedMeasurement.AddHours(1) < DateTime.UtcNow)
                    {
                        await _dataAccess.GetWeatherDataCollection().InsertOneAsync(measurement);
                        HistoricalWeather.Add(measurement);
                    }
                }
            }
        }
    }
}
