using Google.Apis.Auth.OAuth2;
using Google.Cloud.SecretManager.V1;
using Grpc.Auth;

namespace DiscoverCostaRica.Infraestructure.Services;
public class SecretManagerService
{
    private readonly SecretManagerServiceClient client;
    private readonly string projectId;

    public SecretManagerService(string projectId, string credentialPath)
    {
        this.projectId = projectId;
        var credential = GoogleCredential.FromFile(credentialPath);
        client = new SecretManagerServiceClientBuilder()
        {
            ChannelCredentials = credential.ToChannelCredentials()
        }.Build();
    }

    public string GetSecret(string secretId)
    {
        var secretName = new SecretVersionName(projectId, secretId, "latest");
        var result = client.AccessSecretVersion(secretName);
        return result.Payload.Data.ToStringUtf8();
    }
}