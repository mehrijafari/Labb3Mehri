using Labb3Mehri.Command;
using Labb3Mehri.Dialogs;
using Labb3Mehri.Model;
using Labb3Mehri.Views;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

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

        public ConfigurationViewModel ConfigurationViewModel { get; }
        public PlayerViewModel PlayerViewModel { get; }
        public DelegateCommand ShowCreateNewQuestionPackCommand { get; }
        public DelegateCommand DeleteActivePackCommand { get; }
        public DelegateCommand SelectActivePackCommand { get; }
        public DelegateCommand ShowPlayerViewCommand { get; }
        public DelegateCommand ShowConfigurationViewCommand { get; }
        public DelegateCommand DeleteCategoryCommand { get; }
        public DelegateCommand AddCategoryCommand { get; }
        public AddNewCategoryWindow AddNewCategoryWindow { get; set; }
        public DelegateCommand OpenAddNewCategoryCommand { get; }
        public DelegateCommand CloseAddNewCategoryCommand { get; }
        public DelegateCommand CloseAppCommand { get; }
        public DelegateCommand FullscreenCommand { get; }
        public ObservableCollection<Category> Categories { get; set; }

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

        private Category _selectedCategory;

        public Category SelectedCategory
        {
            get => _selectedCategory;
            set 
            {
                _selectedCategory = value;
                RaisePropertyChanged();
                DeleteCategoryCommand.RaiseCanExecuteChanged();
            }
        }
        private Category _newCategory;

        public Category NewCategory
        {
            get => _newCategory;
            set 
            {
                _newCategory = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            IsInFullscreen = false;

            LoadMongoDB();

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
            DeleteCategoryCommand = new DelegateCommand(DeleteCategory, CanDeleteCategory);
            AddCategoryCommand = new DelegateCommand(AddCategory);
            OpenAddNewCategoryCommand = new DelegateCommand(OpenAddNewCategory);
            CloseAddNewCategoryCommand = new DelegateCommand(CloseAddNewCategory);
        }

        private void CloseAddNewCategory(object obj)
        {
            AddNewCategoryWindow.Close();
        }

        private void OpenAddNewCategory(object obj)
        {
            NewCategory = new Category(string.Empty);
            AddNewCategoryWindow = new AddNewCategoryWindow();
            AddNewCategoryWindow.Show();
        }

        private void AddCategory(object obj)
        {
            Categories.Add(NewCategory);

            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("MehriJafari");
            var categoryCollection = database.GetCollection<Category>("Categories");

            if (NewCategory.Name == string.Empty)
            {
                MessageBox.Show("Category name can not be empty");
                return;
            }

            categoryCollection.InsertOne(NewCategory);
            AddNewCategoryWindow.Close();
        }

        private bool CanDeleteCategory(object? arg)
        {
            if (SelectedCategory != null) return true;
            else return false;
        }

        private void DeleteCategory(object obj)
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("MehriJafari");
            var categoryCollection = database.GetCollection<Category>("Categories");

            var filter = Builders<Category>.Filter.Eq(c => c.Id, SelectedCategory.Id);
            categoryCollection.DeleteOne(filter);

            Categories.Remove(SelectedCategory);
        }

        private void SaveMongoDB()
        {
            var client = new MongoClient("mongodb://localhost:27017/");
            var database = client.GetDatabase("MehriJafari");
            var questionPackCollection = database.GetCollection<QuestionPack>("QuestionPacks");

            foreach (var pack in Packs)
            {
                var filter = Builders<QuestionPack>.Filter.Eq("_id", pack.ID);

                var newPack = new QuestionPack(pack.Name)
                {
                    Id = pack.ID,
                    Difficulty = pack.Difficulty,
                    TimeLimitSeconds = pack.TimeLimitSeconds,
                    Questions = pack.Questions.ToList(),
                    CategoryId = pack.CategoryId,
                    Category = pack.Category
                };

                questionPackCollection.ReplaceOne
                    (
                        filter,
                        newPack,
                        new ReplaceOptions { IsUpsert = true }
                    );

            }
        }
        private void LoadMongoDB()
        {

            var client = new MongoClient("mongodb://localhost:27017/"); 
            var database = client.GetDatabase("MehriJafari");

            bool databaseExists = client.ListDatabaseNames().ToList().Contains("MehriJafari");

            if (!databaseExists) //if database does not exist
            {
                var questionPackCollection = database.GetCollection<QuestionPack>("QuestionPacks");
                Packs = new ObservableCollection<QuestionPackViewModel>();

                var categoriesCollection = database.GetCollection<Category>("Categories");

                Categories = new ObservableCollection<Category>();

                var category1 = new Category("History");
                var category2 = new Category("Entertainment");
                var category3 = new Category("Biology");
                Categories.Add(category1);
                Categories.Add(category2);
                Categories.Add(category3);

                categoriesCollection.InsertMany(Categories);

                var defaultPack = new QuestionPack("Default Question Pack")
                {
                    Difficulty = Difficulty.Medium,
                    TimeLimitSeconds = 30,
                    Questions = new List<Question>()
                };

                ActivePack = new QuestionPackViewModel(defaultPack);
                Packs.Add(ActivePack);

                questionPackCollection.InsertOne(defaultPack);


            }
            else //if database exists
            {
                var questionPackCollection = database.GetCollection<QuestionPack>("QuestionPacks");
                var questionPackFilter = Builders<QuestionPack>.Filter.Empty;

                var categoriesCollection = database.GetCollection<Category>("Categories");
                var categoriesFilter = Builders<Category>.Filter.Empty;
                Categories = new ObservableCollection<Category>();

                var categoriesFromCollection = categoriesCollection.Find(categoriesFilter).ToList();

                foreach (var category in categoriesFromCollection)
                {
                    Categories.Add(category);
                }

                Packs = new ObservableCollection<QuestionPackViewModel>();

                var packsFromCollection = questionPackCollection.Find(questionPackFilter).ToList();

                foreach (var pack in packsFromCollection)
                {
                    if (pack.CategoryId.HasValue)
                    {
                        pack.Category = Categories.FirstOrDefault(c => c.Id == pack.CategoryId);
                    }

                    var viewModel = new QuestionPackViewModel(pack);
                    Packs.Add(viewModel);
                }

                ActivePack = Packs.FirstOrDefault();


            }

        }

        private void Fullscreen(object obj)
        {

            IsInFullscreen = !IsInFullscreen;

        }

        private async void CloseApp(object obj)
        {
            SaveMongoDB();
            Application.Current.Shutdown();
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

            if (ConfigurationViewModel.ConfigurationViewVisibility is Visibility.Visible && ActivePack.Questions.Count >= 1) return true;
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
