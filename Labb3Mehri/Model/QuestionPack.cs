using Labb3Mehri.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    
namespace Labb3Mehri.Model
{
    enum Difficulty { Easy, Medium, Hard}
    internal class QuestionPack
    {
        public QuestionPack()
        {
            Questions = new List<Question>();
        }
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitSeconds = 30, ObjectId id = new ObjectId(), Category category = null) //konstruktor
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitSeconds = timeLimitSeconds;
            Questions = new List<Question>();
            Id = id = ObjectId.GenerateNewId();
            CategoryId = category?.Id;
            Category = category;
        }

        [BsonId]
        public ObjectId Id { get; set; } 

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Difficulty")]
        public Difficulty Difficulty { get; set; }

        [BsonElement("TimeLimitSeconds")]
        public int TimeLimitSeconds { get; set; }

        [BsonElement("Questions")]
        public List<Question> Questions { get; set; }

        [BsonElement("CategoryId")]
        public ObjectId? CategoryId { get; set; }

        [BsonIgnore]
        public Category Category { get; set; }

    }
}
