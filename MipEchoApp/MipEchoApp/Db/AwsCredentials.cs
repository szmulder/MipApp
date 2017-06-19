
namespace MipEchoApp.Db
{
    public class ConfigObj
    {
        public AwsCredentials AwsCredentials { get; set; }
    }

    public class AwsCredentials
    {
        public string AccountId { get; set; }

        public string IdentityPoolId { get; set; }

        public string UnAuthRoleArn { get; set; }

        public string AuthRoleArn { get; set; }

        public string RegionEndpoint { get; set; }
    }
}
