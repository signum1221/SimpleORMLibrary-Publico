using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleORMLibrary
{
    public class BsonIdManager
    {
        //Id de mongo
        public ObjectId _id { get; set; }

        //Generar id de mongo
        public BsonIdManager()
        {
            _id = ObjectId.GenerateNewId();
        }
    }
}
