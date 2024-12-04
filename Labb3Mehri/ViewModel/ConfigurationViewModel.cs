using Labb3Mehri.Command;
using Labb3Mehri.Dialogs;
using Labb3Mehri.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Labb3Mehri.ViewModel
{
    internal class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;
        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack;}
        public DelegateCommand AddQuestionCommand { get; }
        public DelegateCommand RemoveQuestionCommand { get; }
        public DelegateCommand ShowPackOptionsCommand { get; }

        private Question? _activeQuestion;

        public Question? ActiveQuestion
        {
            get => _activeQuestion;
            set
            {
                _activeQuestion = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsEnabled");
                RemoveQuestionCommand?.RaiseCanExecuteChanged();
            }
        }

        public bool isEnabled
        {
            get => ActiveQuestion is not null;
        }


        private Visibility configurationViewVisibility;

        public Visibility ConfigurationViewVisibility
        {
            get { return configurationViewVisibility; }
            set { configurationViewVisibility = value;
                RaisePropertyChanged();
            }
        }



        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel) 
        {
            this.mainWindowViewModel = mainWindowViewModel;

            AddQuestionCommand = new DelegateCommand(AddQuestion);

            RemoveQuestionCommand = new DelegateCommand(RemoveQuestion, IsRemovebuttonEnabled);

            ShowPackOptionsCommand = new DelegateCommand(ShowPackOptions);

            ActiveQuestion = ActivePack?.Questions.FirstOrDefault();

        }

       

        private void ShowPackOptions(object obj)
        {
            ShowPackOptionsCommand.RaiseCanExecuteChanged();
            PackOptionsDialog packOptionsDialog = new();
            packOptionsDialog.ShowDialog();
            
        }

        private bool IsRemovebuttonEnabled (object? arg)
        {
            if (ActiveQuestion != null) return true;
            else return false;
        }
        private void RemoveQuestion(object obj)
        {
            ActivePack.Questions.Remove(ActiveQuestion);
            RemoveQuestionCommand.RaiseCanExecuteChanged();
            mainWindowViewModel.ShowPlayerViewCommand.RaiseCanExecuteChanged();

        }

        private void AddQuestion(object obj)
        {

            ActivePack.Questions.Add(new Question("New Question", string.Empty, string.Empty, string.Empty, string.Empty));
            ActiveQuestion = ActivePack.Questions.LastOrDefault();
            AddQuestionCommand.RaiseCanExecuteChanged();
            mainWindowViewModel.ShowPlayerViewCommand.RaiseCanExecuteChanged();

        }
        
    }
}
