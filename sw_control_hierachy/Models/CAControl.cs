using System;
using System.Collections.Generic;

namespace sw_control_hierachy.Models;

public partial class Cacontrol
{
    public int CacontrolId { get; set; }

    public string Control { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int OrderId { get; set; }

    public bool Enabled { get; set; }
}
