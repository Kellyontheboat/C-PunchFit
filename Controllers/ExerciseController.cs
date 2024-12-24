using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using WebApplication_punchFit.Models;

//namespace WebApplication_punchFit.Controllers
//{
//    [Route("api/")]
//    [ApiController]
//    public class ExerciseController : ControllerBase
//    {
//        private readonly IDbContextFactory<workoutContext> _contextFactory;  //先在全域宣告資料庫物件，告诉编译器和运行时，这个类有一个名为 _workoutContext 的字段，它的类型是 workoutContext。
//        private readonly AuthService _authService;
//        private readonly IConfiguration _configuration;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public ExerciseController(IDbContextFactory<workoutContext> contextFactory, AuthService authService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
//        {
//            _contextFactory = contextFactory;
//            _authService = authService;
//            _configuration = configuration;
//            _httpContextAccessor = httpContextAccessor;
//        }


//        // Login endpoint
//        [HttpPost("login")]
//        public IActionResult Login([FromForm] UserLogin userLogin)
//        {
//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var user = context.Members.FirstOrDefault(u => u.email == userLogin.Email);
//                if (user == null || !_authService.VerifyPassword(userLogin.Password, user.password))
//                {
//                    return Unauthorized("Invalid username or password.");
//                }

//                // Generate token using AuthService
//                var tokenString = _authService.GenerateToken(userLogin.Email ?? string.Empty, user.id.ToString(), user.username, user.is_coach);

//                Console.WriteLine("User logged in: " + userLogin.Email);
//                return Ok(new { Token = tokenString });
//            }
//        }

//        [HttpPost("verify-token")]
//        public async Task<IActionResult> VerifyToken([FromBody] string token)
//        {
//            Console.WriteLine("Verifying token: " + token);
//            var claimsPrincipal = await _authService.ValidateTokenAsync(token);

//            if (claimsPrincipal == null)
//            {
//                return Unauthorized("Invalid token.");
//            }

//            var claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }).ToList();
//            return Ok(claims);
//        }


//        public class UserLogin
//    {
//        public string? Email { get; set; }
//        public string? Password { get; set; }
//    }


//// GET: api/Sections
//[HttpGet("Sections")]
//        public async Task<ActionResult<IEnumerable<Sections>>> GetSections()
//        {
//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var Sections = await context.Sections.ToListAsync();
//                return Ok(Sections);
//            }
//        }

//        // GET: api/Parts
//        [HttpGet("Parts")]
//        public async Task<ActionResult<IEnumerable<Parts>>> GetParts()
//        {
//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var parts = await context.Parts.ToListAsync();
//                //var parts = await _workoutContext.Parts;
//                //.Include(p => p.Exercises)
//                //.Include(p => p.sections)
//                //.ToListAsync();
//                return Ok(parts);
//            }
//        }

//        // GET: api/Members
//        [HttpGet("Members")]
//        public async Task<ActionResult<IEnumerable<Members>>> GetMembers()
//        {
//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var members = await context.Members.ToListAsync();
//                //var parts = await _workoutContext.Parts;
//                //.Include(p => p.Exercises)
//                //.Include(p => p.sections)
//                //.ToListAsync();
//                return Ok(members);
//            }
//        }

//        // GET: api/Modules
//        [HttpGet("Modules")]
//        [Authorize]
//        public async Task<ActionResult<IEnumerable<Modules>>> GetModules()
//        {
//            var user = _httpContextAccessor.HttpContext.User;
//            var memberIdClaim = user.FindFirst("id");
//            Console.WriteLine("memberIdClaim: " + memberIdClaim);

//            if (memberIdClaim == null)
//            {
//                return Unauthorized("User is not authenticated.");
//            }

//            var memberId = int.Parse(memberIdClaim.Value);

//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var modules = await context.Modules
//                    .Where(m => m.member_id == memberId)
//                    .ToListAsync();

//                return Ok(modules);
//            }
//        }

