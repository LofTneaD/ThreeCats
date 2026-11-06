namespace ThreeCats.Application.Options;
public sealed class ClinicOptions
{
    public const string SectionName = "Clinic";
    public string Name { get; set; } = "ThreeCats";
    public string SupportEmail { get; set; } = string.Empty;
}
