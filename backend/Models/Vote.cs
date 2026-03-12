namespace ThreadSpace.API.Models;

public class Vote
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsLike { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
