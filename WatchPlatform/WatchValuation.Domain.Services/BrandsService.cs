using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using WatchValuation.Api.Contracts;
using System.Text.Json;
using WatchValuation.Storage.Interfaces;
using WatchValuation.Storage.Records;
using WatchValuation.Domain.Services.Exceptions;

namespace WatchValuation.Domain.Services;

public class BrandsService(HttpClient _httpClient, IConfiguration _configuration, IBrandsCacheRepository _cacheRepository) : IBrandsService
{
    public async Task<BrandListResponseContract> GetWatchBrandsFromCacheAsync()
    {
        var cachedBrands = await _cacheRepository.GetCachedBrandsAsync();
        if (cachedBrands is not null)
            return new BrandListResponseContract{ Brands = cachedBrands.Brands };

        var brands = await GetWatchBrandsFromApiAsync() 
            ?? throw new WatchBrandListException("Failed to retrieve watch brands from API.");

        return new BrandListResponseContract { Brands = brands.Brands }; 
    }

    private async Task<BrandListResponseContract> GetWatchBrandsFromApiAsync()
    {
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
            throw new WatchBrandListException("Failed to retrieve watch brands.");

        var brandList = new BrandListResponseContract { Brands = apiResponse.Data };
        await SetWatchBrands(brandList);

        return brandList;
    }


    private async Task SetWatchBrands(BrandListResponseContract brands)
    {
        var cachedBrands = new CachedBrands{Brands = brands.Brands};
        await _cacheRepository.SetCachedBrandsAsync(cachedBrands);
    }

    private record WatchBrandsResponse
    {
        public List<string> Data { get; set; } = new();
    }
}
