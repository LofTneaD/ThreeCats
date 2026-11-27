using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Pets;

public sealed record PetDto(
    Guid Id,
    Guid ClientId,
    string Name,
    string Species,
    string Breed,
    PetSex Sex,
    DateOnly BirthDate,
    decimal? Weight,
    string? Notes);
