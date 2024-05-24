using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class LnkProductAttribute
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public int? AttributeId { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public virtual Attribute? Attribute { get; set; }

    public virtual Product? Product { get; set; }
}
