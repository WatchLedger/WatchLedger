using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WatchValuation.Api.Contracts;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using WatchValuation.Domain.Services.Exceptions;

namespace WatchValuation.Domain.Services;

public class ValuationService(HttpClient _httpClient, IConfiguration _configuration, IValuationCacheRepository _cacheRepository) : IValuationService
{

    public async Task<ValuationResponseContract> GetWatchValuationFromCacheAsync(string referenceNumber)
    {
        var cachedValuation = await _cacheRepository.GetCachedValuationAsync(referenceNumber);
        if (cachedValuation is not null && cachedValuation.AveragePriceLastSixMonths > 0)
            return new ValuationResponseContract { AveragePriceLastSixMonths = cachedValuation.AveragePriceLastSixMonths };
        
        var valuation = await GetWatchValuationFromApiAsync(referenceNumber) 
            ?? throw new WatchValuationException("Failed to retrieve watch valuation from API.");
        
        return new ValuationResponseContract { AveragePriceLastSixMonths = valuation.AveragePriceLastSixMonths };
    }

    private async Task<ValuationResponseContract> GetWatchValuationFromApiAsync(string referenceNumber)
    {
        //var token = _configuration["WatchApi"] 
        //     ?? throw new InvalidOperationException("Watch API token is not configured. Set it via user secrets or configuration.");
        // string url = $"https://api.thewatchapi.com/v1/reference/price/history?reference_number={request.ReferenceNumber}&api_token={token}";

        // var response = await _httpClient.GetAsync(url);
        // response.EnsureSuccessStatusCode();
        
        // await using var responseStream = await response.Content.ReadAsStreamAsync();
        // var apiResponse = await
        //     JsonSerializer.DeserializeAsync<WatchPriceHistoryResponse>(
        //         responseStream,
        //         new JsonSerializerOptions 
        //         { 
        //             PropertyNameCaseInsensitive = true 
        //         });

        // temporarily mocked response bacause API access is not available right now
        var apiResponse = new WatchPriceHistoryResponse
        {
            Meta = new WatchMeta
            {
                Brand = "Rolex",
                Reference_Number = referenceNumber
            },
            Data = new List<WatchPricePoint>
            {
                new WatchPricePoint { Date = DateTime.UtcNow.AddMonths(-1), Price = 12000 },
                new WatchPricePoint { Date = DateTime.UtcNow.AddMonths(-3), Price = 11500 },
                new WatchPricePoint { Date = DateTime.UtcNow.AddMonths(-5), Price = 11800 },
            }
        };

        if (apiResponse is null || apiResponse.Data is null || apiResponse.Data.Count == 0 || apiResponse.Meta is null)
            throw new WatchValuationException("Failed to retrieve valuation data.");
        
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);
        var pricesLastSixMonths = apiResponse.Data
            .Where(p => p.Date >= sixMonthsAgo)
            .Select(p => p.Price)
            .ToList();

        if (pricesLastSixMonths.Count == 0)
            throw new WatchValuationUnavailableException("No price data available for the last six months.");

        var watchValuation = new ValuationResponseContract{ AveragePriceLastSixMonths = pricesLastSixMonths.Average() };
        await SetCachedValuation(watchValuation, referenceNumber);
        return watchValuation;
    }

    private async Task SetCachedValuation(ValuationResponseContract valuation, string referenceNumber)
    {
        try{
            var cachedValuation = new CachedValuation
            {
                id = referenceNumber,
                AveragePriceLastSixMonths = valuation.AveragePriceLastSixMonths
            };
            await _cacheRepository.SetCachedValuationAsync(cachedValuation);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error caching valuation: {ex.Message}");
        }
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