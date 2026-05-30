using System;
using System.Collections.Generic;

namespace CareSync.Models;

public partial class Medicine
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Manufacturer { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? Discount { get; set; }

    public int? Quantity { get; set; }

    public DateTime? ExpDate { get; set; }

    public string? ImageUrl { get; set; }

    public int? Status { get; set; }
}
