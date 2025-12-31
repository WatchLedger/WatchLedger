using System;
using WatchCollection.Shared.Enums;

namespace WatchCollection.Shared.Extensions;

public static class AdvertisementStatusExtensions
{
    public static AdvertisementStatus ParseAdvertisementStatus(this string value)
    {
        if (Enum.TryParse<AdvertisementStatus>(value, ignoreCase: true, out var result))
            return result;
        
        var validValues = string.Join(", ", Enum.GetNames(typeof(AdvertisementStatus)));
        throw new InvalidOperationException($"Invalid condition value '{value}'. Valid values are: {validValues}");
    }
}
