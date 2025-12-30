using System;
using WatchCollection.Shared.Enums;

namespace WatchCollection.Shared.Extensions;

public static class WatchConditionExtensions
{
     public static WatchCondition ParseWatchCondition(this string value)
    {
        if (Enum.TryParse<WatchCondition>(value, ignoreCase: true, out var result))
            return result;
        
        var validValues = string.Join(", ", Enum.GetNames(typeof(WatchCondition)));
        throw new InvalidOperationException($"Invalid condition value '{value}'. Valid values are: {validValues}");
    }
}
