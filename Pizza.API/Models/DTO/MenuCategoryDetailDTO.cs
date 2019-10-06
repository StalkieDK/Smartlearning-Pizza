namespace Pizza.API.Models.DTO
{
    using System.Collections.Generic;

    public class MenuCategoryDetailDTO : MenuCategoryDTO
    {
        public ICollection<MenuItemDTO> Items { get; set; }
    }
}
