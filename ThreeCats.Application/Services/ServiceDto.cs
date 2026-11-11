namespace ThreeCats.Application.Services
{
    public sealed record ServiceDto(
        Guid Id,
        string Name,
        string Desctiption,
        decimal Price,
        bool IsActive);
}
