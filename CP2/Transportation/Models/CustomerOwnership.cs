using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class CustomerOwnership
{
    public int CustomerId { get; set; }

    public int Vin { get; set; }

    public DateOnly PurchaseDate { get; set; }

    public int PurchasePrice { get; set; }

    public DateOnly? WaranteeExpireDate { get; set; }

    public int DealerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Dealer Dealer { get; set; } = null!;

    public virtual CarVin VinNavigation { get; set; } = null!;
}
