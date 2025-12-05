using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WatchValuation.Api.Contracts;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using Microsoft.Azure.Cosmos.Linq;

namespace WatchValuation.Domain.Services;

public class ValuationService(HttpClient _httpClient, IConfiguration _configuration, IValuationCacheRepository _cacheRepository) : IValuationService
{
    public async Task<ValuationResponseContract> GetValuation(ValuationRequestContract request)
    {
        var cachedValuation = await GetCachedValuation(request.ReferenceNumber);
        if (cachedValuation is not null)
            return cachedValuation;

        var token = _configuration["WatchApi"] 
            ?? throw new InvalidOperationException("Watch API token is not configured. Set it via user secrets or configuration.");
        string url = $"https://api.thewatchapi.com/v1/reference/price/history?reference_number={request.ReferenceNumber}&api_token={token}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        await using var responseStream = await response.Content.ReadAsStreamAsync();

        var apiResponse = await
            JsonSerializer.DeserializeAsync<WatchPriceHistoryResponse>(
                responseStream,
                new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

        if (apiResponse is null || apiResponse.Data is null || apiResponse.Data.Count == 0 || apiResponse.Meta is null)
            throw new Exception("Failed to retrieve valuation data.");
        
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var pricesLastSixMonths = apiResponse.Data
            .Where(p => p.Date >= sixMonthsAgo)
            .Select(p => p.Price)
            .ToList();

        if (pricesLastSixMonths.Count == 0)
            throw new Exception("No price data available for the last six months.");

        var averagePrice = pricesLastSixMonths.Average();
        return new ValuationResponseContract
        {
            AveragePriveLastSixMonths = averagePrice
        };
    }

    public async Task<ValuationResponseContract?> GetCachedValuation(string referenceNumber)
    {
        //goes to DB to check cached valuation first (not implemented yet)
        var cachedValuation = await _cacheRepository.GetCachedValuationAsync(referenceNumber);
        if (cachedValuation is not null)
        {
            return new ValuationResponseContract
            {
                AveragePriveLastSixMonths = cachedValuation.AveragePriceValuation
            };
        }
        return null;
    }

    public async Task SetCachedValuation(ValuationResponseContract valuation)
    {
        //goes to DB to set cached valuation (not implemented yet)
        throw new NotImplementedException();
    }

    // temporarily here for simple testing purposes, will be moved later
    private record WatchPriceHistoryResponse
    {
        public WatchMeta Meta { get; set; } = default!;
        public List<WatchPricePoint> Data { get; set; } = new();
    }

    private record WatchMeta
    {
        public string Brand { get; set; } = string.Empty;
        public string Reference_Number { get; set; } = string.Empty;
    }

    private record WatchPricePoint
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}