using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class ModuleItems
{
    public int id { get; set; }

    public int exercise_id { get; set; }

    public long module_id { get; set; }

    public int? reps { get; set; }

    public int? sets { get; set; }

    public decimal? weight { get; set; }

    public virtual Exercises exercise { get; set; } = null!;

    public virtual Modules module { get; set; } = null!;
}
