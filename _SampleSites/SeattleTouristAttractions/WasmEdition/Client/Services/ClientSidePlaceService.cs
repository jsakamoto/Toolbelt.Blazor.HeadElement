using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SeattleTouristAttractions.Shared;

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
            return _HttpClient.GetJsonAsync<Place[]>("/places");
        }
    }
}
