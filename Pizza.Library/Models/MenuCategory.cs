namespace Pizza.Library.Models
{
    public class MenuCategory : IMenuCategory
    {
        public MenuCategory() { }

        public MenuCategory(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}