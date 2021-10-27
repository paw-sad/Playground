using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace MongoChangesApi
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionStringMongo = "mongodb://mongo1:30001,mongo2:30002/replicaSet=rs0";
            var mongoClient = new MongoClient(connectionStringMongo);
            var db = mongoClient.GetDatabase("users-module");
            var users = db.GetCollection<User>("Users");
            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.Default };

            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(change =>
                    change.OperationType == ChangeStreamOperationType.Update &&
                    change.UpdateDescription.UpdatedFields.Contains("Events"));
                        
                //.AppendStage<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>, BsonDocument>(
                //    "{ $addFields : { newField : 'this is an added field!' } }");

            var cursor = users.Watch(options);
            while (true)
            {
                while (cursor.MoveNext() && !cursor.Current.Any()) { }

                foreach (var changeStreamDocument in cursor.Current)
                {
                    Console.WriteLine(changeStreamDocument.ToJson(JsonWriterSettings.Defaults));
                }
            }
            cursor.Dispose();
        }
    }

    internal class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<object> Events { get; set; } = new List<object>();
    }
}
