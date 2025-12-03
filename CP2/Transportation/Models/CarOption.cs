using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class CarOption
{
    public int OptionSetId { get; set; }

    public int? ModelId { get; set; }

    public int EngineId { get; set; }

    public int TransmissionId { get; set; }

    public int ChassisId { get; set; }

    public int? PremiumSoundId { get; set; }

    public string Color { get; set; } = null!;

    public int OptionSetPrice { get; set; }

    public virtual ICollection<CarVin> CarVins { get; set; } = new List<CarVin>();

    public virtual CarPart Chassis { get; set; } = null!;

    public virtual CarPart Engine { get; set; } = null!;

    public virtual Model? Model { get; set; }

    public virtual CarPart? PremiumSound { get; set; }

    public virtual CarPart Transmission { get; set; } = null!;
}
