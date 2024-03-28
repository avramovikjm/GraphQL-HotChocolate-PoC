using AppAny.HotChocolate.FluentValidation;
using FirebaseAdmin;
using FirebaseAdminAuthentication.DependencyInjection.Extensions;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using FluentValidation.AspNetCore;
using GraphQLDemo.DataLoaders;
using GraphQLDemo.Schema.Mutations;
using GraphQLDemo.Schema.Queries;
using GraphQLDemo.Schema.Subscriptions;
using GraphQLDemo.Services;
using GraphQLDemo.Services.Courses;
using GraphQLDemo.Services.Instructors;
using GraphQLDemo.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddTransient<CourseTypeInputValidator>();
builder.Services.AddTransient<InstructorTypeInputValidator>();
string connectionString = builder.Configuration.GetConnectionString("default")!;
builder.Services.AddPooledDbContextFactory<SchoolDbContext>(o => o.UseSqlite(connectionString).LogTo(Console.WriteLine));
builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()    
    .AddSubscriptionType<Subscription>()
    .AddType<CourseType>()
    .AddType<InstructorType>()
    .AddTypeExtension<CourseQuery>()
    .AddTypeExtension<InstructionQuery>()
    .AddTypeExtension<CourseMutation>()
    .AddTypeExtension<InstructorMutation>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddInMemorySubscriptions()
    .AddFluentValidation(o =>
    {
        //o.UseDefaultErrorMapper();
    });

builder.Services.AddSingleton(FirebaseApp.Create());
builder.Services.AddFirebaseAuthentication();
builder.Services.AddAuthorization(o => o.AddPolicy("IsAdmin", p =>
{
    p.RequireClaim(FirebaseUserClaimType.EMAIL, "");
}));

builder.Services.AddScoped<CoursesRepository>();
builder.Services.AddScoped<InstructorsRepository>();
builder.Services.AddScoped<InstructorDataLoader>();

var app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<SchoolDbContext>>();
using var context = contextFactory.CreateDbContext();
context.Database.Migrate();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();

app.MapGraphQL();

app.Run();