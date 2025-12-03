using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class CarPart
{
    public int PartId { get; set; }

    public string PartName { get; set; } = null!;

    public int ManufacturePlantId { get; set; }

    public DateOnly ManufactureStartDate { get; set; }

    public DateOnly? ManufactureEndDate { get; set; }

    public int? PartRecall { get; set; }

    public virtual ICollection<CarOption> CarOptionChasses { get; set; } = new List<CarOption>();

    public virtual ICollection<CarOption> CarOptionEngines { get; set; } = new List<CarOption>();

    public virtual ICollection<CarOption> CarOptionPremiumSounds { get; set; } = new List<CarOption>();

    public virtual ICollection<CarOption> CarOptionTransmissions { get; set; } = new List<CarOption>();

    public virtual ManufacturePlant ManufacturePlant { get; set; } = null!;
}
