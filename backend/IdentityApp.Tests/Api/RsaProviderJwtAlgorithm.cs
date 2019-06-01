using System.Security.Cryptography;
using JWT.Algorithms;
using Microsoft.IdentityModel.Tokens;

namespace IdentityApp
{
    public class RsaProviderJwtAlgorithm : IJwtAlgorithm
    {
        private RSAParameters rsaParameters;

        public RsaProviderJwtAlgorithm(RSAParameters rsaParameters)
        {
            this.rsaParameters = rsaParameters;
        }

        public string Name => "RS256";

        public bool IsAsymmetric => true;

        public byte[] Sign(byte[] key, byte[] bytesToSign)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(rsaParameters);
                return RSA.Encrypt(bytesToSign, true);
            }
        }
    }
}