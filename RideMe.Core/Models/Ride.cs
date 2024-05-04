using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class Ride
{
    public int Id { get; set; }

    public int? PassengerId { get; set; }

    public int? DriverId { get; set; }

    public string? RideSource { get; set; }

    public string? RideDestination { get; set; }

    public int? StatusId { get; set; }

    public double? Price { get; set; }

    public int? Rating { get; set; }

    public string? Feedback { get; set; }

    public DateTime RideDate { get; set; }

    public Driver? Driver { get; set; }

    public Passenger? Passenger { get; set; }

    public  RideStatus? Status { get; set; }
}
