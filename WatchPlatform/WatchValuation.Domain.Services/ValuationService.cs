using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WatchValuation.Api.Contracts;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Options;
using WatchValuation.Infrastructure;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using WatchValuation.Domain.Services.Exceptions;

namespace WatchValuation.Domain.Services;

public class ValuationService(HttpClient _httpClient, IOptions<ExternalApiOptions> _options, IValuationCacheRepository _cacheRepository) : IValuationService
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
        var apiOptions = _options.Value;
        string url = $"{apiOptions.BaseUrl}/reference/price/history?reference_number={referenceNumber}&api_token={apiOptions.ApiKey}";

        var response = await _httpClient.GetAsync(url);
        
       if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (errorContent.Contains("malformed_parameters"))
                {
                    throw new WatchValuationUnavailableException("No valuation available for the provided reference number.");
                }
                throw new WatchValuationException("Invalid reference number format.");
            }
            
            throw new WatchValuationUnavailableException($"External API error: {response.StatusCode}");
        }
        
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var apiResponse = await
            JsonSerializer.DeserializeAsync<WatchPriceHistoryResponse>(
                responseStream,
                new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

        if (apiResponse is null || apiResponse.Data is null || apiResponse.Data.Count == 0 || apiResponse.Meta is null)
            throw new WatchValuationException("Failed to retrieve valuation data.");
        
        // I had to alter this slightly since the data is outdated and stops at 2024-07-.. so it's the last six months from the latest date available
        var anchorDate = apiResponse.Data.Max(p => p.Date);
        var sixMonthsAgo = anchorDate.AddMonths(-6);
        var pricesLastSixMonths = apiResponse.Data
            .Where(p => p.Date >= sixMonthsAgo)
            .Select(p => p.Price)
            .ToList();

        if (pricesLastSixMonths.Count == 0)
            throw new WatchValuationUnavailableException("No price data available for the last six months.");

         var average = pricesLastSixMonths.Average();
         var rounded = Math.Round(average, 2, MidpointRounding.AwayFromZero);
         var watchValuation = new ValuationResponseContract { AveragePriceLastSixMonths = rounded };
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