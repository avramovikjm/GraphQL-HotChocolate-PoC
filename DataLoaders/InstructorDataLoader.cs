using GraphQLDemo.DTOs;
using GraphQLDemo.Services.Instructors;

namespace GraphQLDemo.DataLoaders
{
    public class InstructorDataLoader : BatchDataLoader<Guid, InstructorDTO>
    {
        private readonly InstructorsRepository _instructorsRepository;

        public InstructorDataLoader(
            InstructorsRepository instructorRepository, 
            IBatchScheduler batchScheduler, 
            DataLoaderOptions? options = null) 
            : base(batchScheduler, options)
        {
            _instructorsRepository = instructorRepository;
        }

        protected override async Task<IReadOnlyDictionary<Guid, InstructorDTO>> LoadBatchAsync(IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
        {
            var instructors = await _instructorsRepository.GetManyByIds(keys);
            return instructors.ToDictionary(i => i.Id);
        }
    }
}
