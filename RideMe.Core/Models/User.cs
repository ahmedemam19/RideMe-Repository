using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? RoleId { get; set; }

    public int? StatusId { get; set; }

    public Driver? Driver { get; set; }

    public Passenger? Passenger { get; set; }

    public Role? Role { get; set; }

    public UserStatus? Status { get; set; }
}
