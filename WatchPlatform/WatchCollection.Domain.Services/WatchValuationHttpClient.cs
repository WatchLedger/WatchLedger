using System.Net.Http.Json;
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
        var dto = await _httpClient.GetFromJsonAsync<ValuationResponse>("http://localhost:5005/api/brands", cancellationToken);
        return dto?.Brands ?? new List<string>();
    }

    public async Task<decimal> GetWatchValuationAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            throw new ArgumentException("Reference number is required.", nameof(referenceNumber));
        }

        var response = await _httpClient.GetAsync($"http://localhost:5005/api/valuation?referenceNumber={Uri.EscapeDataString(referenceNumber)}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var dto = await response.Content.ReadFromJsonAsync<ValuationResponse>(cancellationToken: cancellationToken);

        if (dto is null)
        {
            throw new InvalidOperationException("Valuation response was empty.");
        }

        return dto.AveragePriceLastSixMonths;
    }
}
