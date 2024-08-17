using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Shop.DB;
using Shop.MVVM.Model;
using Shop.MVVM.View.UserApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class EditCharactersViewModel : ObservableObject
    {
        public event EventHandler ProductChangedEvent;

        public ShopContext Context { get; set; }

        private bool isLoaded = false;
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; OnPropertyChanged(nameof(IsLoaded)); }
        }

        private Product current;
        public Product Current
        {
            get { return current; }
            set { current = value; OnPropertyChanged(nameof(Current)); }
        }

        private ObservableCollection<Character> characters  = [];
        public ObservableCollection<Character> Characters
        {
            get { return characters; }
            set { characters = value; OnPropertyChanged(nameof(Characters)); }
        }

        private Dictionary<int, ObservableCollection<CharacterValue>> characterValues = [];
        public Dictionary<int, ObservableCollection<CharacterValue>> CharacterValues
        {
            get { return characterValues; }
            set { characterValues = value; OnPropertyChanged(nameof(CharacterValues)); }
        }

        public RelayCommand EditCharactersCommand { get; set; }
        public RelayCommand AddNewCharacterCommand { get; set; }


        public EditCharactersViewModel(Product product)
        {
            Initialize(product);

            EditCharactersCommand = new(ExecuteEditCharacters);
            AddNewCharacterCommand = new(ExecuteAddNewCharacter);
        }

        private async void Initialize(Product product)
        {
            await Task.Run( async () =>
            {
                Context = new ShopContext();

                Current = await Context.FindProductEditCharactersAsync(product.Id);

                var allUniqueCharacters = Current.Categories
                    .SelectMany(category => category.Characters)
                    .GroupBy(c => c.Id)
                    .Select(group => group.First());

                Characters = [.. allUniqueCharacters];

                for (int i = 0; i < Characters.Count; i++)
                {
                    var characterValues = new ObservableCollection<CharacterValue>([.. Current.CharacterValues.Where(cv => cv.CharactersId == Characters[i].Id)]);
                    CharacterValues.Add(Characters[i].Id, characterValues);
                }
                IsLoaded = true;
                OnPropertyChanged(nameof(CharacterValues));
            });
        }


        private void ExecuteEditCharacters(object parameter)
        {
            Current.CharacterValues.Clear();

            foreach (var character in Characters)
            {
                CharacterValues.TryGetValue(character.Id, out ObservableCollection<CharacterValue> values);
                foreach (var characterValue in values)
                {                     
                    Current.CharacterValues.Add(characterValue);
                }
            }

            Context.SaveChanges();
            Context.Dispose();

            OnProductChanged();
        }

        private void ExecuteAddNewCharacter(object parameter)
        {
            if (parameter is Tuple<int, string> tuple)
            {
                var charId = tuple.Item1;
                var charValues = tuple.Item2;

                var newCharacter = new CharacterValue
                {
                    CharactersId = charId,
                    Value = charValues
                };

                Characters.First(c => c.Id == charId).CharacterValues.Add(newCharacter);
                CharacterValues.TryGetValue(charId, out ObservableCollection<CharacterValue> values);
                values.Add(newCharacter);

                Context.SaveChanges();
            }
        }

        protected virtual void OnProductChanged()
        {
            ProductChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
