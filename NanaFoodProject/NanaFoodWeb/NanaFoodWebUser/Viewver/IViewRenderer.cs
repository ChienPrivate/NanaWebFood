namespace StoreManagement.Viewver
{
    public interface IViewRenderer
    {
        Task<string> RenderViewToStringAsync(string viewName, object model);
    }

}
