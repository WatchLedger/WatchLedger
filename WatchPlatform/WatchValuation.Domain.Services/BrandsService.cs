using System;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using WatchValuation.Api.Contracts;
using System.Text.Json;

namespace WatchValuation.Domain.Services;

public class BrandsService(HttpClient _httpClient, IConfiguration _configuration) : IBrandsService
{
    public async Task<BrandListResponseContract> GetBrands()
    {
        var brands = await GetWatchBrands();
        if (brands is not null)
            return brands;

        var token = _configuration["ApiKeys:WatchApi"];
        //     ?? throw new InvalidOperationException("Watch API token is not configured. Set it via user secrets or configuration.");
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

        return new BrandListResponseContract
        {
            Brands = apiResponse.Data
        };
    }

    public async Task<BrandListResponseContract?> GetWatchBrands()
    {
        //goes to DB to check cached brands first (not implemented yet)
        return null;
    }

    public record WatchBrandsResponse
    {
        public List<string> Data { get; set; } = new();
    }
}
