using System.Net.Http.Json;
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
        var dto = await _httpClient.GetFromJsonAsync<ValuationResponse>("https://watchvaluationservice.azurewebsites.net/api/brands", cancellationToken);
        return dto?.Brands ?? new List<string>();
    }

    public async Task<decimal> GetWatchValuationAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new DomainInvalidOperationException("Reference number is required.");
        }

        var response = await _httpClient.GetAsync($"https://watchvaluationservice.azurewebsites.net/api/valuation?referenceNumber={Uri.EscapeDataString(referenceNumber)}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content
            .ReadFromJsonAsync<ValuationResponse>(cancellationToken: cancellationToken) ??
            throw new ValuationUnavailableException("Valuation data is unavailable.");


        return dto.AveragePriceLastSixMonths;
    }
}
