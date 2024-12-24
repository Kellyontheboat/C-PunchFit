using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class Sections
{
    public int id { get; set; }

    public string section_name { get; set; } = null!;

    public string section_img { get; set; } = null!;

    public virtual ICollection<Modules> Modules { get; set; } = new List<Modules>();

    public virtual ICollection<Parts> Parts { get; set; } = new List<Parts>();
}
