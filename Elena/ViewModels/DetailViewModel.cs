using Elena.Models;

namespace Elena.ViewModels
{
    public class DetailViewModel
    {
        public string Name { get; }
        public string Description { get; }
        public string Image { get; }
        public decimal Price { get; }
        public ProductType Type { get; }

        public DetailViewModel(Product product)
        {
            Name = product.Name;
            Description = product.Description;
            Image = product.ImagePath;
            Price = product.Price;
            Type = product.Type;
        }
    }
}
