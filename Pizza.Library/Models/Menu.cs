namespace Pizza.Library.Models
{
    using System.Collections.Generic;

    public class Menu : IMenu
    {
        public Menu()
        {
            this.Items = new List<IMenuItem>();
        }

        public ICollection<IMenuItem> Items { get; }
    }
}
