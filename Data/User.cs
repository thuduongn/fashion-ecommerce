using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? UserName { get; set; }

    public string? Pd { get; set; }

    public virtual ICollection<OrderHistory> OrderHistories { get; set; } = new List<OrderHistory>();
}
