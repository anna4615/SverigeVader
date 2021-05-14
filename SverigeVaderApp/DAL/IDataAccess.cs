using MongoDB.Driver;
using SverigeVaderApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SverigeVaderApp.DAL
{
    public interface IDataAccess
    {
        IEnumerable<Measurement> GetWeatherDataList();
        IMongoCollection<Measurement> GetWeatherDataCollection();
    }
}
