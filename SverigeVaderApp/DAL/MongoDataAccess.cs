using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using SverigeVaderApp.Models;
using System;
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
    }
}
