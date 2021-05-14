using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SverigeVaderApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SverigeVaderApp.DAL
{
    public class MongoDataAccess : IDataAccess
    {
        private readonly IConfiguration _configuration;

        public MongoDataAccess(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Measurement> GetWeatherDataList()
        {
            MongoClient client = GetMongoClient();

            IMongoDatabase dataBase = client.GetDatabase(_configuration["MongoDbSettings:DbName"]);
            IMongoCollection<Measurement> collection = dataBase.GetCollection<Measurement>(_configuration["MongoDbSettings:CollectionName"]);

            return collection.Find(new BsonDocument()).ToList();
        }

        public IMongoCollection<Measurement> GetWeatherDataCollection()
        {
            MongoClient client = GetMongoClient();

            IMongoDatabase dataBase = client.GetDatabase(_configuration["MongoDbSettings:DbName"]);
            IMongoCollection<Measurement> collection = dataBase.GetCollection<Measurement>(_configuration["MongoDbSettings:CollectionName"]);

            return collection;
        }


        //MongoDbSettings från secrets.json
        private MongoClient GetMongoClient()
        {
            MongoClientSettings settings = new MongoClientSettings();

            settings.Server = new MongoServerAddress(_configuration["MongoDbSettings:Host"], 10255);
            settings.UseTls = true;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            settings.RetryWrites = false;

            MongoIdentity identity = new MongoInternalIdentity(_configuration["MongoDbSettings:DbName"], _configuration["MongoDbSettings:UserName"]);
            MongoIdentityEvidence evidence = new PasswordEvidence(_configuration["MongoDbSettings:PassWord"]);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            MongoClient mongoClient = new MongoClient(settings);

            return mongoClient;

            //    MongoClient mongoClient = new MongoClient(_configuration["ConnectionStrings:CosmosMongoConnection"]);
            //    return mongoClient;
        }


        // Villkor för vår:
        // - Första dygnet av 7 dygn i rad med medeltemperatur över 0,0C
        // - Kan starta tidigast 15:e februari       

        // Villkor för sommar:
        // - Första dygnet av 5 dygn i rad med medeltemperatur över 10,0C
        // - Kan starta tidigast 15:e februari

        public DateTime StartOfSeason(string city, string season)
        {
            List<Measurement> measurements = GetWeatherDataList().ToList();

            DateTime earliestStartOfSpringOrSummer = new DateTime(DateTime.Now.Year, 2, 15);

            DailyTemp[] dailyAverageTempArray = measurements
                .Where(m => m.City == city && m.Record.Temperature != null)
                .GroupBy(m => m.Date.Date)
                .Where(g => g.Key >= earliestStartOfSpringOrSummer)
                .Select(g => new DailyTemp
                {
                    Date = g.Key,
                    AverageTemp = (double)g.Average(m => m.Record.Temperature)
                })
                .ToArray();

            double startTemp = 0.0;

            switch (season.ToLower())
            {
                case "vår":
                    startTemp = 0.0;
                    break;
                case "sommar":
                    startTemp = 10.0;
                    break;
                default:
                    break;
            }

            DateTime seasonStart = new DateTime();

            if (season.ToLower() == "vår")
            {
                for (int i = 6; i < dailyAverageTempArray.Length; i++)
                {
                    if (dailyAverageTempArray[i].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 1].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 2].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 3].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 4].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 5].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 6].AverageTemp > startTemp)
                    {
                        seasonStart = dailyAverageTempArray[i - 6].Date;
                        break;
                    }
                }
            }

            if (season.ToLower() == "sommar")
            {
                for (int i = 4; i < dailyAverageTempArray.Length; i++)
                {
                    if (dailyAverageTempArray[i].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 1].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 2].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 3].AverageTemp > startTemp &&
                        dailyAverageTempArray[i - 4].AverageTemp > startTemp)
                    {
                        seasonStart = dailyAverageTempArray[i - 4].Date;
                        break;
                    }
                }
            }

            return seasonStart;
        }
    }
}
