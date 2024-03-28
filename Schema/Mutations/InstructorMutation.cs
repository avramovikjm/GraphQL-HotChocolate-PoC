using AppAny.HotChocolate.FluentValidation;
using GraphQLDemo.DTOs;
using GraphQLDemo.Services;
using GraphQLDemo.Validators;
using HotChocolate.Authorization;

namespace GraphQLDemo.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class InstructorMutation
    {
        [Authorize]
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorResult> CreateInstructor(
            [UseFluentValidation, UseValidator<InstructorTypeInputValidator>] InstructorTypeInput instructorInput,
            [ScopedService] SchoolDbContext context)
        {
            var instructorDTO = new InstructorDTO()
            {
                FirstName = instructorInput.FirstName,
                LastName = instructorInput.LastName,
                Salary = instructorInput.Salary
            };
        
            context.Add(instructorDTO);
            await context.SaveChangesAsync();

            var instructorResult = new InstructorResult()
            {
                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName               
            };

            return instructorResult;
        }
        [Authorize]
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<InstructorResult> UpdateInstructor(
           Guid id,
           [UseFluentValidation, UseValidator<InstructorTypeInputValidator>] InstructorTypeInput instructorInput,
           [ScopedService] SchoolDbContext context)
        {
            var instructorDTO = await context.Instructors.FindAsync(id);

            if (instructorDTO == null)
            {
                throw new GraphQLException(new Error("Instructor not found", "INSTRUCTOR_NOT_FOUND"));
            }

            instructorDTO.FirstName = instructorInput.FirstName;
            instructorDTO.LastName = instructorInput.LastName;   
            instructorDTO.Salary = instructorInput.Salary;

            context.Update(instructorDTO);
            await context.SaveChangesAsync();

            var instructorResult = new InstructorResult()
            {
                Id = instructorDTO.Id,
                FirstName = instructorDTO.FirstName,
                LastName = instructorDTO.LastName
              
            };

            return instructorResult;
        }

        [Authorize(Policy = "IsAdmin")]        
        [UseDbContext(typeof(SchoolDbContext))]
        public async Task<bool> DeleteInstructor(Guid id, [ScopedService] SchoolDbContext context)
        {
            var instructorDTO = new InstructorDTO()
            {
                Id = id
            };

            context.Remove(instructorDTO);

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
            }

            
        }
    }
}
