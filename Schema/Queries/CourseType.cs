using GraphQLDemo.DataLoaders;
using GraphQLDemo.Models;
using GraphQLDemo.Services.Instructors;

namespace GraphQLDemo.Schema.Queries
{
    public class CourseType : ISearchResultType
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public Subject Subject { get; set; }

        [IsProjected(true)]
        public Guid InstructorId { get; set; }

        [GraphQLNonNullType]
        public async Task<InstructorType?> Instructor([Service] InstructorDataLoader instructorDataLoader)
        {
            var instructorDto = await instructorDataLoader.LoadAsync(InstructorId, CancellationToken.None);

            return new InstructorType
            {
                FirstName = instructorDto!.FirstName,
                LastName = instructorDto!.LastName,
                Id = instructorDto!.Id,
                Salary = instructorDto!.Salary
            };
        }

        public IEnumerable<StudentType>? Students { get; set; }

        public string Description()
        {
            return "Test2";
        }
    }
}
