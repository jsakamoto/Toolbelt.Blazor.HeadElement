using System.Threading.Tasks;

namespace SeattleTouristAttractions.Shared
{
    public interface IPlacesService
    {
        Task<Place[]> GetPlacesAsync();
    }
}
