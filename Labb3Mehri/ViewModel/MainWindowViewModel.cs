using Labb3Mehri.Command;
using Labb3Mehri.Dialogs;
using Labb3Mehri.Model;
using Labb3Mehri.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Windows;

namespace Labb3Mehri.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
       
        private ObservableCollection<QuestionPackViewModel> _packs;

        public ObservableCollection<QuestionPackViewModel> Packs
        {
            get => _packs;
            set 
            { 
                _packs = value;
                _packs.CollectionChanged += Packs_CollectionChanged;
                RaisePropertyChanged();
                RaisePropertyChanged("IsCreateQuestionPackEnabled");
                DeleteActivePackCommand?.RaiseCanExecuteChanged();
            }
        }
        private void Packs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("IsCreateQuestionPackEnabled");
            DeleteActivePackCommand?.RaiseCanExecuteChanged();
        }

        private bool _isInFullscreen;

        public bool IsInFullscreen
        {
            get => _isInFullscreen;
            set
            {
                _isInFullscreen = value;
                RaisePropertyChanged();
               
            }
        }

        public ConfigurationViewModel ConfigurationViewModel { get;} 
        public PlayerViewModel PlayerViewModel { get; } 
        public DelegateCommand ShowCreateNewQuestionPackCommand { get; }
        public DelegateCommand DeleteActivePackCommand { get; }
        public DelegateCommand SelectActivePackCommand { get; }
        public DelegateCommand ShowPlayerViewCommand { get; }
        public DelegateCommand ShowConfigurationViewCommand { get; }
        public DelegateCommand CloseAppCommand { get; }
        public DelegateCommand FullscreenCommand { get; }

        private QuestionPackViewModel? _selectedPack;

        public QuestionPackViewModel? SelectedPack
        {
            get => _selectedPack;
            set 
            { 
                _selectedPack = value;
                RaisePropertyChanged();
            }
        }

        public string FilePath { get; set; }
        public string FolderPath { get; set; }

        private CreateNewPackDialog _newPackDialog;

        public CreateNewPackDialog NewPackDialog
        {
            get => _newPackDialog;
            set 
            { 
                _newPackDialog = value;
            }
        }

        public DelegateCommand CloseCreateNewQuestionPackCommand { get; }
        public DelegateCommand AddNewPackCommand { get; }

        private QuestionPackViewModel? _activePack; 
		public QuestionPackViewModel? ActivePack 
		{
			get => _activePack;
			set 
			{
				_activePack = value;
				RaisePropertyChanged();
                ConfigurationViewModel?.RaisePropertyChanged("ActivePack");
			}
		}

        private QuestionPackViewModel? _newPack;

        public QuestionPackViewModel? NewPack
        {
            get => _newPack;
            set 
            { 
                _newPack = value;
                RaisePropertyChanged();
            }
        }


        public MainWindowViewModel() 
        {
            IsInFullscreen = false;

            FilePath = GetFilePath();
            FolderPath = GetFolderPath();
            LoadJson();

            ConfigurationViewModel = new ConfigurationViewModel(this);
            PlayerViewModel = new PlayerViewModel(this);
            AddNewPackCommand = new DelegateCommand(AddNewPack);
            ShowCreateNewQuestionPackCommand = new DelegateCommand(ShowCreateNewQuestionPack);
            CloseCreateNewQuestionPackCommand = new DelegateCommand(CloseCreateNewQuestionPack);
            SelectActivePackCommand = new DelegateCommand(SelectActivePack);
            DeleteActivePackCommand = new DelegateCommand(DeleteActivePack, IsDeletePackEnabled);
            ShowPlayerViewCommand = new DelegateCommand(ShowPlayerView, IsPlayButtonEnabled);
            ShowConfigurationViewCommand = new DelegateCommand(ShowConfigurationView, IsEditButtonEnabled);
            CloseAppCommand = new DelegateCommand(CloseApp);
            FullscreenCommand = new DelegateCommand(Fullscreen);
        }

        private void Fullscreen(object obj)
        {

            IsInFullscreen = !IsInFullscreen;
        
        }

        private async void CloseApp(object obj)
        {
            await SaveJson();
            Application.Current.Shutdown();   
        }

        private async Task LoadJson()
        {
            Packs = new ObservableCollection<QuestionPackViewModel>();

            if (File.Exists(FilePath))
            {
                await ReadJson();
                ActivePack = Packs.FirstOrDefault();
            }
            else
            {
                ActivePack = new QuestionPackViewModel(new QuestionPack("Default Question Pack"));
                Packs.Add(ActivePack);
            }
        }

        private async Task ReadJson()
        {
            Directory.CreateDirectory(FolderPath);

            var jsonString = await File.ReadAllTextAsync(FilePath);
            var packs = JsonSerializer.Deserialize<QuestionPack[]>(jsonString);

            foreach (var pack in packs)
            {
                Packs.Add(new QuestionPackViewModel(pack));
            }

        }
        public async Task SaveJson()
        {

            Directory.CreateDirectory(FolderPath);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true
            }; 

            var jsonString = JsonSerializer.Serialize(Packs, options);

            await File.WriteAllTextAsync(FilePath, jsonString);
        }
        private string? GetFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolderName = "QuizAppMehri";
            string fileName = "mehrisquiz.json";
            string fullFolderPath = Path.Combine(appDataPath, appFolderName);
            string fullFilePath = Path.Combine(fullFolderPath, fileName);
            

            return fullFilePath;
        }

        private string? GetFolderPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolderName = "QuizAppMehri";
            string fullFolderPath = Path.Combine(appDataPath, appFolderName);

            return fullFolderPath;

        }

        private bool IsEditButtonEnabled(object? arg)
        {
            if (PlayerViewModel.PlayerViewVisibility is Visibility.Visible)
            {
                return true;
            }
            else if (PlayerViewModel.ResultVisibility is Visibility.Visible)
            {
                return true;
            }
            else return false;
        }

        private void ShowConfigurationView(object obj)
        {
            PlayerViewModel.PlayerViewVisibility = Visibility.Hidden;
            PlayerViewModel.ResultVisibility = Visibility.Hidden;
            ConfigurationViewModel.ConfigurationViewVisibility = Visibility.Visible;
            RaisePropertyChanged("IsEditButtonEnabled");
            ShowConfigurationViewCommand.RaiseCanExecuteChanged();
            ShowPlayerViewCommand.RaiseCanExecuteChanged();
        }

        private bool IsPlayButtonEnabled(object? arg)
        {
            
            if ( ConfigurationViewModel.ConfigurationViewVisibility is Visibility.Visible && ActivePack.Questions.Count >= 1) return true;
            else return false;
        }

        private void ShowPlayerView(object obj)
        {
            PlayerViewModel.PlayerViewVisibility = Visibility.Visible;
            ConfigurationViewModel.ConfigurationViewVisibility = Visibility.Hidden;
            RaisePropertyChanged("IsPlayButtonEnabled");
            ShowPlayerViewCommand.RaiseCanExecuteChanged();
            ShowConfigurationViewCommand.RaiseCanExecuteChanged();
            PlayerViewModel.Run();
        }

        private bool IsDeletePackEnabled(object? arg)
        {
            if (Packs.Count > 1) return true;
            else return false;
        }

        private void DeleteActivePack(object obj) 
        {
            Packs.Remove(ActivePack);
            ActivePack = Packs.FirstOrDefault();
        }

        private void SelectActivePack(object obj)
        {
            if (obj is QuestionPackViewModel pack)
            {
                SelectedPack = pack;
                ActivePack = pack;
            }
        }

        private void CloseCreateNewQuestionPack(object obj)
        {
            NewPackDialog.Close();
        }

        private void AddNewPack(object obj)
        {
            Packs.Add(NewPack);
            ActivePack = NewPack;
            NewPackDialog.Close();
        }

        private void ShowCreateNewQuestionPack(object obj)
        {
            NewPack = new QuestionPackViewModel(new QuestionPack("New Question Pack"));
            ShowCreateNewQuestionPackCommand.RaiseCanExecuteChanged();
            NewPackDialog = new CreateNewPackDialog();
            NewPackDialog.ShowDialog();
        }

        
    }
}
