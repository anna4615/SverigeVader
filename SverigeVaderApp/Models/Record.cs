using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SverigeVaderApp.Models
{
    public class Record
    {
        public string Icon { get; set; }

        [DisplayName("Väder")]
        public string Description { get; set; }

        [DisplayName("Temperatur (C)")]
        public double? Temperature { get; set; }

        public double? ApparentTemp { get; set; }

        [DisplayName("Vind (m/s)")]
        public double? WindSpeed { get; set; }

        [DisplayName("Vindriktning")]
        public string WindDirection { get; set; }

        [DisplayName("Molnighet (%)")]
        public double? Clouds { get; set; }

        [DisplayName("Luftfuktighet (%)")]
        public double? RelativeHumidity { get; set; }

        [DisplayName("UV-index")]
        public double? UvIndex { get; set; }



        public static string[] GetValueNames()
        {
            PropertyInfo[] valueNamesQuery = typeof(Record).GetProperties();

            string[] valueNamesArray = new string[valueNamesQuery.Length + 3];
            valueNamesArray[0] = "Plats";
            valueNamesArray[1] = "Datum";
            valueNamesArray[2] = "Tid";

            List<string> valueNamesList = new List<string>();


            // gör lista med Displaynames för properties som har ett sådant
            for (int i = 0; i < valueNamesQuery.Length; i++)
            {
                string[] splitString = valueNamesQuery[i].ToString().Split(" ");
                // hämtar property
                MemberInfo property = typeof(Record).GetProperty(splitString[1]);
                // hämtar attribut för property
                DisplayNameAttribute attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                    .Cast<DisplayNameAttribute>().FirstOrDefault();
                 
                if (attribute != null)
                {
                    //lägger till Displayname
                    valueNamesArray[i + 3] = attribute.DisplayName;
                }
                else
                {
                    //lägger till Propertyname
                    valueNamesArray[i + 3] = splitString[1];
                }
            }

            return valueNamesArray.Where(s => s != "Icon" && s != "ApparentTemp").ToArray();
        }
    }
}
