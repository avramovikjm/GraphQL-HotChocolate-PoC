using AppAny.HotChocolate.FluentValidation;
using GraphQLDemo.DTOs;
using GraphQLDemo.Middlewares.UseUser;
using GraphQLDemo.Models;
using GraphQLDemo.Schema.Subscriptions;
using GraphQLDemo.Services.Courses;
using GraphQLDemo.Validators;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;

namespace GraphQLDemo.Schema.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class CourseMutation
    {       
        private readonly CoursesRepository _coursesRepository;
        private readonly CourseTypeInputValidator _courseTypeInputValidator;
        
        public CourseMutation(CoursesRepository coursesRepository, CourseTypeInputValidator courseTypeInputValidator)
        {           
            _coursesRepository = coursesRepository;
            _courseTypeInputValidator = courseTypeInputValidator;
        }

        [Authorize]
        [UseUser]
        public async Task<CourseResult> CreateCourse(
            [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseInputType courseInput, 
            [Service] ITopicEventSender topicEventSender, 
            [User] User user)
        {
            var courseDto = new CourseDTO
            {
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId,
                CreatorId = user.Id
            };

            courseDto = await _coursesRepository.Create(courseDto);

            var courseType = new CourseResult
            {
                Id = courseDto.Id,
                Name = courseInput.Name,
                Subject = courseInput.Subject,
                InstructorId = courseInput.InstructorId
            };
         
            await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), courseType);
            return courseType;
        }

        [Authorize]
        [UseUser]
        public async Task<CourseResult> UpdateCourse(Guid id,
            [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseInputType courseInput, 
            [Service] ITopicEventSender topicEventSender,
            [User] User user)
        {
            string userId = user.Id ?? string.Empty;

            var currentCourseDto = await _coursesRepository.GetById(id);

            if (currentCourseDto == null)
            {
                throw new GraphQLException(new Error("Course not found.", "COURSE_NOT_FOUND"));
            }

            if(currentCourseDto?.CreatorId != userId)
            {
                throw new GraphQLException(new Error("You do not have permissions to update this course.", "INVALID_PERMISSIONS"));
            }

            currentCourseDto.Id = id;
            currentCourseDto.Name = courseInput.Name;
            currentCourseDto.Subject = courseInput.Subject;
            currentCourseDto.InstructorId = courseInput.InstructorId;

            currentCourseDto = await _coursesRepository.Update(currentCourseDto);
            var course = new CourseResult
            {
               Id = currentCourseDto.Id,
               InstructorId = currentCourseDto.InstructorId,
               Name = courseInput.Name,
               Subject = courseInput.Subject,   
            };

           
            string updateCourseTopic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
            await topicEventSender.SendAsync(updateCourseTopic, course);

            return course;
        }

        [Authorize(Policy = "IsAdmin")]
        public async Task<bool> DeleteCourse(Guid id)
        {            
            return await _coursesRepository.Delete(id);
        }
    }
}
