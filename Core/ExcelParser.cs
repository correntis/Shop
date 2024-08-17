using OfficeOpenXml;
using Shop.MVVM;
using Shop.MVVM.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Core
{
    public class ExcelMapping : ObservableObject
    {
        public string propertyName;
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; OnPropertyChanged(PropertyName); }
        }

        public string excelLabel;
        public string ExcelLabel
        {
            get { return excelLabel; }
            set { excelLabel = value; OnPropertyChanged(ExcelLabel); }
        }

        public Func<string, Product, Product> PropertySetter { get; set; }

        public static List<ExcelMapping> GetDefaultMapping()
        {
            return
            [
                new() { PropertyName="Артикул", ExcelLabel = "/Code", PropertySetter = (value, product) => { product.Code = value; return product; } }, //артикул
                new() {PropertyName="Название", ExcelLabel = "/Name", PropertySetter = (value, product) => { product.Name = value; return product; } }, //название
                new() { PropertyName="Бренд", ExcelLabel = "/Brand", PropertySetter = (value, product) => { product.Brand = value; return product; } }, //бренд
                new() { PropertyName="Поставщик", ExcelLabel = "/Producer", PropertySetter = (value, product) => { product.Producer = value; return product; } }, //поставщик
                new() { PropertyName="Страна", ExcelLabel = "/Country", PropertySetter = (value, product) => { product.Country = value; return product; } }, //страна
                new()
                {
                    PropertyName="Цена",
                    ExcelLabel = "/Price",
                    PropertySetter = (value, product) =>
                    {
                        if (!decimal.TryParse(value, out decimal price))
                        {
                            price = decimal.Zero;
                        }
                        product.Price = price;
                        return product;
                    }
                },//цена
                new()
                {
                    PropertyName="Кол-во",
                    ExcelLabel = "/Quantity",
                    PropertySetter = (value, product) =>
                    {
                        if (!int.TryParse(value, out int quantity))
                        {
                            quantity = 0;
                        }
                        product.Quantity = quantity;
                        return product;
                    }
                },
            ];
        }
    }

    static class ExcelParser
    {
        private static int rowsAmount = 0;

        public static async Task<List<Product>> ParseAsync(string path, List<ExcelMapping> excelMappings)
        {
            return await Task.Run(() =>
            {
                return Parse(path, excelMappings);
            });
        }

        public static List<Product> Parse(string path, List<ExcelMapping> excelMappings)
        {
            if (string.IsNullOrEmpty(path))
            {
                return [];
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var file = new FileInfo(path);
            var products = new List<Product>();

            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                rowsAmount = FindRowsAmount(worksheet);

                for (int i = 2; i < rowsAmount; i++)
                {
                    var product = Product.GetDefault(ProductStatus.Processing);

                    foreach (var map in excelMappings)
                    {
                        int colNum = FindColumnIndex(worksheet, map.ExcelLabel);

                        if (colNum != -1)
                        {
                            string value = worksheet.Cells[i, colNum].Text;
                            product = map.PropertySetter(value, product);
                        } 
                    }
                    products.Add(product);
                }
            }

            return products;
        }

        private static int FindColumnIndex(ExcelWorksheet worksheet, string code)
        {
            for (int j = 1; ; j++)
            {
                var cellValue = worksheet.Cells[1, j].Text;

                if (string.IsNullOrEmpty(cellValue))
                {
                    break;
                }
                else if (string.Compare(cellValue, code, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return j;
                }
            }
            return -1;
        }
        private static int FindRowsAmount(ExcelWorksheet worksheet)
        {
            for (int i = 1; ; i++)
            {
                var cellValue = worksheet.Cells[i, 1].Text;

                if (string.IsNullOrEmpty(cellValue))
                {
                    return i;
                }
            }
        }
    }
}
