using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class City
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}
