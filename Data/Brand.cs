using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class Brand
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Img { get; set; }

    public string Slug { get; set; } = null!;

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public string? Test { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
