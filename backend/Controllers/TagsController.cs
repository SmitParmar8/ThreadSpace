using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThreadSpace.API.Data;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TagsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetTags()
    {
        var tags = _db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name, t.CreatedAt })
            .ToList();

        return Ok(tags);
    }

    [HttpGet("{id}/posts")]
    public IActionResult GetPostsByTag(int id)
    {
        if (!_db.Tags.Any(t => t.Id == id))
            return NotFound(new { message = "Tag not found" });

        var posts = _db.Posts
            .Include(p => p.User)
            .Include(p => p.Tag)
            .Include(p => p.Votes)
            .Where(p => p.TagId == id)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                Author = new { p.User.Username, p.User.UserLevel },
                Tag = new { p.Tag.Id, p.Tag.Name },
                VoteScore = p.Votes.Count(v => v.IsLike) - p.Votes.Count(v => !v.IsLike),
                p.CreatedAt
            })
            .ToList();

        return Ok(posts);
    }
}
