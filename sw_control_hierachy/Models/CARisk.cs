using System;
using System.Collections.Generic;

namespace sw_control_hierachy.Models;

public partial class Carisk
{
    public int CariskId { get; set; }

    public string Risk { get; set; } = null!;

    public int OrderId { get; set; }

    public string ColorCode { get; set; } = null!;
}
