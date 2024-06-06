﻿using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public int? ProductId { get; set; }

    public int? OrderId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
