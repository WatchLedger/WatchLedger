using System;
using WatchValuation.Api.Contracts;

namespace WatchValuation.Domain.Services.Interfaces;

public interface IBrandsService
{
    Task<BrandListResponseContract> GetBrands();
}
