using Microsoft.EntityFrameworkCore;
using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class CharactersViewModel : ObservableObject, IUpdatable
    {
        public event EventHandler CharacterRemove;

        public RelayCommand AddCharacterCommand { get; set; }
        public RelayCommand RemoveCharacterCommand { get; set; }

        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }

        private PagingCollectionView pagingCharacters;
        public PagingCollectionView PagingCharacters
        {
            get { return pagingCharacters; }
            set { pagingCharacters = value; OnPropertyChanged(nameof(PagingCharacters)); }
        }

        private int itemsOnPage = 15;
        public int ItemsOnPage
        {
            get { return itemsOnPage; }
            set { itemsOnPage = value; _ = ResetPaginCharactersAsync(); OnPropertyChanged(nameof(ItemsOnPage)); }
        }
        public List<int> ItemsPerPageOptions { get; } = [1, 15, 30, 50, 100];

        private string nameFilter = string.Empty;
        public string NameFilter
        {
            get { return nameFilter; }
            set { nameFilter = value; OnPropertyChanged(nameof(NameFilter)); }
        }

        private int idFilter = 0;
        public int IdFilter
        {
            get { return idFilter; }
            set { idFilter = value; OnPropertyChanged(nameof(IdFilter)); }
        }

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand ClearFilterCommand { get; set; }


        public CharactersViewModel()
        {
            _ = ResetPaginCharactersAsync();

            NextPageCommand = new(o =>
            {
                PagingCharacters.MoveToNextPage();
            });

            PrevPageCommand = new(o =>
            {
                PagingCharacters.MoveToPreviousPage();
            });

            AddCharacterCommand = new(ExecuteAddCharacter);
            RemoveCharacterCommand = new(ExecuteRemoveCharacter);


            FilterCommand = new(ExecuteFilter);
            ClearFilterCommand = new(ExecuteClearFilter);
        }

        private void ExecuteAddCharacter(object parameter)
        {
            var window = new AddCharacterWindow(AddEventHandler, (Character)parameter);
            window.Show();
        }

        private async void ExecuteRemoveCharacter(object parameter)
        {
            if (parameter is Character character)
            {
                await Task.Run(async() =>
                {
                    using ShopContext context = new();

                    var characterDb = context.Characters
                        .Include(c => c.Categories)
                        .Include(c => c.CharacterValues)
                        .FirstOrDefault(c => c.Id == character.Id);

                    foreach (var product in context.Products.Include(p => p.CharacterValues))
                    {
                        var valuesToRemove = product.CharacterValues
                            .Where(cv => cv.Character.Id == character.Id)
                            .ToList();

                        foreach (var value in valuesToRemove)
                        {
                            product.CharacterValues.Remove(value);
                        }
                    }

                    context.Characters.Remove(characterDb);
                    context.SaveChanges();

                    await ResetPaginCharactersAsync();

                    CharacterRemove?.Invoke(this, null);
                });
            }
        }

        private async void ExecuteClearFilter(object objItem)
        {
            NameFilter = string.Empty;
            IdFilter = 0;

            await ResetPaginCharactersAsync();
        }

        private async void ExecuteFilter(object objItem)
        {
            await ResetPaginCharactersAsync();

        }

        private async Task ResetPaginCharactersAsync()
        {
            await Task.Run(() =>
            {
                using ShopContext context = new();

                var characters = context.Characters.Include(c => c.Categories).ToList()
                     .Where(c =>
                     {
                         return c.Name.ToLower().Contains(NameFilter.ToLower()) &&
                             (IdFilter == 0) ? true : IdFilter == c.Id;
                     })
                     .OrderByDescending(p => p.Id).ToList();

                PagingCharacters = new(characters, ItemsOnPage);
            });
        }

        public async void AddEventHandler(object sender, EventArgs args)
        {
            await ResetPaginCharactersAsync();
        }

        public async Task Update()
        {
            await ResetPaginCharactersAsync();
        }
    }
}
