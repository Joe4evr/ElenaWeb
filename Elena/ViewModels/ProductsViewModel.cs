using System.Collections.Generic;
using Elena.Models;

namespace Elena.ViewModels
{
    public class ProductsViewModel
    {
        public IEnumerable<Product> Products { get; }
        public int TotalLength { get; }

        public ProductsViewModel(IEnumerable<Product> products, int totalLength)
        {
            Products = products;
            TotalLength = totalLength;
        }
    }
}
