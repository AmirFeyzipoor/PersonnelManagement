using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PersonnelManagement.UseCases.AdminServices.IpBlocking.Configs;
using PersonnelManagement.UseCases.AdminServices.IpBlocking.Contracts;

namespace PersonnelManagement.UseCases.AdminServices.IpBlocking;

public class IpBlockingService: IIpBlockingService
{
    private readonly List<string> _blockedIps;

    public IpBlockingService(string blockedIps)
    {
        _blockedIps = blockedIps.Split(',').ToList();
    }

    public bool IsBlocked(IPAddress ipAddress) => _blockedIps.Contains(ipAddress.ToString());
}