namespace ThreeCats.Application.Services
{
    public sealed record ServiceDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        bool IsActive);
}
