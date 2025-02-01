using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3Mehri.ViewModel
{
    internal class Category : ViewModelBase
    {
        [BsonId]
        public ObjectId Id { get; set; }

        private string _name;
        [BsonElement]
        public string Name
        {
            get => _name;
            set 
            {
                _name = value;
                RaisePropertyChanged();
            }
        }
    

        public Category(string name, ObjectId id = new ObjectId()) //konstruktor
        {
            Name = name;
            Id = id = ObjectId.GenerateNewId();
        }
    }
}
