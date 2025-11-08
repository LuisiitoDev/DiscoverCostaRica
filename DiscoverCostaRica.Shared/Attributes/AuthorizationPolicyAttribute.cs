namespace DiscoverCostaRica.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizationPolicyAttribute : Attribute
{
    public string PolicyName { get; }
    public string RequiredScope { get; }

    public AuthorizationPolicyAttribute(string policyName, string requiredScope)
    {
        PolicyName = policyName;
        RequiredScope = requiredScope;
    }
}
