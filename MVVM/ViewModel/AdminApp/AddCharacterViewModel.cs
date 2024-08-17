using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class AddCharacterViewModel : ObservableObject
    {
        public event EventHandler CharacterAddedEvent;

        public Character Current { get; set; }

        public RelayCommand AddCharacterCommand { get; set; }

        public ShopContext Context { get; set; }

        private string headerText { get; set; }
        private string buttonText { get; set; }
        public string HeaderText
        {
            get { return headerText; }
            set { headerText = value; OnPropertyChanged(nameof(HeaderText)); }
        }
        public string ButtonText
        {
            get { return buttonText; }
            set { buttonText = value; OnPropertyChanged(nameof(ButtonText)); }
        }
        public bool IsNew { get; set; }

        public AddCharacterViewModel(Character character)
        {
            IsNew = character is null;
            Context = new ShopContext();

            InitializeProperties(character);

            AddCharacterCommand = new(ExecuteAddCharacter);
        }

        private void InitializeProperties(Character character)
        {
            if (IsNew)
            {
                HeaderText = "Добавление характеристики";
                ButtonText = "Добавить характеристику";
                Current = new Character
                {
                    Name = "test character"
                };
            }
            else
            {
                HeaderText = "Редактирование характеристики";
                ButtonText = "Изменить характеристику";
                Current = Context.Characters.FirstOrDefault(c => c.Id == character.Id);
            }
        }

        private void ExecuteAddCharacter(object parameter)
        {
            if (!Current.ValidateCharacter())
            {
                return;
            }

            if (IsNew)
            {
                Context.Add(Current);
            }

            Context.SaveChanges();
            Context.Dispose();
            OnCharacterAdded();
        }

        protected virtual void OnCharacterAdded()
        {
            CharacterAddedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
