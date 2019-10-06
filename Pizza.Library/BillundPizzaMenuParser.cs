namespace Pizza.Library
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using AngleSharp;
    using AngleSharp.Dom;
    using AngleSharp.Html.Dom;
    using AngleSharp.Html.Parser;
    using AngleSharp.Text;
    using Pizza.Library.Models;

    public class BillundPizzaMenuParser : IMenuParser
    {
        private readonly HttpClient client;
        private readonly IHtmlParser parser;

        public BillundPizzaMenuParser()
        {
            this.client = new HttpClient();
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            this.parser = context.GetService<IHtmlParser>();
        }

        public int Errors { get; private set; }

        public IMenu ParseMenu()
        {
            string data = this.client.GetStringAsync(new Uri("http://billundpizza.dk/menu/")).Result;
            return this.ParseMenu(data);
        }

        public IMenu ParseMenu(string data)
        {
            IHtmlDocument document = this.parser.ParseDocument(data);
            var sections = document.QuerySelectorAll("section.article__content");
            return this.ParseSections(sections);
        }

        private IMenu ParseSections(IEnumerable<IElement> sections)
        {
            IMenu menu = new Menu();

            // Check for tabs
            foreach (IElement section in sections)
            {
                var tabLinks = section.QuerySelectorAll("a[data-toggle='tab']");
                if (tabLinks.Length > 0)
                {
                    // Section has tabs, extra category parsing needed
                    foreach (IElement tabLink in tabLinks)
                    {
                        string categoryPrefix = tabLink.Text();
                        string anchorId = tabLink.GetAttribute("href");
                        IElement tab = section.QuerySelector(anchorId);
                        this.GetMenuItems(menu, categoryPrefix, tab.QuerySelectorAll("h2.menu-list__title"));

                        tab.Parent.RemoveElement(tab); // Removes "tab" from the DOM
                    }
                }

                // Get all remaining menu items in this section
                this.GetMenuItems(menu, string.Empty, section.QuerySelectorAll("h2.menu-list__title")); 
            }

            return menu;
        }

        private void GetMenuItems(IMenu menu, string categoryPrefix, IEnumerable<IElement> categories)
        {
            foreach (IElement categoryElement in categories)
            {
                string categoryName = categoryElement.Text();
                if (!string.IsNullOrEmpty(categoryPrefix))
                {
                    categoryName = $"{categoryPrefix} - {categoryName}";
                }

                IMenuCategory category = new MenuCategory(categoryName);
                IElement menuItemsList = categoryElement.NextElementSibling.NextElementSibling;
                var menuItems = menuItemsList.QuerySelectorAll("li.menu-list__item");
                foreach (IElement menuItem in menuItems)
                {
                    if (menuItem.QuerySelector("span.menu-list__item-price") == null)
                    {
                        continue; // No price, no menu item
                    }

                    var descriptions = menuItem.QuerySelectorAll("span.desc__content");
                    var prices = menuItem.QuerySelectorAll("span.menu-list__item-price");

                    if (descriptions.Length == 1 && prices.Length == 1) // Normal menu item
                    {
                        IMenuItem item = CreateMenuItem(menuItem, category, GetItemDescription(menuItem), GetItemPrice(menuItem));
                        menu.Items.Add(item);
                    }
                    else if (descriptions.Length > 1 && prices.Length == 1) // Multiple descriptions but only one price
                    {
                        IElement price = prices[0];
                        foreach (IElement description in descriptions)
                        {
                            IMenuItem item = CreateMenuItem(menuItem, category, GetItemDescriptionDirect(description), GetItemPriceDirect(price));
                            menu.Items.Add(item);
                        }
                    }
                    else if (descriptions.Length == prices.Length) // Most likely extras
                    {
                        for (int i = 0; i < descriptions.Length; i++)
                        {
                            IMenuItem item = CreateMenuItem(menuItem, category, GetItemDescriptionDirect(descriptions[i]), GetItemPriceDirect(prices[i]));
                            menu.Items.Add(item);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing menu item: {menuItem.InnerHtml}");
                        this.Errors++;
                    }
                }
            }
        }

        private static IMenuItem CreateMenuItem(IParentNode element, IMenuCategory category, string description, decimal price)
        {
            string itemNumber = category.Name;
            string itemName = GetItemName(element);
            int indexOfSpace = itemName.IndexOf(' ');
            if (itemName[0].IsDigit() && indexOfSpace > 0)
            {
                // We probably have a menu number, so let's extract it
                itemNumber = itemName.Substring(0, indexOfSpace);
                if (itemNumber.EndsWith("."))
                {
                    itemNumber = itemNumber.Substring(0, itemNumber.Length - 1);
                }

                itemName = itemName.Substring(indexOfSpace + 1);
            }

            IMenuItem item = new MenuItem { Number = itemNumber.Trim(), Name = itemName.Trim(), Category = category, Description = description, Price = price };
            return item;
        }

        private static string GetItemName(IParentNode node)
        {
            string[] selectors = { "span.item_title", "h4.menu-list__item-title" };
            foreach (string selector in selectors)
            {
                IElement title = node.QuerySelector(selector);
                if (title != null)
                {
                    return Normalize(title.Text());
                }
            }

            // No selectors matched, throw exception
            throw new InvalidOperationException("Title doesn't exist in node");
        }

        private static string GetItemDescription(IParentNode element)
        {
            return GetItemDescriptionDirect(element.QuerySelector("span.desc__content"));
        }

        private static string GetItemDescriptionDirect(INode element)
        {
            return Normalize(element.Text());
        }

        private static decimal GetItemPrice(IParentNode element)
        {
            return GetItemPriceDirect(element.QuerySelector("span.menu-list__item-price"));
        }

        private static decimal GetItemPriceDirect(INode element)
        {
            string price = element.Text();
            int index = price.LastIndexOf(' ');
            return decimal.Parse(price.Substring(index + 1));
        }

        // Decodes HTML entities (&amp; => &)
        private static string Normalize(string data)
        {
            return WebUtility.HtmlDecode(data);
        }
    }
}
