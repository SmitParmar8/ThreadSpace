using Microsoft.AspNetCore.Mvc;
using ThreadSpace.API.Data;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/ai-posts")]
public class AiPostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AiPostsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAiPosts()
    {
        var posts = _db.AiPosts
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new { p.Id, p.Title, p.Content, p.AiModel, p.CreatedAt })
            .ToList();

        return Ok(posts);
    }
}
