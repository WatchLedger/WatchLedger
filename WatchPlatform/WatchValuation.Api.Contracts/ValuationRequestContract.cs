using System;
using System.ComponentModel.DataAnnotations;

namespace WatchValuation.Api.Contracts;

public class ValuationRequestContract
{
    [Required]
    public string ReferenceNumber { get; set; } = string.Empty;
}
