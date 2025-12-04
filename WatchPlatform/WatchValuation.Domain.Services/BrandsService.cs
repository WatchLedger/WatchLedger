using System;
using WatchValuation.Domain.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace WatchValuation.Domain.Services;

public class BrandsService(HttpClient _httpClient, IConfiguration _configuration) : IBrandsService
{
    public async Task<List<string>> GetBrands()
    {
        throw new NotImplementedException();
    }
}
