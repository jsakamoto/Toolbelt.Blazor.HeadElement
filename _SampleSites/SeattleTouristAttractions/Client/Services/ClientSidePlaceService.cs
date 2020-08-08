using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SeattleTouristAttractions.Components;

namespace SeattleTouristAttractions.Client.Services
{
    public class ClientSidePlaceService : IPlacesService
    {
        private readonly HttpClient _HttpClient;

        public ClientSidePlaceService(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }

        public Task<Place[]> GetPlacesAsync()
        {
            return _HttpClient.GetFromJsonAsync<Place[]>("/places");
        }
    }
}
