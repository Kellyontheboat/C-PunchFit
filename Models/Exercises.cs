using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class Exercises
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string? force { get; set; }

    public string? level { get; set; }

    public string? mechanic { get; set; }

    public string? equipment { get; set; }

    public string? category { get; set; }

    public int? parts_id { get; set; }

    public virtual ICollection<ModuleItems> ModuleItems { get; set; } = new List<ModuleItems>();

    public virtual Parts? parts { get; set; }
}
