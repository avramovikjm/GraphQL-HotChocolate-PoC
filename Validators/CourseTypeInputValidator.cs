using FluentValidation;
using GraphQLDemo.Schema.Mutations;

namespace GraphQLDemo.Validators
{
    public class CourseTypeInputValidator : AbstractValidator<CourseInputType>
    {
        public CourseTypeInputValidator()
        {
            RuleFor(c => c.Name)
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Course name must be between 3 and 50 characters.")
                .WithErrorCode("COURSE_NAME_LENGTH");
        }
    }
}
