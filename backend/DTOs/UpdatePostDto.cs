namespace ThreadSpace.API.DTOs;

public class UpdatePostDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int? TagId { get; set; }
}
