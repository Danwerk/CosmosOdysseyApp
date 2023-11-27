#pragma warning disable 1591

using App.BLL.DTO;
using App.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.Routes;

namespace WebApp.Areas.Admin.Controllers
{
    public class RoutesController : Controller
    {
        private readonly RouteService _routeService;
        private const int PageSize = 7;

        public RoutesController(RouteService routeService)
        {
            _routeService = routeService;
        }

        // GET: Routes
        public async Task<IActionResult> Index(
            [FromQuery] SearchModel searchModel, 
            string sortBy,
            int? pageNumber)
        {
            // Default sorting is ascending by From field.
            sortBy ??= "from_asc";
            ;
            ViewData["PriceSortParam"] = sortBy == "price_asc" ? "price_desc" : "price_asc";
            ViewData["DistanceSortParam"] = sortBy == "distance_asc" ? "distance_desc" : "distance_asc";
            ViewData["TravelTimeSortParam"] = sortBy == "travel_time_asc" ? "travel_time_desc" : "travel_time_asc";
            
            // Paging
            int currentPage = pageNumber ?? 1;
            var routesResults = await _routeService.GetFilteredAndSorteredRoutesResults(searchModel, sortBy);
            searchModel.Results = routesResults
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            searchModel.PageNumber = currentPage;
            searchModel.TotalPages = (int)Math.Ceiling(routesResults.Count / (double)PageSize);

            return View(searchModel);
        }
    }
}