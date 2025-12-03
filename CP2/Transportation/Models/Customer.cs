using System;
using System.Collections.Generic;

namespace Transportation.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public int? HouseholdIncome { get; set; }

    public DateOnly Birthdate { get; set; }

    public long PhoneNumber { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<CustomerOwnership> CustomerOwnerships { get; set; } = new List<CustomerOwnership>();
}
