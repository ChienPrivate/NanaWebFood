using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace StoreManagement.Viewver
{
    public class ViewRenderer : IViewRenderer
    {
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IViewEngine _viewEngine;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderer(
            ITempDataProvider tempDataProvider,
            IViewEngine viewEngine,
            IActionContextAccessor actionContextAccessor,
            IServiceProvider serviceProvider)
        {
            _tempDataProvider = tempDataProvider;
            _viewEngine = viewEngine;
            _actionContextAccessor = actionContextAccessor;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            var actionContext = _actionContextAccessor.ActionContext;
            var viewResult = _viewEngine.FindView(actionContext, viewName, false);

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                    new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }
    }

}
