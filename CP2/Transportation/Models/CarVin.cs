using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class CarVin
{
    public int Vin { get; set; }

    public int ModelId { get; set; }

    public int OptionSetId { get; set; }

    public DateOnly ManufacturedDate { get; set; }

    public int ManufacturedPlantId { get; set; }

    public virtual ICollection<CustomerOwnership> CustomerOwnerships { get; set; } = new List<CustomerOwnership>();

    public virtual ManufacturePlant ManufacturedPlant { get; set; } = null!;

    public virtual Model Model { get; set; } = null!;

    public virtual CarOption OptionSet { get; set; } = null!;
}
