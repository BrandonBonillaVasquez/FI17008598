using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class Dealer
{
    public int DealerId { get; set; }

    public string DealerName { get; set; } = null!;

    public string? DealerAddress { get; set; }

    public virtual ICollection<CustomerOwnership> CustomerOwnerships { get; set; } = new List<CustomerOwnership>();

    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();
}
