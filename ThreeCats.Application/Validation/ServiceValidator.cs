using System.ComponentModel.DataAnnotations;
using ThreeCats.Application.DTOs.Services;

namespace ThreeCats.Application.Validation;

public static class ServiceValidator
{
    private const int MaxNameLength = 150;
    private const int MaxDescriptionLength = 2000;

    private const decimal MinPrice = 0.01m;
    private const decimal MaxPrice = 99_999_999.99m;

    public static void Validate(CreateServiceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Validate(
            dto.Name,
            dto.Description,
            dto.Price);
    }

    public static void Validate(UpdateServiceDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Validate(
            dto.Name,
            dto.Description,
            dto.Price);
    }

    private static void Validate(
        string? name,
        string? description,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException(
                "Введите название услуги.");
        }

        if (name.Trim().Length > MaxNameLength)
        {
            throw new ValidationException(
                $"Название услуги не должно превышать {MaxNameLength} символов.");
        }

        if (description?.Length > MaxDescriptionLength)
        {
            throw new ValidationException(
                $"Описание услуги не должно превышать {MaxDescriptionLength} символов.");
        }

        if (price < MinPrice)
        {
            throw new ValidationException(
                "Стоимость услуги должна быть больше нуля.");
        }

        if (price > MaxPrice)
        {
            throw new ValidationException(
                $"Стоимость услуги не должна превышать {MaxPrice:N2}.");
        }

        if (decimal.Round(price, 2) != price)
        {
            throw new ValidationException(
                "Стоимость услуги не должна содержать больше двух знаков после запятой.");
        }
    }
}
