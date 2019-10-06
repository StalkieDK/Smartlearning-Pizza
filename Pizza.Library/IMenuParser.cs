namespace Pizza.Library
{
    using Pizza.Library.Models;

    public interface IMenuParser
    {
        IMenu ParseMenu();
        IMenu ParseMenu(string data);

        int Errors { get; }
    }
}
