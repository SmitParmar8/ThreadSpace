using ThreadSpace.API.Models;

namespace ThreadSpace.API.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureCreated();
        SeedTags(db);
        SeedAiPosts(db);
        SeedAdminUser(db);
        SeedSampleUsers(db);
        SeedSamplePosts(db);
    }

    private static void SeedTags(AppDbContext db)
    {
        if (db.Tags.Any()) return;

        db.Tags.AddRange(
            new Tag { Name = "General" },
            new Tag { Name = "JavaScript" },
            new Tag { Name = "C#" },
            new Tag { Name = "Python" },
            new Tag { Name = "DevOps" },
            new Tag { Name = "Database" },
            new Tag { Name = "Career" }
        );
        db.SaveChanges();
    }

    private static void SeedAdminUser(AppDbContext db)
    {
        if (db.Users.Any(u => u.Username == "admin")) return;

        db.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@threadspace.dev",
            Username = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("admin"),
            UserLevel = "Pro",
            Role = "Admin"
        });
        db.SaveChanges();
    }

    private static void SeedSampleUsers(AppDbContext db)
    {
        if (db.Users.Count() > 1) return;

        db.Users.AddRange(
            new User
            {
                Name = "Alex Rivera",
                Email = "alex@example.com",
                Username = "alexr",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserLevel = "Mid-Level",
                Role = "User"
            },
            new User
            {
                Name = "Priya Sharma",
                Email = "priya@example.com",
                Username = "priya_dev",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserLevel = "Pro",
                Role = "User"
            },
            new User
            {
                Name = "Marcus Chen",
                Email = "marcus@example.com",
                Username = "mchen",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserLevel = "Fresher",
                Role = "User"
            },
            new User
            {
                Name = "Sarah Kim",
                Email = "sarah@example.com",
                Username = "sarahk",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                UserLevel = "Student",
                Role = "User"
            }
        );
        db.SaveChanges();
    }

    private static void SeedSamplePosts(AppDbContext db)
    {
        if (db.Posts.Any()) return;

        var jsTag = db.Tags.First(t => t.Name == "JavaScript");
        var csTag = db.Tags.First(t => t.Name == "C#");
        var pyTag = db.Tags.First(t => t.Name == "Python");
        var devopsTag = db.Tags.First(t => t.Name == "DevOps");
        var dbTag = db.Tags.First(t => t.Name == "Database");
        var careerTag = db.Tags.First(t => t.Name == "Career");
        var generalTag = db.Tags.First(t => t.Name == "General");

        var admin = db.Users.First(u => u.Username == "admin");
        var alex = db.Users.First(u => u.Username == "alexr");
        var priya = db.Users.First(u => u.Username == "priya_dev");
        var marcus = db.Users.First(u => u.Username == "mchen");
        var sarah = db.Users.First(u => u.Username == "sarahk");

        db.Posts.AddRange(
            new Post
            {
                UserId = priya.Id,
                TagId = jsTag.Id,
                Title = "Why I switched from Redux to Zustand for state management",
                Content = "After 3 years of using Redux, I finally made the switch to Zustand on our main product. The boilerplate reduction is insane — what used to take 5 files now takes 20 lines. If you're starting a new React project in 2024, seriously consider Zustand or Jotai before reaching for Redux.",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Post
            {
                UserId = alex.Id,
                TagId = csTag.Id,
                Title = "ASP.NET Core Minimal APIs vs Controllers — which should you use?",
                Content = "Minimal APIs are great for microservices and simple CRUD but Controllers still win for complex apps with filters, versioning, and rich validation pipelines. I've been running both in production and Controllers give you way more structure when the team grows. Use Minimal APIs for side projects, Controllers for enterprise.",
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new Post
            {
                UserId = marcus.Id,
                TagId = pyTag.Id,
                Title = "FastAPI vs Flask in 2024 — a fresher's honest comparison",
                Content = "Coming from Flask, FastAPI blew my mind. Built-in async support, automatic Swagger docs, and Pydantic validation out of the box. Flask is simpler to grasp initially but FastAPI's DX is miles ahead once you get used to type hints. For anything new I'm going FastAPI all the way.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Post
            {
                UserId = priya.Id,
                TagId = devopsTag.Id,
                Title = "Lessons from running Docker in production for 2 years",
                Content = "Top things nobody tells you: always pin your image versions (not :latest), set memory limits on every container, use health checks so orchestrators know when to restart, and keep your images small with multi-stage builds. Also log to stdout/stderr, never to files inside the container.",
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Post
            {
                UserId = alex.Id,
                TagId = dbTag.Id,
                Title = "PostgreSQL indexes I wish I knew about earlier",
                Content = "Partial indexes are underrated — index only the rows you actually query. GIN indexes for JSONB columns are a must if you're storing JSON. And EXPLAIN ANALYZE is your best friend, not a last resort. Most slow queries I've fixed were missing a single index that took 5 minutes to add.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Post
            {
                UserId = sarah.Id,
                TagId = careerTag.Id,
                Title = "What I learned from my first 6 months as a junior dev",
                Content = "Read the docs. Seriously, just read them. Ask questions early before you go down a 3-hour rabbit hole. Your seniors want to help — don't suffer in silence. Also, git blame is your friend, not a tool to shame people. Code review comments are about the code, not you.",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Post
            {
                UserId = admin.Id,
                TagId = generalTag.Id,
                Title = "Welcome to ThreadSpace — community guidelines",
                Content = "Welcome everyone! ThreadSpace is a place for developers to share knowledge, ask questions, and grow together. Keep discussions technical and constructive. Upvote posts that helped you. Tag your posts correctly so others can find them. Let's build something great together.",
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Post
            {
                UserId = marcus.Id,
                TagId = jsTag.Id,
                Title = "TypeScript strict mode broke my code and made it better",
                Content = "Turned on strict mode on a JS-to-TS migration project and had 200+ errors. Spent a week fixing them. Found 3 real bugs in production logic that had been hiding for months. Worth every hour. Enable strict mode from day one on new projects.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Post
            {
                UserId = priya.Id,
                TagId = csTag.Id,
                Title = "Entity Framework Core performance tips that actually matter",
                Content = "1) Use AsNoTracking() for read-only queries — huge win. 2) Never select entire entities when you only need 2 columns, use projections. 3) Batch inserts with AddRange not loop Add. 4) Watch your N+1 queries with Include. 5) Use compiled queries for hot paths. These 5 changes cut our API response times by 40%.",
                CreatedAt = DateTime.UtcNow.AddHours(-8)
            },
            new Post
            {
                UserId = sarah.Id,
                TagId = generalTag.Id,
                Title = "How do you stay motivated when learning gets hard?",
                Content = "Hitting a wall with distributed systems concepts. Everything made sense up to load balancing but now consensus algorithms and CAP theorem are melting my brain. How do you all push through the 'I understand nothing' phase? Any resources that helped it click?",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        );
        db.SaveChanges();
    }

    private static void SeedAiPosts(AppDbContext db)
    {
        if (db.AiPosts.Any()) return;

        db.AiPosts.AddRange(
            new AiPost
            {
                Title = "Understanding Async/Await in Modern Programming",
                Content = "Async/await patterns have revolutionized how we handle asynchronous operations. By allowing code to pause execution without blocking the thread, we can write clean, readable code that handles I/O-bound tasks efficiently.",
                AiModel = "GPT-4"
            },
            new AiPost
            {
                Title = "Best Practices for RESTful API Design",
                Content = "When designing RESTful APIs, consistency and simplicity are key principles. Use nouns for resource names, keep URLs clean and hierarchical, return appropriate HTTP status codes, and always version your API from the start.",
                AiModel = "Claude"
            },
            new AiPost
            {
                Title = "Introduction to Docker Containerization",
                Content = "Docker containers allow developers to package applications with all dependencies into a single portable unit. This eliminates the classic 'it works on my machine' problem and streamlines deployment across different environments.",
                AiModel = "Gemini"
            },
            new AiPost
            {
                Title = "Writing Clean Code: Principles Every Developer Should Know",
                Content = "Clean code is code that is easy to read, understand, and maintain. Follow naming conventions, keep functions small and focused, avoid duplication, and always write code as if the next person to read it is a senior engineer.",
                AiModel = "GPT-4"
            },
            new AiPost
            {
                Title = "Git Branching Strategies for Team Development",
                Content = "Choosing the right branching strategy impacts team velocity significantly. Feature branching, GitFlow, and trunk-based development each have trade-offs. For most teams, trunk-based development with short-lived feature branches offers the best balance.",
                AiModel = "Claude"
            },
            new AiPost
            {
                Title = "The Case for TypeScript in Every JavaScript Project",
                Content = "TypeScript catches entire classes of bugs at compile time that JavaScript won't surface until runtime in production. The initial setup cost is offset within weeks by reduced debugging time. Type inference means you write fewer annotations than you think.",
                AiModel = "GPT-4"
            },
            new AiPost
            {
                Title = "Database Indexing Strategies for High-Traffic Applications",
                Content = "Proper indexing is one of the highest-leverage optimizations available. Composite indexes should match your query patterns exactly. Covering indexes eliminate table lookups entirely. Monitor your slow query log weekly — index needs evolve as traffic patterns change.",
                AiModel = "Claude"
            }
        );
        db.SaveChanges();
    }
}
