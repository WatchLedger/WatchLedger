using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class WatchImageRequestContract
{
    public bool? IsPrimary { get; set; }
}
