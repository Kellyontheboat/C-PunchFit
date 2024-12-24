using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class Modules
{
    public long id { get; set; }

    public long member_id { get; set; }

    public DateTime created_at { get; set; }

    public int section_id { get; set; }

    public string? module_name { get; set; }

    public virtual ICollection<ModuleItems> ModuleItems { get; set; } = new List<ModuleItems>();

    public virtual Members member { get; set; } = null!;

    public virtual Sections? section { get; set; }
}
