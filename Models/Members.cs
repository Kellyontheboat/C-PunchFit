using System;
using System.Collections.Generic;

namespace WebApplication_punchFit.Models;

public partial class Members
{
    public long id { get; set; }

    public string email { get; set; } = null!;

    public string username { get; set; } = null!;

    public string password { get; set; } = null!;

    public DateTime time { get; set; }

    public bool is_coach { get; set; }

    public virtual ICollection<Modules> Modules { get; set; } = new List<Modules>();
}
