namespace ThreadSpace.API.DTOs;

public class CreatePostDto
{
    public int TagId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
