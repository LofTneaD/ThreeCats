using ThreeCats.Domain.Enums;

namespace ThreeCats.Application.DTOs.Pets;

public sealed record UpdatePetDto(
    Guid Id,
    string Name,
    string Species,
    string Breed,
    PetSex Sex,
    DateOnly BirthDate,
    decimal? Weight,
    string? Notes);
