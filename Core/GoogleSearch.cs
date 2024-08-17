using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shop.MVVM.Model;
using WinRT;
using Xceed.Wpf.Toolkit;

namespace Shop.Core
{
    public class SearchResultItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Snippet { get; set; }
        public ImageData Image { get; set; }
    }

    public class ImageData
    {
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class SearchResults
    {
        public List<SearchResultItem> Items { get; set; }
        public string Kind { get; set; }
    }

    public class GoogleSearch(Product product)
    {
        public Product Product { get; set; } = product;
        private readonly List<byte[]> images = [];

        public async Task<List<byte[]>> FindImagesAsync()
        {
            await FindImages(Product.Code + " " + Product.Brand + " " + Product.Name);

            return images;
        }


        private async Task FindImages(string query)
        {
            await Task.Run( async () =>
            {
                string apiKey = "AIzaSyBE-hku6BtE6vq82wAT1uonFofh_6BfcYg";
                string cx = "865fea8f1117b4680";

                using HttpClient httpClient = new();
                try
                {
                    string apiUrl = $"https://www.googleapis.com/customsearch/v1?q={query}&key={apiKey}&cx={cx}&searchType=image";
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        await Console.Out.WriteLineAsync(json);

                        SearchResults searchResults = JsonConvert.DeserializeObject<SearchResults>(json);

                        if (searchResults.Items != null && searchResults.Kind == "customsearch#search")
                        {
                            foreach (var item in from result in searchResults.Items  select result)
                            {
                                using HttpClient imageClient = new();
                                byte[] imageBytes = await imageClient.GetByteArrayAsync(item.Link);
                                images.Add(imageBytes);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return;
                }

                return;
            });
        }
    }
}
