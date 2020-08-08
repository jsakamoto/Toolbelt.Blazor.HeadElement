using System.Threading.Tasks;

namespace SeattleTouristAttractions.Components
{
    public interface IPlacesService
    {
        Task<Place[]> GetPlacesAsync();
    }
}
