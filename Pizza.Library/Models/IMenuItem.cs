namespace Pizza.Library.Models
{
    public interface IMenuItem
    {
        IMenuCategory Category { get; set; }
        string Number { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        decimal Price { get; set; }
    }
}
