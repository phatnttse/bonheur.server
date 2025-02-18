using Bonheur.BusinessObjects.Models;
using Bonheur.Services.Interfaces;
using Bonheur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bonheur.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly string _goongApiKey;
        private readonly string _autoCompleteUrl;
        private readonly HttpClient _httpClient;

        public PlaceService(HttpClient httpClient)
        {
            _goongApiKey = Environment.GetEnvironmentVariable("GOONG_API_KEY") ?? throw new ArgumentNullException("GOONG_API_KEY is not set");
            _autoCompleteUrl = "https://rsapi.goong.io/place/autocomplete";
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ApplicationResponse> AutoComplete(string input, string? location)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) throw new ApiException("Input is required", System.Net.HttpStatusCode.BadRequest);

                string url = $"{_autoCompleteUrl}?input={Uri.EscapeDataString(input)}&api_key={_goongApiKey}";

                if (!string.IsNullOrEmpty(location))
                {
                    url = $"{_autoCompleteUrl}?input={Uri.EscapeDataString(input)}&location={Uri.EscapeDataString(location)}&api_key={_goongApiKey}";
                }

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException("Error calling Goong API", response.StatusCode);
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var placeResults = JsonSerializer.Deserialize<object>(responseBody);

                return new ApplicationResponse
                {
                    Data = placeResults,
                    Message = "Place results",
                    Success = true,
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.Message, System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
