using GraphQLDemo.Schema.Filters;
using GraphQLDemo.Schema.Sorters;
using GraphQLDemo.Services.Courses;
using GraphQLDemo.Services;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Schema.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class CourseQuery
    {
        private readonly CoursesRepository _coursesRepository;

        public CourseQuery(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }
        
        [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        public async Task<IEnumerable<CourseType>> GetCourses()
        {           
            var courseDtos = await _coursesRepository.GetAll();

            return courseDtos.Select(c => new CourseType
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId             
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        [UseSorting(typeof(CourseSortType))]
        public IQueryable<CourseType> GetPaginatingCourses([ScopedService] SchoolDbContext context)
        {          
            return context.Courses.Select(c => new CourseType
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId               
            });
        }

        public async Task<CourseType> GetCourseById(Guid id)
        {
            var courseDto = await _coursesRepository.GetById(id);

            return new CourseType
            {
                Id = courseDto!.Id,
                Name = courseDto.Name,
                Subject = courseDto.Subject,
                InstructorId = courseDto.InstructorId             
            };
        }

        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<IEnumerable<ISearchResultType>> Search(string term, [ScopedService] SchoolDbContext context)
        {
            IEnumerable<CourseType> courses = await context.Courses
                .Where(c => !string.IsNullOrEmpty(c.Name) && c.Name.Contains(term))
                .Select(c => new CourseType
                {
                    Id = c.Id,
                    Name = c.Name,
                    Subject = c.Subject,
                    InstructorId = c.InstructorId
                })
                .ToListAsync();

            IEnumerable<InstructorType> instructors = await context.Instructors
                .Where(c => (!string.IsNullOrEmpty(c.FirstName) && c.FirstName.Contains(term)) ||
                            (!string.IsNullOrEmpty(c.LastName) && c.LastName.Contains(term))
                    )
                .Select(c => new InstructorType
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Salary = c.Salary
                })
                .ToListAsync();

            return new List<ISearchResultType>()
                .Concat(courses)
                .Concat(instructors);
        }
    }
}
