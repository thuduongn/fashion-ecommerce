using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class LnkProductCategory
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? CategoryId { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
