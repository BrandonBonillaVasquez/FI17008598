using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class Model
{
    public int ModelId { get; set; }

    public string ModelName { get; set; } = null!;

    public int ModelBasePrice { get; set; }

    public int BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<CarOption> CarOptions { get; set; } = new List<CarOption>();

    public virtual ICollection<CarVin> CarVins { get; set; } = new List<CarVin>();
}
