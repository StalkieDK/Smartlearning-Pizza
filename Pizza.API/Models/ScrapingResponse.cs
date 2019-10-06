namespace Pizza.API.Models
{
    public class ScrapingResponse
    {
        public bool Success { get; set; }

        public int NumberUpdated { get; set; }

        public int NumberCreated { get; set; }

        public int NumberRemoved { get; set; }
    }
}
