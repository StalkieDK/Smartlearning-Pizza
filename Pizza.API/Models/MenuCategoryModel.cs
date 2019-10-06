namespace Pizza.API.Models
{
    using System.Collections.Generic;
    using Pizza.Library.Models;

    public class MenuCategoryModel
    {
        public MenuCategoryModel() { }

        public MenuCategoryModel(IMenuCategory menuCategory)
        {
            this.Name = menuCategory.Name;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<MenuItemModel> MenuItems { get; set; }
    }
}
