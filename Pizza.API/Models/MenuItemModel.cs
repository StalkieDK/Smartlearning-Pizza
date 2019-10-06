namespace Pizza.API.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Pizza.Library.Models;

    public class MenuItemModel
    {
        public MenuItemModel() { }

        public MenuItemModel(IMenuItem menuItem)
        {
            this.Category = new MenuCategoryModel(menuItem.Category);
            this.Number = menuItem.Number;
            this.Name = menuItem.Name;
            this.Description = menuItem.Description;
            this.Price = menuItem.Price;
        }

        public int ID { get; set; }

        public MenuCategoryModel Category { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "MONEY")]
        public decimal Price { get; set; }

        public bool Update(MenuItemModel other)
        {
            bool changed = false;

            // Currently only price can change
            // If name, category or description changes, the old entry gets deleted and a new gets posted
            if (this.Price != other.Price)
            {
                this.Price = other.Price;
                changed = true;
            }

            return changed;
        }
    }
}
