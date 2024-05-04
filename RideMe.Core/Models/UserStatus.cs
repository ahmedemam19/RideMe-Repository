﻿using System;
using System.Collections.Generic;

namespace RideMe.Core.Models;

public class UserStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public  ICollection<User> Users { get; set; } = new List<User>();
}
