using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class OrderHistory
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? OrderId { get; set; }

    public string? Description { get; set; }

    public int? CreatedAt { get; set; }

    public int? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Order? Order { get; set; }

    public virtual User? User { get; set; }
}
