using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class BidRequestContract
{
    [Required]
    public required decimal Amount { get; set; }
}
