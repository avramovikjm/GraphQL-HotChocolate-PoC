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

        //[UsePaging(IncludeTotalCount = true, DefaultPageSize = 1)]
        [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 1)]
        public async Task<IEnumerable<CourseType>> GetCourses()
        {
            //return _courseFaker.Generate(5);
            var courseDtos = await _coursesRepository.GetAll();

            return courseDtos.Select(c => new CourseType
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId
                //Instructor = new InstructorType
                //{
                //    Id = c.Instructor!.Id,
                //    FirstName = c.Instructor!.FirstName,
                //    LastName = c.Instructor!.LastName,
                //    Salary = c.Instructor!.Salary,
                //}
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 2)]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        [UseSorting(typeof(CourseSortType))]
        public IQueryable<CourseType> GetPaginatingCourses([ScopedService] SchoolDbContext context)
        {
            //return _courseFaker.Generate(5);
            //var courseDtos = await _coursesRepository.GetAll();

            return context.Courses.Select(c => new CourseType
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId
                //Instructor = new InstructorType
                //{
                //    Id = c.Instructor!.Id,
                //    FirstName = c.Instructor!.FirstName,
                //    LastName = c.Instructor!.LastName,
                //    Salary = c.Instructor!.Salary,
                //}
            });
        }

        public async Task<CourseType> GetCourseById(Guid id)
        {
            //await Task.Delay(1000);
            //var course = _courseFaker.Generate();
            //course.Id = id;
            //return course;

            var courseDto = await _coursesRepository.GetById(id);

            return new CourseType
            {
                Id = courseDto!.Id,
                Name = courseDto.Name,
                Subject = courseDto.Subject,
                InstructorId = courseDto.InstructorId//,
                //Instructor = new InstructorType
                //{
                //    Id = courseDto.Instructor!.Id,
                //    FirstName = courseDto.Instructor!.FirstName,
                //    LastName = courseDto.Instructor!.LastName,
                //    Salary = courseDto.Instructor!.Salary,
                //}
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
