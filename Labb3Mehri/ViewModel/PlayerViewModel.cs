using Labb3Mehri.Command;
using Labb3Mehri.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Labb3Mehri.ViewModel
{
    internal class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? mainWindowViewModel;

        public DelegateCommand CheckAnswerCommand { get; }
        public DelegateCommand ShowQuestionCountCommand { get; }
        public DelegateCommand RestartQuizCommand { get; }

        private ObservableCollection<Question> _questions;

        public ObservableCollection<Question> Questions
        {
            get 
            {
                if (_questions == null || !_questions.Any())
                {
                    _questions = new ObservableCollection<Question>(ActivePack.Questions);
                }
                return _questions;
            }
            set 
            {
                _questions = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> _answerAlternatives;

        public ObservableCollection<string> AnswerAlternatives 
        {
            get => _answerAlternatives;
            set 
            { 
                _answerAlternatives = value;
                RaisePropertyChanged();
            
            }
        }

        private ObservableCollection<Brush> _buttonColors;

        public ObservableCollection<Brush> ButtonColors
        {
            get => _buttonColors;
            set 
            {
                _buttonColors = value;
                RaisePropertyChanged();
            }
        }



        private Question _currentQuestion;

        public Question CurrentQuestion
        {
            get { return _currentQuestion; }
            set 
            { 
                _currentQuestion = value;
                RaisePropertyChanged();
            }
        }

        public QuestionPackViewModel? ActivePack { get => mainWindowViewModel.ActivePack; }

        private DispatcherTimer timer;

        private int tick;

        public int Tick
        {
            get => tick; 
            set 
            {
                tick = value;
                RaisePropertyChanged();
            }
        }

     
        private Visibility playerViewVisibility;

        public Visibility PlayerViewVisibility
        {
            get { return playerViewVisibility; }
            set
            {
                playerViewVisibility = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _resultVisibility;

        public Visibility ResultVisibility
        {
            get { return _resultVisibility; }
            set
            {
                _resultVisibility = value;
                RaisePropertyChanged();
            }
        }


        private int _currentQuestionIndex;

        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                RaisePropertyChanged("ShowQuestionCount");
            }
        }

        private int _totalAmountofQuestions;

        public int TotalAmountOfQuestions
        {
            get => _totalAmountofQuestions;
            set 
            { 
                _totalAmountofQuestions = value;
                RaisePropertyChanged("ShowQuestionCount");
                RaisePropertyChanged("ResultText");
            }
        }

        private int _totalCorrectAnswers;

        public int TotalCorrectAnswers
        {
            get => _totalCorrectAnswers;
            set 
            { 
                _totalCorrectAnswers = value;
                RaisePropertyChanged("ResultText");
            }
        }

        public string ResultText
        {
            get { return $"Du fick {TotalCorrectAnswers} av {TotalAmountOfQuestions} rätt!"; }
        }

        public string ShowQuestionCount
        {
            get { return $"Fråga {CurrentQuestionIndex} of {TotalAmountOfQuestions}"; }
        }



        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel) 
        {
            this.mainWindowViewModel = mainWindowViewModel;
            PlayerViewVisibility = Visibility.Hidden;
            ResultVisibility = Visibility.Hidden;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Countdown;
            AnswerAlternatives = new ObservableCollection<string>() { "", "", "", "" };
            CurrentQuestionIndex = 0;
            CheckAnswerCommand = new DelegateCommand(CheckAnswer);
            RestartQuizCommand = new DelegateCommand(RestartQuiz);
            ButtonColors = new ObservableCollection<Brush>
            {
                Brushes.LightGray,
                Brushes.LightGray,
                Brushes.LightGray,
                Brushes.LightGray
            };
        }


        public void Run()
        {
            ResetQuiz();

            LoadNextQuestion();
        }
        private void RestartQuiz(object obj)
        {
            ResultVisibility = Visibility.Hidden;
            PlayerViewVisibility = Visibility.Visible;
            Run();
        }

        

        private async void CheckAnswer(object obj)
        {
            string selectedAnswer = obj as string;

            int selectedIndex = 0; 

            for (int i = 0; i < AnswerAlternatives.Count; i++)
            {
                if (AnswerAlternatives[i] == selectedAnswer)
                {
                    selectedIndex = i;
                    break;
                }
            }

            int correctIndex = 0; 

            for (int i = 0; i < AnswerAlternatives.Count; i++)
            {
                if (AnswerAlternatives[i] == CurrentQuestion.CorrectAnswer)
                {
                    correctIndex = i;
                    break;
                }
            }

            if (selectedAnswer == CurrentQuestion.CorrectAnswer)
            {
                ButtonColors[selectedIndex] = Brushes.Green;
                timer.Stop();
                TotalCorrectAnswers++;
                await Task.Delay(2000);
                LoadNextQuestion();
            }
            else 
            {
                ButtonColors[selectedIndex] = Brushes.Red;
                ButtonColors[correctIndex] = Brushes.Green; 
                timer.Stop();
                await Task.Delay(2000);
                LoadNextQuestion();
            }
        }


        private void LoadNextQuestion()
        {
            if (CurrentQuestionIndex < Questions.Count)
            {
                TotalAmountOfQuestions = Questions.Count;
                Tick = ActivePack.TimeLimitSeconds;
                timer.Start();
                CurrentQuestion = Questions[CurrentQuestionIndex];
                LoadQuestion();
                CurrentQuestionIndex++;
            }
            else
            {
                ShowResults();
            }
        }
        private void LoadQuestion()
        {
            for (int i = 0; i < ButtonColors.Count; i++)
            {
                ButtonColors[i] = Brushes.LightGray;
            }

            var answersInQuestion = new List<string> { CurrentQuestion.CorrectAnswer };
            answersInQuestion.AddRange(CurrentQuestion.IncorrectAnswers);

            Shuffle(answersInQuestion);

            for (int i = 0; i < AnswerAlternatives.Count; i++)
            {
                AnswerAlternatives[i] = answersInQuestion[i];
            }
        }

        private void ShowResults()
        {
            PlayerViewVisibility = Visibility.Hidden;
            ResultVisibility = Visibility.Visible;
            mainWindowViewModel.RaisePropertyChanged("IsEditButtonEnabled");
            mainWindowViewModel.ShowConfigurationViewCommand.RaiseCanExecuteChanged();
        }

        private void ResetQuiz()
        {
            CurrentQuestionIndex = 0;

            TotalCorrectAnswers = 0;


            for (int i = 0; i < ButtonColors.Count; i++)
            {
                ButtonColors[i] = Brushes.LightGray;
            }

            Questions = new ObservableCollection<Question>(ActivePack.Questions);

            Shuffle(Questions);

        }

        private void Countdown(object? sender, EventArgs e)
        {
            Tick--;
            RaisePropertyChanged("Tick");

            if (Tick == 0)
            {
                timer.Stop();
                LoadNextQuestion();
            }
        }

        private void Shuffle<T>(ObservableCollection<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void Shuffle<T>(List<T> list)
        {
            Random rnd = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    
        
       
    }
}
