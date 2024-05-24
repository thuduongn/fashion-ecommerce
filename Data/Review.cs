using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class Review
{
    public int Id { get; set; }

    public int Rate { get; set; }

    public string? Content { get; set; }

    public int? Status { get; set; }

    public int? Deleted { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public int? ProductId { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product? Product { get; set; }
}
