using Microsoft.Extensions.Options;
using ThreeCats.Application.Interfaces.Services;
using ThreeCats.Application.Options;

namespace ThreeCats.Application.Services;

public sealed class ClinicInfoService : IClinicInfoService
{
    private readonly ClinicOptions _options;

    public ClinicInfoService(IOptions<ClinicOptions> options)
    {
        _options = options.Value;
    }
    public string GetClinicName()
    {
        return _options.Name;
    }
}
