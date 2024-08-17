using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class CategoriesViewModel : ObservableObject, IUpdatable
    {
        public event EventHandler CategoryRemove;

        public RelayCommand AddCategoryCommand { get; set; }
        public RelayCommand RemoveCategoryCommand { get; set; }

        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }
            
        private PagingCollectionView pagingCategories;
        public PagingCollectionView PagingCategories
        {
            get { return pagingCategories; }
            set { pagingCategories = value; OnPropertyChanged(nameof(PagingCategories)); }
        }

        private int itemsOnPage = 15;
        public int ItemsOnPage
        {
            get { return itemsOnPage; }
            set { itemsOnPage = value; _ = ResetPaginCategoriesAsync(); OnPropertyChanged(nameof(ItemsOnPage)); }
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


        public CategoriesViewModel()
        {
            _ = ResetPaginCategoriesAsync();

            NextPageCommand = new(o =>
            {
                PagingCategories.MoveToNextPage();
            });

            PrevPageCommand = new(o =>
            {
                PagingCategories.MoveToPreviousPage();
            });

            AddCategoryCommand = new(ExecuteAddCategory);
            RemoveCategoryCommand = new(ExecuteRemoveCategory);

            FilterCommand = new(ExecuteFilter);
            ClearFilterCommand = new(ExecuteClearFilter);
        }

        private void ExecuteAddCategory(object parameter)
        {
            var window = new AddCategoryWindow(AddEventHandler, (Category)parameter);
            window.Show();
        }

        //private async void ExecuteRemoveCategory(object parameter)
        //{
        //    if (parameter is Category category)
        //    {
        //        using ShopContext context = new();

        //        var categoryDb = context.Categories
        //            .Include(c => c.Characters)
        //            .Include(c => c.Products)
        //            .FirstOrDefault(c => c.Id == category.Id);

        //        foreach (var product in context.Products.Include(p => p.CharacterValues))
        //        {
        //            if (product.Categories.Any(c => c.Id == categoryDb.Id))
        //            {
        //                var valuesToRemove = product.CharacterValues
        //                .Where(cv => cv.Character.Categories.Any(cat => cat.Id == categoryDb.Id))
        //                .ToList();

        //                foreach (var value in valuesToRemove)
        //                {
        //                    product.CharacterValues.Remove(value);
        //                }
        //            }
        //        }
        //        context.Categories.Remove(categoryDb);
        //        context.SaveChanges();
        //        context.Dispose();

        //        await ResetPaginCategoriesAsync();

        //        CategoryRemove?.Invoke(this, null);
        //    }
        //}

        private async void ExecuteRemoveCategory(object parameter)
        {
            if (parameter is Category category)
            {
                await Task.Run( async () =>
                {
                    using ShopContext context = new();

                    var categoryDb = context.Categories
                        .Include(c => c.Characters)
                        .Include(c => c.Products)
                        .FirstOrDefault(c => c.Id == category.Id);

                    foreach (var product in context.Products.Include(p => p.CharacterValues))
                    {
                        if (product.Categories.Any(c => c.Id == categoryDb.Id))
                        {
                            var deleteCategoryCharacters = categoryDb.Characters;
                            var productCharacters = product.Categories.Where(c => c.Id != categoryDb.Id).SelectMany(c => c.Characters);

                            if (productCharacters.Any(c => deleteCategoryCharacters.Any(dct => dct.Id == c.Id)))
                            {
                                continue;
                            }

                            var valuesToRemove = product.CharacterValues
                            .Where(cv => cv.Character.Categories.Any(cat => cat.Id == categoryDb.Id))
                            .ToList();

                            foreach (var value in valuesToRemove)
                            {
                                product.CharacterValues.Remove(value);
                            }
                        }
                    }
                    context.Categories.Remove(categoryDb);
                    context.SaveChanges();
                    context.Dispose();

                    await ResetPaginCategoriesAsync();

                    CategoryRemove?.Invoke(this, null);
                });
            }
        }

        private async Task ResetPaginCategoriesAsync()
        {
            await Task.Run(() =>
            {
                using ShopContext context = new();

                var categories = context.Categories.ToList()
                    .Where(c =>
                    {
                        return c.Name.ToLower().Contains(NameFilter.ToLower()) &&
                            (IdFilter == 0) ? true : IdFilter == c.Id;
                    })
                    .OrderByDescending(p => p.Id).ToList();

                PagingCategories = new(categories, ItemsOnPage);
            });
        }

        private async void ExecuteClearFilter(object objItem)
        {
            NameFilter = string.Empty;
            IdFilter = 0;

            await ResetPaginCategoriesAsync();
        }

        private async void ExecuteFilter(object objItem)
        {
            await ResetPaginCategoriesAsync();

        }

        public async void AddEventHandler(object sender, EventArgs args)
        {
            await ResetPaginCategoriesAsync();
        }

        public async Task Update()
        {
            await ResetPaginCategoriesAsync();
        }
    }
}
