namespace NanaFoodWeb.IRepository
{
    public interface ITokenProvider
    {
        void SetToken(string token);
        string? GetToken();
        void ClearToken();
        string? ReadToken(string type, string token);
        void SetCartCount(string cartCount);
        string? GetCartCount();
        void ClearCartCount();
    }
}
