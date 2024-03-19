using GraphQLDemo.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services.Instructors
{
    public class InstructorsRepository
    {
        private readonly IDbContextFactory<SchoolDbContext> _contextFactory;

        public InstructorsRepository(IDbContextFactory<SchoolDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<InstructorDTO?> GetById(Guid instructorId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Instructors.FirstOrDefaultAsync(c => c.Id == instructorId);
        }

        public async Task<IEnumerable<InstructorDTO>> GetManyByIds(IReadOnlyList<Guid> instructorIds)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Instructors
                .Where(c => instructorIds.Contains(c.Id))
                .ToListAsync();
        }
    }
}
