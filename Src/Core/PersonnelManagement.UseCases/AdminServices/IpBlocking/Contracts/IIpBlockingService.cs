using System.Net;

namespace PersonnelManagement.UseCases.AdminServices.IpBlocking.Contracts;

public interface IIpBlockingService
{
    bool IsBlocked(IPAddress ipAddress);
}