using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication_punchFit.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
//using HotChocolate.Authorization.AuthorizeAttribute;
using System.Security.Claims;

//namespace WebApplication_punchFit.graphql
//{
//    // .AddTypes() in Program.cs will automatically register this class by HotChocolate 
//    [QueryType]
//    public class Query
//    {
//        private readonly ILogger<Query> _logger;
//        //private readonly AuthService _authService;

//        public Query(ILogger<Query> logger)//, AuthService authService
//        {
//            _logger = logger;
//            // _authService = authService;
//        }

//        public async Task<Parts> GetPartById(int parts_id, [Service] IDbContextFactory<workoutContext> dbContextFactory)
//        {
//            await using var context = dbContextFactory.CreateDbContext();
//            return await context.Parts.FirstOrDefaultAsync(p => p.id == parts_id);
//        }

//        // Fetch Exercises
//        public async Task<IEnumerable<Exercises>> GetExercises(
//            int? exercise_id,
//            [Service] IDbContextFactory<workoutContext> dbContextFactory)
//        {
//            _logger.LogInformation("Fetching Exercises");
//            try
//            {
//                await using var context = dbContextFactory.CreateDbContext();

//                IQueryable<Exercises> query = context.Exercises
//                   .Include(e => e.parts);

//                if (exercise_id.HasValue)
//                {
//                    query = query.Where(e => e.id == exercise_id.Value);
//                }

//                var exercisesResult = await query.ToListAsync();

//                if (exercisesResult == null || !exercisesResult.Any())
//                {
//                    _logger.LogWarning("No Exercises found");
//                }

//                return exercisesResult;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "An error occurred while fetching Exercises");
//                throw; // Rethrow to let GraphQL handle
//            }
//        }

//        #region GetModuleItems with specific member
//        // Fetch ModuleItems by moduleId
//        public async Task<IEnumerable<ModuleItems>> GetModuleItems(int moduleId,
//        [Service] IHttpContextAccessor httpContextAccessor,
//        [Service] IDbContextFactory<workoutContext> dbContextFactory)
//        {
//            var user = httpContextAccessor.HttpContext.User;
//            if (user == null || !user.Identity.IsAuthenticated)
//            {
//                throw new UnauthorizedAccessException("User is not authenticated.");
//            }

//            var memberIdClaim = user.FindFirst("id");
//            if (memberIdClaim == null)
//            {
//                throw new UnauthorizedAccessException("User ID claim not found.");
//            }
//            var memberId = int.Parse(memberIdClaim.Value);

//            _logger.LogInformation("Fetching ModuleItems with moduleId: {moduleId}", moduleId);
//            await using var context = dbContextFactory.CreateDbContext();

//            var module = await context.Modules
//                .FirstOrDefaultAsync(m => m.id == moduleId);

//            if (module == null)
//            {
//                _logger.LogWarning("Module not found for moduleId: {moduleId}", moduleId);
//                throw new UnauthorizedAccessException("Module not found.");
//            }

//            if (module.member_id != memberId)
//            {
//                _logger.LogWarning("User is not authorized to access moduleId: {moduleId}", moduleId);
//                throw new UnauthorizedAccessException("User is not authorized to access this module.");
//            }

//            return await context.ModuleItems
//                .Where(mi => mi.module_id == moduleId)
//                .ToListAsync();
//        }
//        #endregion

//        public async Task<IEnumerable<Modules>> GetModules(
//        [Service] IHttpContextAccessor httpContextAccessor,
//        [Service] IDbContextFactory<workoutContext> dbContextFactory)
//        {
//            var user = httpContextAccessor.HttpContext.User;
//            if (user == null || !user.Identity.IsAuthenticated)
//            {
//                throw new UnauthorizedAccessException("User is not authenticated.");
//            }

//            var memberIdClaim = user.FindFirst("id");
//            if (memberIdClaim == null)
//            {
//                throw new UnauthorizedAccessException("User ID claim not found.");
//            }
//            var memberId = int.Parse(memberIdClaim.Value);

//            _logger.LogInformation("Fetching Modules with memberId: {memberId}", memberId);
//            await using var context = dbContextFactory.CreateDbContext();

//            return await context.Modules
//                .Where(m => m.member_id == memberId)
//                .Include(m => m.ModuleItems)
//                .ToListAsync();
//        }

//        //// Fetch ModuleItems by moduleId
//        //public async Task<IEnumerable<ModuleItems>> GetModuleItems(int moduleId,
//        //    [Service] IDbContextFactory<workoutContext> dbContextFactory)
//        //{
//        //    _logger.LogInformation("Fetching ModuleItems with moduleId: {moduleId}", moduleId);
//        //    try
//        //    {

//        //        // Create a new DbContext instance using factory
//        //        await using var context = dbContextFactory.CreateDbContext();

