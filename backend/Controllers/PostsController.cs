using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ThreadSpace.API.Data;
using ThreadSpace.API.DTOs;
using ThreadSpace.API.Models;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PostsController(AppDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public IActionResult GetPosts()
    {
        var posts = _db.Posts
            .Include(p => p.User)
            .Include(p => p.Tag)
            .Include(p => p.Votes)
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostDto dto)
    {
        var userId = GetUserId();

        if (!_db.Tags.Any(t => t.Id == dto.TagId))
            return BadRequest(new { message = "Tag not found" });

        var post = new Post
        {
            UserId = userId,
            TagId = dto.TagId,
            Title = dto.Title,
            Content = dto.Content
        };

        _db.Posts.Add(post);
        await _db.SaveChangesAsync();

        return StatusCode(201, new { post.Id, post.Title, post.Content, post.TagId, VoteScore = 0, post.CreatedAt });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, UpdatePostDto dto)
    {
        var userId = GetUserId();
        var post = _db.Posts.Find(id);

        if (post == null) return NotFound(new { message = "Post not found" });
        if (post.UserId != userId) return Forbid();

        if (dto.Title != null) post.Title = dto.Title;
        if (dto.Content != null) post.Content = dto.Content;
        if (dto.TagId.HasValue)
        {
            if (!_db.Tags.Any(t => t.Id == dto.TagId))
                return BadRequest(new { message = "Tag not found" });
            post.TagId = dto.TagId.Value;
        }

        await _db.SaveChangesAsync();
        return Ok(new { post.Id, post.Title, post.Content, post.TagId, post.CreatedAt });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var userId = GetUserId();
        var post = _db.Posts.Find(id);

        if (post == null) return NotFound(new { message = "Post not found" });
        if (post.UserId != userId) return Forbid();

        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Post deleted" });
    }

    [Authorize]
    [HttpPost("{id}/vote")]
    public async Task<IActionResult> Vote(int id, VoteDto dto)
    {
        var userId = GetUserId();
        var post = _db.Posts.Include(p => p.Votes).FirstOrDefault(p => p.Id == id);

        if (post == null) return NotFound(new { message = "Post not found" });

        var existing = post.Votes.FirstOrDefault(v => v.UserId == userId);
        if (existing != null)
        {
            existing.IsLike = dto.IsLike;
        }
        else
        {
            _db.Votes.Add(new Vote { PostId = id, UserId = userId, IsLike = dto.IsLike });
        }

        await _db.SaveChangesAsync();
        var voteScore = _db.Votes.Where(v => v.PostId == id).Sum(v => v.IsLike ? 1 : -1);
        return Ok(new { message = "Vote recorded", voteScore });
    }

    [Authorize]
    [HttpDelete("{id}/vote")]
    public async Task<IActionResult> RemoveVote(int id)
    {
        var userId = GetUserId();
        var vote = _db.Votes.FirstOrDefault(v => v.PostId == id && v.UserId == userId);

        if (vote == null) return NotFound(new { message = "Vote not found" });

        _db.Votes.Remove(vote);
        await _db.SaveChangesAsync();
        var voteScore = _db.Votes.Where(v => v.PostId == id).Sum(v => v.IsLike ? 1 : -1);
        return Ok(new { message = "Vote removed", voteScore });
    }
}
