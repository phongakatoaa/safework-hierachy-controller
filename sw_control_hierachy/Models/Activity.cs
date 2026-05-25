using System;
using System.Collections.Generic;

namespace sw_control_hierachy.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public string ActivityName { get; set; } = null!;

    public bool Enabled { get; set; }
}
