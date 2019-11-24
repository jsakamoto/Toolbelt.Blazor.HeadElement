using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeattleTouristAttractions.Shared;

namespace SeattleTouristAttractions.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlacesController : ControllerBase
    {
        private readonly IPlacesService _PlacesService;

        public PlacesController(IPlacesService placesService)
        {
            _PlacesService = placesService;
        }

        [HttpGet]
        public Task<Place[]> GetAsync()
        {
            return this._PlacesService.GetPlacesAsync();
        }
    }
}
