using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class Parts
{
    public int id { get; set; }

    public string part_name { get; set; } = null!;

    public int? sections_id { get; set; }

    public virtual ICollection<Exercises> Exercises { get; set; } = new List<Exercises>();

    public virtual Sections? sections { get; set; }
}
