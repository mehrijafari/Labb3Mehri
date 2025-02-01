using Labb3Mehri.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.ObjectModel;

namespace Labb3Mehri.ViewModel
{
    internal class QuestionPackViewModel : ViewModelBase
    {
        private readonly QuestionPack model;


        public ObjectId? CategoryId
        {
            get => model.CategoryId;
            set
            {
                model.CategoryId = value;
                RaisePropertyChanged();
            }
        }

        public Category Category
        {
            get => model.Category;
            set
            {
                model.Category = value;
                model.CategoryId = value?.Id;
                RaisePropertyChanged();
            }
        }

        public ObjectId ID => model.Id;
        
        public string Name
        { 
            get => model.Name;
            set
            {
                model.Name = value;
                RaisePropertyChanged();
            }
        }
        public Difficulty Difficulty
        {
            get => model.Difficulty;
            set
            {
                model.Difficulty = value;
                RaisePropertyChanged();
            }
        }
        public int TimeLimitSeconds
        {
            get => model.TimeLimitSeconds;
            set
            {
                model.TimeLimitSeconds = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<Question> Questions { get; }
        public object Model { get; internal set; }
        public QuestionPackViewModel(QuestionPack model) //konstruktor
        {
            this.model = model;
            this.Questions = new ObservableCollection<Question>(model.Questions);
        }
    }
}
