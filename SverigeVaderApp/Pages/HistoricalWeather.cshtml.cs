using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SverigeVaderApp.DAL;
using SverigeVaderApp.Models;

namespace SverigeVaderApp.Pages
{
    public class HistoricalWeatherModel : PageModel
    {
        public List<Measurement> Measurements { get; set; }

        public SelectList CitiesSelectList { get; set; }

        public SelectList ValueNames { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCity { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortOn { get; set; }

        private readonly IDataAccess _dataAccess;

        public HistoricalWeatherModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }


        public void OnGet()
        {
            string[] recordValuesNames = Record.GetValueNames();

            ValueNames = new SelectList(recordValuesNames);

            Measurements = _dataAccess.GetWeatherDataList().OrderByDescending(m => m.Date).ToList();
            
            CitiesSelectList = new SelectList(Measurements.GroupBy(m => m.City).OrderBy(g => g.Key).Select(g => g.Key));

            if (string.IsNullOrWhiteSpace(SelectedCity) == false)
            {
                Measurements = Measurements.Where(m => m.City == SelectedCity).ToList();
            }

            if (string.IsNullOrWhiteSpace(SortOn) == false)
            {
                switch (SortOn)
                {
                    case "City": 
                    case "Plats":
                        Measurements = Measurements.OrderBy(m => m.City).ToList();
                        break;
                    case "Date":
                    case "Datum":
                        Measurements = Measurements.OrderBy(m => m.Date).ToList(); ;
                        break;
                    case "Tid":
                        Measurements = Measurements.OrderBy(m => m.Date.Hour).ThenBy(m => m.Date.Minute).ToList();
                        break;
                    case "Description":
                    case "Väder":
                        Measurements = Measurements.OrderByDescending(m => m.Record.Description != null).ThenBy(m => m.Record.Description).ToList();
                        break;
                    case "Temperature":
                    case "Temperatur (C)":                        
                        Measurements = Measurements.OrderByDescending(m => m.Record.Temperature.HasValue).ThenBy(m => m.Record.Temperature).ToList();
                        break;
                    case "WindSpeed":
                    case "Vind (m/s)":
                        Measurements = Measurements.OrderByDescending(m => m.Record.WindSpeed.HasValue).ThenBy(m => m.Record.WindSpeed).ToList();
                        break;
                    case "WindDirection":
                    case "Vindriktning":
                        Measurements = Measurements.OrderByDescending(m => m.Record.WindDirection != null).ThenBy(m => m.Record.WindDirection).ToList();
                        break;
                    case "Clouds":
                    case "Molnighet (%)":
                        Measurements = Measurements.OrderByDescending(m => m.Record.Clouds.HasValue).ThenBy(m => m.Record.Clouds).ToList();
                        break;
                    case "RelativeHumidity":
                    case "Luftfuktighet (%)":
                        Measurements = Measurements.OrderByDescending(m => m.Record.RelativeHumidity.HasValue).ThenBy(m => m.Record.RelativeHumidity).ToList();
                        break;
                    case "UvIndex":
                    case "UV-index":
                        Measurements = Measurements.OrderByDescending(m => m.Record.UvIndex.HasValue).ThenBy(m => m.Record.UvIndex).ToList();
                        break;
                    default:
                        break;
                }
            }

            Measurements = Measurements.ToList();
        }
    }
}
