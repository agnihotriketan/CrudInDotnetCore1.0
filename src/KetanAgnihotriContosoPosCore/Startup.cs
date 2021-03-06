﻿using System;
using System.Linq;
using KetanAgnihotriContosoPosCore.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace KetanAgnihotriContosoPosCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
            services.AddDbContext<SchoolContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            /* services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));*/
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            SchoolContext context)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            DbInitializer.Initialize(context);
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Student.Any())
            {
                return; // DB has been seeded
            }

            var students = new Student[]
            {
                new Student
                {
                    FirstMidName = "Carson",
                    LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2005-09-01")
                },
                new Student
                {
                    FirstMidName = "Meredith",
                    LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2002-09-01")
                },
                new Student {FirstMidName = "Arturo", LastName = "Anand", EnrollmentDate = DateTime.Parse("2003-09-01")},
                new Student
                {
                    FirstMidName = "Gytis",
                    LastName = "Barzdukas",
                    EnrollmentDate = DateTime.Parse("2002-09-01")
                },
                new Student {FirstMidName = "Yan", LastName = "Li", EnrollmentDate = DateTime.Parse("2002-09-01")},
                new Student
                {
                    FirstMidName = "Peggy",
                    LastName = "Justice",
                    EnrollmentDate = DateTime.Parse("2001-09-01")
                },
                new Student {FirstMidName = "Laura", LastName = "Norman", EnrollmentDate = DateTime.Parse("2003-09-01")},
                new Student
                {
                    FirstMidName = "Nino",
                    LastName = "Olivetto",
                    EnrollmentDate = DateTime.Parse("2005-09-01")
                }
            };
            foreach (Student s in students)
            {
                context.Student.Add(s);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course {CourseId = 1050, Title = "Chemistry", Credits = 3,},
                new Course {CourseId = 4022, Title = "Microeconomics", Credits = 3,},
                new Course {CourseId = 4041, Title = "Macroeconomics", Credits = 3,},
                new Course {CourseId = 1045, Title = "Calculus", Credits = 4,},
                new Course {CourseId = 3141, Title = "Trigonometry", Credits = 4,},
                new Course {CourseId = 2021, Title = "Composition", Credits = 3,},
                new Course {CourseId = 2042, Title = "Literature", Credits = 4,}
            };
            foreach (Course c in courses)
            {
                context.Course.Add(c);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
                new Enrollment {StudentId = 1, CourseId = 1050, Grade = Grade.A},
                new Enrollment {StudentId = 1, CourseId = 4022, Grade = Grade.C},
                new Enrollment {StudentId = 1, CourseId = 4041, Grade = Grade.B},
                new Enrollment {StudentId = 2, CourseId = 1045, Grade = Grade.B},
                new Enrollment {StudentId = 2, CourseId = 3141, Grade = Grade.F},
                new Enrollment {StudentId = 2, CourseId = 2021, Grade = Grade.F},
                new Enrollment {StudentId = 3, CourseId = 1050},
                new Enrollment {StudentId = 4, CourseId = 1050,},
                new Enrollment {StudentId = 4, CourseId = 4022, Grade = Grade.F},
                new Enrollment {StudentId = 5, CourseId = 4041, Grade = Grade.C},
                new Enrollment {StudentId = 6, CourseId = 1045},
                new Enrollment {StudentId = 7, CourseId = 3141, Grade = Grade.A},
            };
            foreach (Enrollment e in enrollments)
            {
                context.Enrollment.Add(e);
            }
            context.SaveChanges();
        }
    }
}