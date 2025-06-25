using System.Collections.Concurrent;

namespace TodoApi.Services;

public class RoleClaimService
{
    private readonly ConcurrentDictionary<string, string[]> roleDict = new();


    public void AddRoleClaims(string roleName, string[] claims)
    {
        if (roleDict.ContainsKey(roleName))
        {
            roleDict[roleName] = claims;
        }
        else
        {
            roleDict.TryAdd(roleName, claims);
        }
    }

    public string[]? GetRoleClaims(string roleName)
    {
        if (roleDict.TryGetValue(roleName, out var claims))
        {
            return claims;
        }
        return null;
    }

    public bool RoleExists(string roleName)
    {
        return roleDict.ContainsKey(roleName);
    }


    public void ClearRoleClaims()
    {
        roleDict.Clear();
    }

    public void RemoveRoleClaims(string roleName)
    {
        roleDict.TryRemove(roleName, out _);
    }

}