using GraphQLDemo.Schema.Filters;
using GraphQLDemo.Schema.Sorters;
using GraphQLDemo.Services;
using GraphQLDemo.Services.Courses;

namespace GraphQLDemo.Schema.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class InstructionQuery
    {
        [UseDbContext(typeof(SchoolDbContext))]     
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 2)]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<InstructorType> GetInstructors([ScopedService] SchoolDbContext context)
        {
            return context.Instructors.Select(i => new InstructorType
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,  
                Salary = i.Salary,
            });
        }


        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorType> GetInstructorById(Guid id, [ScopedService] SchoolDbContext context)
        {
            var instructorDto = await context.Instructors.FindAsync(id);

            if (instructorDto == null)
            {
                return null!;
            }

            return new InstructorType
            {
                Id = instructorDto.Id,
                FirstName = instructorDto.FirstName,
                LastName = instructorDto.LastName,
                Salary = instructorDto.Salary
            };
        }

    }
}
