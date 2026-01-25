using DAL;
using DTOs.Constants;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API
{
    public static class DataSeeder
    {
        public static async Task SeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // --- 1. SEED USERS ---

                var managerId = "11111111-1111-1111-1111-111111111111"; // ID Cố định để FE không bị lỗi

                // Hash mật khẩu 123456
                var defaultPasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");

                var managerUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "Manager@gmail.com");
                if (managerUser == null)
                {
                    managerUser = new User
                    {
                        Id = managerId,
                        Email = "Manager@gmail.com",
                        FullName = "Manager Boss",
                        Role = UserRoles.Manager,
                        PasswordHash = defaultPasswordHash,
                        IsActive = true
                        // Đã xóa các trường UserName, NormalizedEmail... gây lỗi
                    };
                    context.Users.Add(managerUser);
                }

                // Tạo Annotators
                var annotators = new List<User>();
                for (int i = 1; i <= 5; i++)
                {
                    var staffId = $"22222222-2222-2222-2222-22222222222{i}";
                    var email = $"Staff{i}@gmail.com";

                    var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (user == null)
                    {
                        user = new User
                        {
                            Id = staffId,
                            Email = email,
                            FullName = $"Staff Annotator {i}",
                            Role = UserRoles.Annotator,
                            PasswordHash = defaultPasswordHash,
                            IsActive = true
                        };
                        context.Users.Add(user);
                        annotators.Add(user);
                    }
                    else
                    {
                        annotators.Add(user);
                    }
                }

                await context.SaveChangesAsync();

                // --- 2. SEED PROJECTS & DATA ---
                if (!context.Projects.Any())
                {
                    var projects = new List<Project>();

                    for (int p = 1; p <= 3; p++)
                    {
                        var project = new Project
                        {
                            Name = $"Dự án Gán Nhãn Xe Hơi {p}",
                            Description = "Dự án test dữ liệu cho FE team.",
                            ManagerId = managerUser.Id,
                            CreatedDate = DateTime.UtcNow,
                            StartDate = DateTime.UtcNow,
                            EndDate = DateTime.UtcNow.AddDays(30),
                            PricePerLabel = 1000 + (p * 100),
                            TotalBudget = 1000000,
                            Deadline = DateTime.UtcNow.AddDays(10 + p),
                            AllowGeometryTypes = "Rectangle"
                        };

                        var labels = new List<LabelClass>
                        {
                            new LabelClass { Name = "Car", Color = "#FF0000", GuideLine = "Vẽ bao quanh xe" },
                            new LabelClass { Name = "Bike", Color = "#00FF00", GuideLine = "Vẽ bao quanh xe đạp" },
                            new LabelClass { Name = "Bus", Color = "#0000FF", GuideLine = "Vẽ xe buýt" }
                        };
                        project.LabelClasses = labels;

                        var dataItems = new List<DataItem>();
                        for (int d = 1; d <= 10; d++)
                        {
                            dataItems.Add(new DataItem
                            {
                                StorageUrl = $"https://via.placeholder.com/600x400?text=Project{p}_Image{d}",
                                Status = "New",
                                UploadedDate = DateTime.UtcNow,
                                MetaData = "{}"
                            });
                        }

                        // Giao việc
                        int staffIndex = 0;
                        for (int k = 0; k < 8; k++)
                        {
                            var item = dataItems[k];
                            item.Status = "Assigned";

                            var assignedStaff = annotators[staffIndex % annotators.Count];

                            var status = "Assigned";
                            if (k == 1) status = "InProgress";
                            if (k >= 2 && k < 4) status = "Submitted";
                            if (k >= 4) status = "Rejected";

                            var assignment = new Assignment
                            {
                                Project = project,
                                DataItem = item,
                                AnnotatorId = assignedStaff.Id,
                                Status = status,
                                AssignedDate = DateTime.UtcNow,
                                SubmittedAt = (status == "Submitted" || status == "Rejected") ? DateTime.UtcNow : null
                            };

                            // Sửa lỗi WrongLabel -> IncorrectLabel
                            if (status == "Rejected")
                            {
                                assignment.ReviewLogs = new List<ReviewLog>
                                {
                                    new ReviewLog
                                    {
                                        ReviewerId = managerId,
                                        Verdict = "Rejected",
                                        // Dùng hằng số có sẵn trong code của bạn
                                        ErrorCategory = ErrorCategories.IncorrectLabel,
                                        Comment = "Vẽ sai rồi, đây là xe máy không phải xe đạp!",
                                        CreatedAt = DateTime.UtcNow
                                    }
                                };
                            }

                            if (item.Assignments == null) item.Assignments = new List<Assignment>();
                            item.Assignments.Add(assignment);

                            staffIndex++;
                        }

                        project.DataItems = dataItems;
                        projects.Add(project);
                    }

                    context.Projects.AddRange(projects);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}