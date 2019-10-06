namespace Pizza.Library.Models
{
    public class MenuItem : IMenuItem
    {
        public MenuItem() { }

        public MenuItem(IMenuCategory category, string number, string name, string description, decimal price)
        {
            this.Category = category;
            this.Number = number;
            this.Name = name;
            this.Description = description;
            this.Price = price;
        }

        public IMenuCategory Category { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"Category: {this.Category.Name}, Number: {this.Number}, Name: {this.Name}, Price: {this.Price}, Description: {this.Description}";
        }
    }
}
