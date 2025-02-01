using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3Mehri.Model
{
    internal class Question
    {

        public Question()
        {
            IncorrectAnswers = new string[3];
        }
        public Question(string query, string correctAnswer, string incorrectAnswer1, string incorrectAnswser2, string incorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = new string[3] { incorrectAnswer1, incorrectAnswser2, incorrectAnswer3 };
        }
        [BsonElement("Query")]
        public string Query { get; set; }

        [BsonElement("CorrectAnswer")]
        public string CorrectAnswer { get; set; }

        [BsonElement("IncorrectAnswers")]
        public string[] IncorrectAnswers { get; set; }

    }
}
