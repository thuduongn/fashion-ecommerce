using System;
using System.Collections.Generic;

namespace fashion.Data;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Price { get; set; }

    public string? Abstract { get; set; }

    public string? Description { get; set; }

    public string? Img { get; set; }

    public string Slug { get; set; } = null!;

    public int? Quantity { get; set; }

    public int Status { get; set; }

    public int? Deleted { get; set; }

    public int CreatedBy { get; set; }

    public int CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public int UpdatedAt { get; set; }

    public int? BrandId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<LnkProductAttribute> LnkProductAttributes { get; set; } = new List<LnkProductAttribute>();

    public virtual ICollection<LnkProductCategory> LnkProductCategories { get; set; } = new List<LnkProductCategory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
