using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;

namespace MipEchoApp.Db
{
    public class AwsCredentialComponent
    {
        private AwsCredentials _awsCredentials = new AwsCredentials();

        public AwsCredentialComponent(AwsCredentials awsCredentials)
        {
            _awsCredentials = awsCredentials;
        }

        public SessionAWSCredentials GetAwsCredential()
        {
            var sessionCredentials = default(SessionAWSCredentials);

            var region = RegionEndpoint.GetBySystemName(_awsCredentials.RegionEndpoint);

            var cognitoCredentials = new CognitoAWSCredentials(
                _awsCredentials.AccountId,        // account number
                _awsCredentials.IdentityPoolId,   // identity pool id
                _awsCredentials.UnAuthRoleArn,    // role for unauthenticated users
                string.IsNullOrEmpty(_awsCredentials.AuthRoleArn) ? null : _awsCredentials.AuthRoleArn, // role for authenticated users, not set
                region);

            //Get Credential from Cache
            var cacheCredentialState = cognitoCredentials.GetCachedCredentials();
            if (cacheCredentialState != null && cacheCredentialState.Expiration < DateTime.UtcNow)
            {
                var cacheCredential = cacheCredentialState.Credentials;
                sessionCredentials = new SessionAWSCredentials(cacheCredential.AccessKey,
                    cacheCredential.SecretKey,
                    cacheCredential.Token);
            }
            //Get Credential from AWS and Cache it
            else
            {
                var cognitoCredential = cognitoCredentials.GetCredentials();
                sessionCredentials = new SessionAWSCredentials(cognitoCredential.AccessKey,
                                                                    cognitoCredential.SecretKey,
                                                                    cognitoCredential.Token);
                var cacheCredential = new RefreshingAWSCredentials.CredentialsRefreshState()
                {
                    Credentials = cognitoCredential,
                    Expiration = DateTime.UtcNow.AddHours(1)
                };

                cognitoCredentials.CacheCredentials(cacheCredential);
            }

            return sessionCredentials;
        }
    }
}
