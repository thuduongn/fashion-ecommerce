using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class Attribute
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int ParentId { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public virtual ICollection<LnkProductAttribute> LnkProductAttributes { get; set; } = new List<LnkProductAttribute>();
}
