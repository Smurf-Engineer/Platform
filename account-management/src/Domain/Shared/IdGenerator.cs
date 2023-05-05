using System.Net;
using System.Net.Sockets;

namespace PlatformPlatform.AccountManagement.Domain.Shared;

/// <summary>
///     IdGenerator is a utility that can generate IDs in a low-latency, distributed, uncoordinated, roughly
///     time-ordered manner, in a highly available system. By maintaining the order of IDs, there is no need to sort
///     rows when fetching from the database (which is not the case when using GUIDs). Since IDs are not generated by
///     the database, they can be used immediately (e.g., in log messages). Moreover, longs are more readable, faster,
///     and require less storage than GUIDs.
///     This implementation uses the IdGen NuGet package, which is inspired by the Twitter Snowflake algorithm to
///     ensure uniqueness across distributed systems, such as web farms.
///     This implementation uses the default configuration of GenId, where 2015-01-01 is used as the epoch (start date
///     of ID 1), and it will be able to generate IDs for 69 years (until 2084). In this default setup, each machine in
///     the distributed system is assigned a unique generator ID (based on its IPv4 address), and it allows up to 1024
///     nodes. With this setting, it allows generating up to 4096 unique IDs per ms per node.
/// </summary>
public static class IdGenerator
{
    private static readonly IdGen.IdGenerator Generator = new(GetUniqueGeneratorIdFromIpAddress());

    /// <summary>
    ///     Generates a new unique ID based on the Twitter Snowflake algorithm.
    /// </summary>
    public static long NewId()
    {
        return Generator.CreateId();
    }

    /// <summary>
    ///     Retrieves a unique generator ID based on the machine's IPv4 address.
    /// </summary>
    private static int GetUniqueGeneratorIdFromIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork) ??
                        throw new InvalidOperationException(
                            "No network adapters with an IPv4 address in the system. IdGenerator is meant to create unique IDs across multiple machines, and requires an IP address to do so.");

        var lastSegment = ipAddress.ToString().Split('.').Last();
        return int.Parse(lastSegment);
    }
}