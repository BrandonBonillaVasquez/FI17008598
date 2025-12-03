using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class ManufacturePlant
{
    public int ManufacturePlantId { get; set; }

    public string PlantName { get; set; } = null!;

    public string? PlantType { get; set; }

    public string? PlantLocation { get; set; }

    public int? CompanyOwned { get; set; }

    public virtual ICollection<CarPart> CarParts { get; set; } = new List<CarPart>();

    public virtual ICollection<CarVin> CarVins { get; set; } = new List<CarVin>();
}
