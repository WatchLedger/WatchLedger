using System.Net.Http.Json;
using Duende.IdentityModel.Client;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Domain.Services;

public sealed class WatchValuationHttpClient(HttpClient _httpClient) : IWatchValuationHttpClient
{

    private sealed class ValuationResponse
    {
        public List<string> Brands { get; init; } = new();
        public decimal AveragePriceLastSixMonths { get; init; }
    }

    public async Task<IReadOnlyList<string>> GetWatchBrandsAsync(CancellationToken cancellationToken = default)
    {
        await SetAccessTokenAsync();
        //var dto = await _httpClient.GetFromJsonAsync<ValuationResponse>("https://watchvaluationservice.azurewebsites.net/api/brands", cancellationToken);
        var response = await _httpClient.GetAsync("http://localhost:5005/api/brands", cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new DomainInvalidOperationException("Access to valuation service is denied.");
            }

            throw new DomainInvalidOperationException("Unable to retrieve watch brands from valuation service.");
        }

        var dto =  await response.Content
            .ReadFromJsonAsync<ValuationResponse>(cancellationToken: cancellationToken);
        return dto?.Brands ?? new List<string>();
    }

    public async Task<decimal> GetWatchValuationAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        await SetAccessTokenAsync();
        
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new DomainInvalidOperationException("Reference number is required.");
        }

        //var response = await _httpClient.GetAsync($"https://watchvaluationservice.azurewebsites.net/api/valuation?referenceNumber={Uri.EscapeDataString(referenceNumber)}", cancellationToken);
        var response = await _httpClient.GetAsync($"http://localhost:5005/api/valuation?referenceNumber={Uri.EscapeDataString(referenceNumber)}", cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ValuationUnavailableException("No valuation available for the provided reference number.");
            } else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                       response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new DomainInvalidOperationException("Access to valuation service is denied.");
            }
            
            throw new DomainInvalidOperationException($"Valuation service error: {response.StatusCode}");
        }

        var dto = await response.Content
            .ReadFromJsonAsync<ValuationResponse>(cancellationToken: cancellationToken) ??
            throw new ValuationUnavailableException("Valuation data is unavailable.");

        return dto.AveragePriceLastSixMonths;
    }

    public async Task SetAccessTokenAsync()
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
        var tokenResponse = await _httpClient
        .RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "m2m.WatchCollection-WatchValuation",
                ClientSecret = "WatchCollectionSecretWoohoo",
                Scope = "WatchValuation.Api.Read"
            });
        
        _httpClient.SetBearerToken(tokenResponse.AccessToken ?? throw new DomainInvalidOperationException("Token not found"));
    }
}
