using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class Driver
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? CarType { get; set; }

    public bool Smoking { get; set; }

    public int? CityId { get; set; }

    public string? Region { get; set; }

    public bool Available { get; set; }

    public double AvgRating { get; set; }

    public City? City { get; set; }

    public ICollection<Ride> Rides { get; set; } = new List<Ride>();

    public User? User { get; set; }
}
