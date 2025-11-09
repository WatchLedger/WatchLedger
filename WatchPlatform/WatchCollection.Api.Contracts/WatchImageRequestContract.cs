using System;
using System.ComponentModel.DataAnnotations;

namespace WatchCollection.Api.Contracts;

public class WatchImageRequestContract
{
    [Required]
    public Guid WatchId { get; set; }
    [Required]
    public string FileName { get; set; } = null!;
    [Required]
    public string ContentType { get; set; } = null!;
    public bool? IsPrimary { get; set; }

}
