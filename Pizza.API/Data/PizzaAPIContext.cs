namespace Pizza.API.Data
{
    using Microsoft.EntityFrameworkCore;

    public class PizzaAPIContext : DbContext
    {
        public PizzaAPIContext (DbContextOptions<PizzaAPIContext> options)
            : base(options)
        {
            var conn = (System.Data.SqlClient.SqlConnection)this.Database.GetDbConnection();
            conn.AccessToken = (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
        }

        public DbSet<Pizza.API.Models.MenuItemModel> MenuItemModel { get; set; }

        public DbSet<Pizza.API.Models.MenuCategoryModel> MenuCategoryModel { get; set; }
    }
}
