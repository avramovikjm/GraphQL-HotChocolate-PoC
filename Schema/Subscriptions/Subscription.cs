using GraphQLDemo.Schema.Mutations;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQLDemo.Schema.Subscriptions
{
    public class Subscription
    {
        [Subscribe]
        public CourseResult CourseCreated([EventMessage] CourseResult course) 
        {
            return course;
        }

        public ValueTask<ISourceStream<CourseResult>> SubscribeToCourseUpdated(Guid courseId, [Service] ITopicEventReceiver topicEventReceiver)
        {
            string topicName = $"{courseId}_{nameof(Subscription.CourseUpdated)}";
            return topicEventReceiver.SubscribeAsync<CourseResult>(topicName);
        }

        [Subscribe(With = nameof(SubscribeToCourseUpdated))]
        public CourseResult CourseUpdated(Guid _, [EventMessage] CourseResult course) 
        {
            return course;
        }
    }
}
