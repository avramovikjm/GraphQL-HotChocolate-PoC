using GraphQLDemo.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services.Courses
{
    public class CoursesRepository
    {
        private readonly IDbContextFactory<SchoolDbContext> _contextFactory;

        public CoursesRepository(IDbContextFactory<SchoolDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<CourseDTO>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();
        }

        public async Task<CourseDTO?> GetById(Guid courseId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Students)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<CourseDTO> Create(CourseDTO course)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Add(course);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return course;
        }

        public async Task<CourseDTO> Update(CourseDTO course)
        {
            using var context = _contextFactory.CreateDbContext();
            context.Update(course);
            await context.SaveChangesAsync().ConfigureAwait(false);
            return course;
        }

        public async Task<bool> Delete(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            var course = new CourseDTO { Id = id };
            context.Courses.Remove(course);
            return await context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }
    }
}
