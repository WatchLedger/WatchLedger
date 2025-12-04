using System;

namespace WatchValuation.Api.Contracts;

public class BrandListResponseContract
{
    public List<string> Brands { get; set; } = new List<string>();
}
