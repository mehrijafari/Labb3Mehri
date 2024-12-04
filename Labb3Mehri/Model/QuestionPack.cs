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
        public QuestionPack(string name, Difficulty difficulty = Difficulty.Medium, int timeLimitSeconds = 30)
        {
            Name = name;
            Difficulty = difficulty;
            TimeLimitSeconds = timeLimitSeconds;
            Questions = new List<Question>();
        }

        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int TimeLimitSeconds { get; set; }
        public List<Question> Questions { get; set; }

    }
}