//        // GET: api/Modules/{moduleId}/ModuleItems
//        [HttpGet("Modules/{moduleId}/ModuleItems")]
//        [Authorize]
//        public async Task<ActionResult<IEnumerable<ModuleItemDto>>> GetModuleItemsByModuleId(int moduleId)
//        {
//            var user = _httpContextAccessor.HttpContext.User;
//            var memberIdClaim = user.FindFirst("id");

//            if (memberIdClaim == null)
//            {
//                return Unauthorized("User is not authenticated.");
//            }

//            var memberId = int.Parse(memberIdClaim.Value);

//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var module = await context.Modules
//                    .Include(m => m.ModuleItems)
//                    .FirstOrDefaultAsync(m => m.id == moduleId && m.member_id == memberId);

//                if (module == null)
//                {
//                    return NotFound("Module not found or user is not authorized.");
//                }

//                var moduleItems = module.ModuleItems.Select(mi => new ModuleItemDto
//                {
//                    Id = mi.id,
//                    ExerciseId = mi.exercise_id,
//                    ModuleId = mi.module_id,
//                    Reps = mi.reps,
//                    Sets = mi.sets,
//                    Weight = mi.weight
//                }).ToList();

//                return Ok(moduleItems);
//            }
//        }





//        [HttpGet("restful/ModulesWithItems")]
//        [Authorize]
//        public async Task<ActionResult<IEnumerable<ModuleWithItemsDto>>> GetModulesWithItems()
//        {
//            var user = _httpContextAccessor.HttpContext.User;
//            var memberIdClaim = user.FindFirst("id");

//            if (memberIdClaim == null)
//            {
//                return Unauthorized("User is not authenticated.");
//            }

//            var memberId = int.Parse(memberIdClaim.Value);

//            using (var context = _contextFactory.CreateDbContext())
//            {
//                var modules = await context.Modules
//                    .Where(m => m.member_id == memberId)
//                    .Include(m => m.ModuleItems) // Include associated ModuleItems
//                    .ToListAsync();

//                var result = modules.Select(m => new ModuleWithItemsDto
//                {
//                    Id = m.id,
//                    MemberId = m.member_id,
//                    CreatedAt = m.created_at,
//                    SectionId = m.section_id,
//                    ModuleName = m.module_name ?? string.Empty,
//                    ModuleItems = m.ModuleItems.Select(mi => new ModuleItemDto
//                    {
//                        Id = mi.id,
//                        ExerciseId = mi.exercise_id,
//                        ModuleId = mi.module_id,
//                        Reps = mi.reps,
//                        Sets = mi.sets,
//                        Weight = mi.weight
//                    }).ToList()
//                });

//                return Ok(result);
//            }
//        }

//        public class ModuleWithItemsDto
//        {
//            public long Id { get; set; }
//            public long MemberId { get; set; }
//            public DateTime CreatedAt { get; set; }
//            public int SectionId { get; set; }
//            public string ModuleName { get; set; }
//            public List<ModuleItemDto> ModuleItems { get; set; }
//        }

//        public class ModuleItemDto
//        {
//            public int Id { get; set; }
//            public int ExerciseId { get; set; }
//            public long ModuleId { get; set; }
//            public int? Reps { get; set; }
//            public int? Sets { get; set; }
//            public decimal? Weight { get; set; }
//        }





//    }
//}

