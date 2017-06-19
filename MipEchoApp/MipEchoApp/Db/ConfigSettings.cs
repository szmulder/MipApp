using System.IO;
using Newtonsoft.Json;

namespace MipEchoApp.Db
{
    public class ConfigSettings
    {
        public static AwsCredentials GetAwsCredentials()
        {
            var configText = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + @"/AwsConfig.json");
            var config = JsonConvert.DeserializeObject<ConfigObj>(configText);

            return config.AwsCredentials;
        }
    }
}
