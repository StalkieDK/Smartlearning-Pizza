namespace Pizza.API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using Pizza.API.Data;
    using Pizza.API.Models;
    using Pizza.API.Models.DTO;
    using Pizza.Library;
    using Pizza.Library.Models;

    [Route("api")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly PizzaAPIContext _context;

        public MenuController(PizzaAPIContext context)
        {
            _context = context;
        }

        [HttpPost("scrape")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ScrapingResponse>> Scrape(ScrapingRequest request)
        {
            ScrapingResponse response = new ScrapingResponse { Success = true };

            try
            {
                // Scrape server
                IMenuParser menuParser = new BillundPizzaMenuParser();
                IMenu menu = menuParser.ParseMenu();
                ICollection<MenuItemModel> scraped = menu.Items.Select(x => new MenuItemModel(x)).ToList();

                // Load all existing entries
                ICollection<MenuItemModel> existing = await this._context.MenuItemModel.ToListAsync();
                ICollection<MenuCategoryModel> categories = await this._context.MenuCategoryModel.ToListAsync();

                // Update existing records with new info
                foreach (MenuItemModel menuItem in scraped)
                {
                    MenuItemModel existingItem = existing.FirstOrDefault(x => x.Number == menuItem.Number && x.Name == menuItem.Name && 
                                                                              x.Category.Name == menuItem.Category.Name && x.Description == menuItem.Description);
                    if (existingItem != null)
                    {
                        // Entry found, remove from cached collection
                        existing.Remove(existingItem);

                        //  Only update database if data is different
                        if (existingItem.Update(menuItem))
                        {
                            this._context.Entry(existingItem).State = EntityState.Modified;
                            response.NumberUpdated++;
                        }
                    }
                    else if (request.AddNew)
                    {
                        // Add new items from scrape
                        MenuCategoryModel category = categories.FirstOrDefault(x => x.Name == menuItem.Category.Name);
                        if (category == null)
                        {
                            category = new MenuCategoryModel { Name = menuItem.Category.Name };
                            categories.Add(category);
                            this._context.Add(category);
                        }

                        menuItem.Category = category;
                        this._context.Add(menuItem);
                        response.NumberCreated++;
                    }
                }

                // If requested, remove all existing entries not accounted for
                if (request.RemoveMissing)
                {
                    foreach (MenuItemModel existingItem in existing)
                    {
                        this._context.Remove(existingItem);
                        response.NumberRemoved++;
                    }
                }

                // Save changes
                await this._context.SaveChangesAsync();
            }
            catch
            {
                response.Success = false;
            }

            return response;
        }

        [HttpGet("categories")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<IEnumerable<MenuCategoryDTO>> GetCategories()
        {
            var categories = this._context.MenuCategoryModel.Select(c => new MenuCategoryDTO { ID = c.ID, Name = c.Name });
            return categories.ToList();
        }

        [HttpGet("categories/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<MenuCategoryDetailDTO>> GetCategory(int id)
        {
            MenuCategoryModel category = await this._context.MenuCategoryModel.FindAsync(id);
            if (category == null)
            {
                return this.BadRequest();
            }

            await this._context.Entry(category).Collection(t => t.MenuItems).LoadAsync();
            var result = new MenuCategoryDetailDTO
                             {
                                 ID = category.ID,
                                 Name = category.Name,
                                 Items = category.MenuItems.Select(i => new MenuItemDTO
                                                                            {
                                                                                ID = i.ID,
                                                                                Number = i.Number,
                                                                                Name = i.Name,
                                                                                Description = i.Description,
                                                                                Price = i.Price
                                                                            }).ToList()
                             };
            return result;
        }

        [HttpGet("items")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public ActionResult<IEnumerable<MenuCategoryDetailDTO>> GetItems()
        {
            var items = this._context.MenuCategoryModel.Include(c => c.ID).Select(
                c => new MenuCategoryDetailDTO
                         {
                             ID = c.ID,
                             Name = c.Name,
                             Items = c.MenuItems.Select(
                                 i => new MenuItemDTO
                                          {
                                              ID = i.ID,
                                              Number = i.Number,
                                              Name = i.Name,
                                              Description = i.Description,
                                              Price = i.Price
                                          }).ToList()
                         });

            return items.ToList();
        }

        [HttpGet("items/{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<MenuItemDetailDTO>> GetItem(int id)
        {
            MenuItemModel item = await this._context.MenuItemModel.FindAsync(id);
            if (item == null)
            {
                return this.BadRequest();
            }

            await this._context.Entry(item).Reference(t => t.Category).LoadAsync();
            var result = new MenuItemDetailDTO
                             {
                                 ID = item.ID,
                                 Category = new MenuCategoryDTO
                                                {
                                                    ID = item.Category.ID,
                                                    Name = item.Category.Name
                                                },
                                 Number = item.Number,
                                 Name = item.Name,
                                 Description = item.Description,
                                 Price = item.Price
                             };
            return result;
        }
    }
}