namespace WebApplication_punchFit.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly workoutContext _context;
        private readonly AuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExerciseController(workoutContext context, AuthService authService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _authService = authService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        // Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromForm] UserLogin userLogin)
        {
            var user = _context.Members.FirstOrDefault(u => u.email == userLogin.Email);
            if (user == null || !_authService.VerifyPassword(userLogin.Password, user.password))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Generate token using AuthService
            var tokenString = _authService.GenerateToken(userLogin.Email ?? string.Empty, user.id.ToString(), user.username, user.is_coach);

            Console.WriteLine("User logged in: " + userLogin.Email);
            return Ok(new { Token = tokenString });
        }

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] string token)
        {
            Console.WriteLine("Verifying token: " + token);
            var claimsPrincipal = await _authService.ValidateTokenAsync(token);

            if (claimsPrincipal == null)
            {
                return Unauthorized("Invalid token.");
            }

            var claims = claimsPrincipal.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }

        public class UserLogin
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        // GET: api/Sections
        [HttpGet("Sections")]
        public async Task<ActionResult<IEnumerable<Sections>>> GetSections()
        {
            var sections = await _context.Sections.ToListAsync();
            return Ok(sections);
        }

        // GET: api/Parts
        [HttpGet("Parts")]
        public async Task<ActionResult<IEnumerable<Parts>>> GetParts()
        {
            var parts = await _context.Parts.ToListAsync();
            return Ok(parts);
        }

        // GET: api/Members
        [HttpGet("Members")]
        public async Task<ActionResult<IEnumerable<Members>>> GetMembers()
        {
            var members = await _context.Members.ToListAsync();
            return Ok(members);
        }

        // GET: api/Modules
        [HttpGet("Modules")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Modules>>> GetModules()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var memberIdClaim = user.FindFirst("id");
            Console.WriteLine("memberIdClaim: " + memberIdClaim);

            if (memberIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var memberId = int.Parse(memberIdClaim.Value);

            var modules = await _context.Modules
                .Where(m => m.member_id == memberId)
                .ToListAsync();

            return Ok(modules);
        }

        // GET: api/Modules/{moduleId}/ModuleItems
        [HttpGet("Modules/{moduleId}/ModuleItems")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ModuleItemDto>>> GetModuleItemsByModuleId(int moduleId)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var memberIdClaim = user.FindFirst("id");

            if (memberIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var memberId = int.Parse(memberIdClaim.Value);

            var module = await _context.Modules
                .Include(m => m.ModuleItems)
                .FirstOrDefaultAsync(m => m.id == moduleId && m.member_id == memberId);

            if (module == null)
            {
                return NotFound("Module not found or user is not authorized.");
            }

            var moduleItems = module.ModuleItems.Select(mi => new ModuleItemDto
            {
                Id = mi.id,
                ExerciseId = mi.exercise_id,
                ModuleId = mi.module_id,
                Reps = mi.reps,
                Sets = mi.sets,
                Weight = mi.weight
            }).ToList();

            return Ok(moduleItems);
        }

        [HttpGet("restful/ModulesWithItems")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ModuleWithItemsDto>>> GetModulesWithItems()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var memberIdClaim = user.FindFirst("id");

            if (memberIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var memberId = int.Parse(memberIdClaim.Value);

            var modules = await _context.Modules
                .Where(m => m.member_id == memberId)
                .Include(m => m.ModuleItems) // Include associated ModuleItems
                .ToListAsync();

            var result = modules.Select(m => new ModuleWithItemsDto
            {
                Id = m.id,
                MemberId = m.member_id,
                CreatedAt = m.created_at,
                SectionId = m.section_id,
                ModuleName = m.module_name ?? string.Empty,
                ModuleItems = m.ModuleItems.Select(mi => new ModuleItemDto
                {
                    Id = mi.id,
                    ExerciseId = mi.exercise_id,
                    ModuleId = mi.module_id,
                    Reps = mi.reps,
                    Sets = mi.sets,
                    Weight = mi.weight
                }).ToList()
            });

            return Ok(result);
        }

        public class ModuleWithItemsDto
        {
            public long Id { get; set; }
            public long MemberId { get; set; }
            public DateTime CreatedAt { get; set; }
            public int SectionId { get; set; }
            public string ModuleName { get; set; }
            public List<ModuleItemDto> ModuleItems { get; set; }
        }

        public class ModuleItemDto
        {
            public int Id { get; set; }
            public int ExerciseId { get; set; }
            public long ModuleId { get; set; }
            public int? Reps { get; set; }
            public int? Sets { get; set; }
            public decimal? Weight { get; set; }
        }
    }
}