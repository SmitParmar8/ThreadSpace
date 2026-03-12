using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreadSpace.API.Data;
using ThreadSpace.API.Models;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(new
        {
            totalUsers = _db.Users.Count(),
            totalPosts = _db.Posts.Count(),
            totalTags = _db.Tags.Count()
        });
    }

    [HttpGet("tags")]
    public IActionResult GetTags()
    {
        var tags = _db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name, postCount = _db.Posts.Count(p => p.TagId == t.Id) })
            .ToList();
        return Ok(tags);
    }

    [HttpPost("tags")]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Tag name is required" });

        var name = dto.Name.Trim();
        if (_db.Tags.Any(t => t.Name == name))
            return BadRequest(new { message = "Tag already exists" });

        var tag = new Tag { Name = name };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        return Ok(new { tag.Id, tag.Name, postCount = 0 });
    }

    [HttpDelete("tags/{id}")]
    public async Task<IActionResult> DeleteTag(int id)
    {
        var tag = _db.Tags.Find(id);
        if (tag == null) return NotFound(new { message = "Tag not found" });

        if (_db.Posts.Any(p => p.TagId == id))
            return BadRequest(new { message = "Cannot delete tag — it is used by existing posts" });

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Tag deleted" });
    }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
}
