using FluentValidation;
using GraphQLDemo.Schema.Mutations;

namespace GraphQLDemo.Validators
{
    public class InstructorTypeInputValidator : AbstractValidator<InstructorTypeInput>
    {
        public InstructorTypeInputValidator()
        {
            RuleFor(i => i.FirstName).NotEmpty();
            RuleFor(i => i.LastName).NotEmpty();
        }
    }
}