//        //        var moduleItemsResult = await context.ModuleItems
//        //            .Where(mi => mi.module_id == moduleId)
//        //            .Select(mi => new ModuleItems
//        //            {
//        //                id = mi.id,
//        //                exercise_id = mi.exercise_id,
//        //                module_id = mi.module_id, 
//        //                reps = mi.reps,
//        //                sets = mi.sets,
//        //                weight = mi.weight
//        //            })
//        //            .ToListAsync();

//        //        if (moduleItemsResult == null || !moduleItemsResult.Any())
//        //        {
//        //            _logger.LogWarning("No ModuleItems found for moduleId: {moduleId}", moduleId);
//        //        }

//        //        return moduleItemsResult;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "An error occurred while fetching ModuleItems for moduleId: {moduleId}", moduleId);
//        //        throw; // Rethrow to let GraphQL handle
//        //    }
//        //}
//    }
//}

namespace WebApplication_punchFit.graphql
{
    // .AddTypes() in Program.cs will automatically register this class by HotChocolate 
    [QueryType]
    public class Query
    {
        private readonly ILogger<Query> _logger;
        private readonly workoutContext _context;

        public Query(ILogger<Query> logger, workoutContext context)//, AuthService authService
        {
            _logger = logger;
            _context = context;
            // _authService = authService;
        }

        //public async Task<Parts> GetPartById(int parts_id)
        //{
        //    await using var context = dbContextFactory.CreateDbContext();
        //    return await context.Parts.FirstOrDefaultAsync(p => p.id == parts_id);
        //}

        //Fetch Exercises
        //public async Task<IEnumerable<Exercises>> GetExercises(
        //    int? exercise_id,
        //    [Service] IDbContextFactory<workoutContext> dbContextFactory)
        //{
        //    _logger.LogInformation("Fetching Exercises");
        //    try
        //    {
        //        await using var context = dbContextFactory.CreateDbContext();

        //        IQueryable<Exercises> query = context.Exercises
        //           .Include(e => e.parts);

        //        if (exercise_id.HasValue)
        //        {
        //            query = query.Where(e => e.id == exercise_id.Value);
        //        }

        //        var exercisesResult = await query.ToListAsync();

        //        if (exercisesResult == null || !exercisesResult.Any())
        //        {
        //            _logger.LogWarning("No Exercises found");
        //        }

        //        return exercisesResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while fetching Exercises");
        //        throw; // Rethrow to let GraphQL handle
        //    }
        //}

        public async Task<IEnumerable<ModuleItems>> GetModuleItems(int moduleId, [Service] IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var memberIdClaim = user.FindFirst("id");
            if (memberIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim not found.");
            }
            var memberId = int.Parse(memberIdClaim.Value);

            _logger.LogInformation("Fetching ModuleItems with moduleId: {moduleId}", moduleId);

            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.id == moduleId);

            if (module == null)
            {
                _logger.LogWarning("Module not found for moduleId: {moduleId}", moduleId);
                throw new UnauthorizedAccessException("Module not found.");
            }

            if (module.member_id != memberId)
            {
                _logger.LogWarning("User is not authorized to access moduleId: {moduleId}", moduleId);
                throw new UnauthorizedAccessException("User is not authorized to access this module.");
            }

            return await _context.ModuleItems
                .Where(mi => mi.module_id == moduleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Modules>> GetModules([Service] IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var memberIdClaim = user.FindFirst("id");
            if (memberIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID claim not found.");
            }
            var memberId = int.Parse(memberIdClaim.Value);

            _logger.LogInformation("Fetching Modules with memberId: {memberId}", memberId);

            return await _context.Modules
                .Where(m => m.member_id == memberId)
                .Include(m => m.ModuleItems)
                .ToListAsync();
        }

        //// Fetch ModuleItems by moduleId
        //public async Task<IEnumerable<ModuleItems>> GetModuleItems(int moduleId,
        //    [Service] IDbContextFactory<workoutContext> dbContextFactory)
        //{
        //    _logger.LogInformation("Fetching ModuleItems with moduleId: {moduleId}", moduleId);
        //    try
        //    {

        //        // Create a new DbContext instance using factory
        //        await using var context = dbContextFactory.CreateDbContext();

        //        var moduleItemsResult = await context.ModuleItems
        //            .Where(mi => mi.module_id == moduleId)
        //            .Select(mi => new ModuleItems
        //            {
        //                id = mi.id,
        //                exercise_id = mi.exercise_id,
        //                module_id = mi.module_id, 
        //                reps = mi.reps,
        //                sets = mi.sets,
        //                weight = mi.weight
        //            })
        //            .ToListAsync();

        //        if (moduleItemsResult == null || !moduleItemsResult.Any())
        //        {
        //            _logger.LogWarning("No ModuleItems found for moduleId: {moduleId}", moduleId);
        //        }

        //        return moduleItemsResult;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while fetching ModuleItems for moduleId: {moduleId}", moduleId);
        //        throw; // Rethrow to let GraphQL handle
        //    }
        //}
    }
}