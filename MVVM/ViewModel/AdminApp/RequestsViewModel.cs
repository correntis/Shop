using Microsoft.EntityFrameworkCore;
using Shop.AdminApp;
using Shop.Core;
using Shop.DB;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.MVVM.ViewModel.AdminApp
{
    public class RequestsViewModel : ObservableObject, IUpdatable
    {
        public event EventHandler CharacterRemove;

        public RelayCommand ComlepeteRequestStatusCommand { get; set; }
        public RelayCommand ProcessRequestStatusCommand { get; set; }

        public RelayCommand NextPageCommand { get; set; }
        public RelayCommand PrevPageCommand { get; set; }

        private PagingCollectionView pagingRequests;
        public PagingCollectionView PagingRequests
        {
            get { return pagingRequests; }
            set { pagingRequests = value; OnPropertyChanged(nameof(PagingRequests)); }
        }

        private int itemsOnPage = 15;
        public int ItemsOnPage
        {
            get { return itemsOnPage; }
            set { itemsOnPage = value; _ = ResetPagingRequestsAsync(); OnPropertyChanged(nameof(ItemsOnPage)); }
        }
        public List<int> ItemsPerPageOptions { get; } = [1, 15, 30, 50, 100];

        private RequestStatus statusFilter = RequestStatus.All;
        public RequestStatus StatusFilter
        {
            get { return statusFilter; }
            set { statusFilter = value; OnPropertyChanged(nameof(StatusFilter)); }
        }
        public List<RequestStatus> StatusOptions { get; } = [RequestStatus.All, RequestStatus.Unread, RequestStatus.Processing, RequestStatus.Completed];

        public RelayCommand FilterCommand { get; set; }
        public RelayCommand ClearFilterCommand { get; set; }


        public RequestsViewModel()
        {
            _ = ResetPagingRequestsAsync();

            NextPageCommand = new(o =>
            {
                PagingRequests.MoveToNextPage();
            });

            PrevPageCommand = new(o =>
            {
                PagingRequests.MoveToPreviousPage();
            });

            ComlepeteRequestStatusCommand = new(ExecuteCompleteRequestStatus);
            ProcessRequestStatusCommand = new(ExecuteProcessRequestStatus);

            FilterCommand = new(ExecuteFilter);
            ClearFilterCommand = new(ExecuteClearFilter);
        }

        private async void ExecuteCompleteRequestStatus(object parameter)
        {
            if (parameter is Request req)
            {
                using ShopContext context = new();

                var request = context.Requests.FirstOrDefault(r => r.Id == req.Id);

                if (request is not null)
                {
                    request.Status = ((int)RequestStatus.Completed);
                    context.SaveChanges();
                    await ResetPagingRequestsAsync();
                }
            }
        }

        private async void ExecuteProcessRequestStatus(object parameter)
        {
            if (parameter is Request req)
            {
                using ShopContext context = new();

                var request = context.Requests.FirstOrDefault(r => r.Id == req.Id);

                if (request is not null)
                {
                    request.Status = ((int)RequestStatus.Processing);
                    context.SaveChanges();
                    await ResetPagingRequestsAsync();
                }
            }
        }

        private async void ExecuteClearFilter(object objItem)
        {
            StatusFilter = RequestStatus.All;

            await ResetPagingRequestsAsync();
        }

        private async void ExecuteFilter(object objItem)
        {
            await ResetPagingRequestsAsync();
        }

        private async Task ResetPagingRequestsAsync()
        {
            await Task.Run( async () =>
            {
                using ShopContext context = new();

                var requests = context.Requests
                    .Include(r => r.User)
                    .Include(r => r.Product)
                    .ToList()
                    .Where(r =>
                    {
                        return StatusFilter == RequestStatus.All || r.Status == (int)statusFilter;
                    })
                    .OrderByDescending(r => r.Date)
                    .ToList();

                foreach (var req in requests)
                {
                    if (req.Status == (int)RequestStatus.Processing || req.Status == (int)RequestStatus.Unread)
                    {
                        if (req.Product.Quantity > 0)
                        {
                            req.Status = (int)RequestStatus.Completed;
                        }
                    }
                }

                await context.SaveChangesAsync();

                PagingRequests = new(requests, ItemsOnPage);
            });
        }

        public async void AddEventHandler(object sender, EventArgs args)
        {
            await ResetPagingRequestsAsync();
        }

        public async Task Update()
        {
            await ResetPagingRequestsAsync();
        }
    }
}
