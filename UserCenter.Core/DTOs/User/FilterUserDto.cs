// DTOs/FilterUserDto.cs
public class FilterUserDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? NickName { get; set; }
    public string? AvatarUrl { get; set; }
    public int? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? UserRole { get; set; }

    public int Current { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

