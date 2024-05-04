using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class RideStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public ICollection<Ride> Rides { get; set; } = new List<Ride>();
}
