using System;

namespace WatchValuation.Domain.Services.Interfaces;

public interface IBrandsService
{
    Task<List<string>> GetBrands();
}
