namespace Pizza.Library.Models
{
    using System.Collections.Generic;

    public interface IMenu
    {
        ICollection<IMenuItem> Items { get; }
    }
}
