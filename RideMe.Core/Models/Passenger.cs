using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class Passenger
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public ICollection<Ride> Rides { get; set; } = new List<Ride>();

    public User? User { get; set; }
}
