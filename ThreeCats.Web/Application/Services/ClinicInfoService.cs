using Microsoft.Extensions.Options;
using ThreeCats.Web.Configuration;
using ThreeCats.Web.Application.Interfaces;

namespace ThreeCats.Web.Application.Services;

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
