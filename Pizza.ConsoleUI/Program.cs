namespace Pizza.ConsoleUI
{
    using System;
    using System.Linq;

    using Pizza.Library;
    using Pizza.Library.Models;

    class Program
    {
        static void Main()
        {
            IMenuParser menuParser = new BillundPizzaMenuParser();
            IMenu menu = menuParser.ParseMenu();
            int wrong = 0;

            Console.WriteLine("***** Billund Pizzaria Menu *****");
            
            Console.WriteLine();
            foreach (IMenuItem item in menu.Items)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine();
            Console.WriteLine($"Number of items: {menu.Items.Count}");
            var categories = menu.Items.Select(x => x.Category).Distinct().ToList();
            foreach (IMenuCategory category in categories)
            {
                Console.WriteLine($"{category.Name} - {menu.Items.Count(x => x.Category == category)}");
            }
            Console.WriteLine();
            Console.WriteLine("Done");
            Console.WriteLine($"Number of wrong parsings: {wrong}");
            Console.ReadLine();
        }
    }
}
