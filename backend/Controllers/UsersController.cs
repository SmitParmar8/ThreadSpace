using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadSpace.API.Data;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db) => _db = db;

    [HttpGet("{username}")]
    public IActionResult GetUser(string username)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return NotFound(new { message = "User not found" });

        return Ok(new { user.Id, user.Username, user.Name, user.Email, user.UserLevel, user.Role, user.CreatedAt });
    }

    [HttpGet("{username}/posts")]
    public IActionResult GetUserPosts(string username)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return NotFound(new { message = "User not found" });

        var posts = _db.Posts
            .Include(p => p.Tag)
            .Include(p => p.Votes)
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                Tag = new { p.Tag.Id, p.Tag.Name },
                VoteScore = p.Votes.Count(v => v.IsLike) - p.Votes.Count(v => !v.IsLike),
                p.CreatedAt
            })
            .ToList();

        return Ok(posts);
    }
}
