using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SverigeVaderApp.DAL;
using SverigeVaderApp.Models;

namespace SverigeVaderApp.Pages
{
    public class StatisticsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SelectedCity { get; set; }

        public List<string> Cities { get; set; }

        public SelectList CitiesSelectList { get; set; }

        public List<Measurement> Measurements { get; set; }

        private readonly IDataAccess _dataAccess;

        public StatisticsModel(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void OnGet()
        {
            //måsta hämta data för att kunna göra lista med alla städer som det finns data för
            Measurements = _dataAccess.GetWeatherDataList().ToList();

            Cities = new List<string>(Measurements.GroupBy(m => m.City).OrderBy(m => m.Key).Select(g => g.Key));

            CitiesSelectList = new SelectList(Cities);
        }
    }
}
