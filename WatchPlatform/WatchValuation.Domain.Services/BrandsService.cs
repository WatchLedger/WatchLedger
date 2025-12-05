using System;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using WatchValuation.Api.Contracts;
using System.Text.Json;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;

namespace WatchValuation.Domain.Services;

public class BrandsService(HttpClient _httpClient, IConfiguration _configuration, IBrandsCacheRepository _cacheRepository) : IBrandsService
{
    public async Task<BrandListResponseContract> GetBrands()
    {
        var brands = await GetWatchBrands();
        if (brands is not null)
            return brands;

        var token = _configuration["WatchApi"]
             ?? throw new InvalidOperationException("Watch API token is not configured. Set it via user secrets or configuration.");
        string url = $"https://api.thewatchapi.com/v1/brand/list?api_token={token}";
        var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        var apiResponse = await
            JsonSerializer.DeserializeAsync<WatchBrandsResponse>(
                responseStream,
                new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

        if (apiResponse is null || apiResponse.Data is null || apiResponse.Data.Count == 0)
            throw new Exception("Failed to retrieve brand data.");

        var brandList = new BrandListResponseContract { Brands = apiResponse.Data };
        await SetWatchBrands(brandList);

        return brandList;
    }

    public async Task<BrandListResponseContract?> GetWatchBrands()
    {
        var cachedBrands = await _cacheRepository.GetCachedBrandsAsync();
        if (cachedBrands is null)
            return null;

        return new BrandListResponseContract
        {
            Brands = cachedBrands.Brands
        };
    }

    public async Task SetWatchBrands(BrandListResponseContract brands)
    {
        var cachedBrands = new CachedBrands {
            Brands = brands.Brands,
            Ttl = 86400 // 1 day
        };
        await _cacheRepository.SetCachedBrandsAsync(cachedBrands);
    }

    public record WatchBrandsResponse
    {
        public List<string> Data { get; set; } = new();
    }
}
