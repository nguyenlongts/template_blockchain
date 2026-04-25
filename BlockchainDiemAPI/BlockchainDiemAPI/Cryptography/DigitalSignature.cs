using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace BlockchainDiemAPI.Cryptography
{
    public class DigitalSignature
    {
        private RSAParameters _publicKey;
        private RSAParameters _privateKey;

        private static readonly string KeyPath = "data/rsa_key.json";

        public void LoadOrCreateKey()
        {
            if (File.Exists(KeyPath))
            {
                var json = File.ReadAllText(KeyPath);
                var pair = JsonSerializer.Deserialize<RsaKeyPair>(json)!;
                _privateKey = DeserializeKey(pair.PrivateKey, true);
                _publicKey  = DeserializeKey(pair.PublicKey,  false);
            }
            else
            {
                AssignNewKey();
                Directory.CreateDirectory("data");
                File.WriteAllText(KeyPath, JsonSerializer.Serialize(new RsaKeyPair {
                    PrivateKey = SerializeKey(_privateKey),
                    PublicKey  = SerializeKey(_publicKey)
                }));
            }
        }

        public void AssignNewKey()
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.PersistKeyInCsp = false;
            _publicKey  = rsa.ExportParameters(false);
            _privateKey = rsa.ExportParameters(true);
        }

        public byte[] SignData(byte[] hashOfDataToSign)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.PersistKeyInCsp = false;
            rsa.ImportParameters(_privateKey);
            var formatter = new RSAPKCS1SignatureFormatter(rsa);
            formatter.SetHashAlgorithm("SHA256");
            return formatter.CreateSignature(hashOfDataToSign);
        }

        public bool VerifySignature(byte[] hashOfDataToSign, byte[] signature)
        {
            using var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(_publicKey);
            var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
            deformatter.SetHashAlgorithm("SHA256");
            return deformatter.VerifySignature(hashOfDataToSign, signature);
        }
        private static RsaKeyData SerializeKey(RSAParameters p) => new()
        {
            Modulus  = p.Modulus  != null ? Convert.ToBase64String(p.Modulus)  : null,
            Exponent = p.Exponent != null ? Convert.ToBase64String(p.Exponent) : null,
            D        = p.D        != null ? Convert.ToBase64String(p.D)        : null,
            P        = p.P        != null ? Convert.ToBase64String(p.P)        : null,
            Q        = p.Q        != null ? Convert.ToBase64String(p.Q)        : null,
            DP       = p.DP       != null ? Convert.ToBase64String(p.DP)       : null,
            DQ       = p.DQ       != null ? Convert.ToBase64String(p.DQ)       : null,
            InverseQ = p.InverseQ != null ? Convert.ToBase64String(p.InverseQ) : null,
        };

        private static RSAParameters DeserializeKey(RsaKeyData d, bool includePrivate) => new()
        {
            Modulus  = d.Modulus  != null ? Convert.FromBase64String(d.Modulus)  : null,
            Exponent = d.Exponent != null ? Convert.FromBase64String(d.Exponent) : null,
            D        = includePrivate && d.D  != null ? Convert.FromBase64String(d.D)  : null,
            P        = includePrivate && d.P  != null ? Convert.FromBase64String(d.P)  : null,
            Q        = includePrivate && d.Q  != null ? Convert.FromBase64String(d.Q)  : null,
            DP       = includePrivate && d.DP != null ? Convert.FromBase64String(d.DP) : null,
            DQ       = includePrivate && d.DQ != null ? Convert.FromBase64String(d.DQ) : null,
            InverseQ = includePrivate && d.InverseQ != null ? Convert.FromBase64String(d.InverseQ) : null,
        };

        private class RsaKeyPair
        {
            public RsaKeyData PrivateKey { get; set; } = new();
            public RsaKeyData PublicKey  { get; set; } = new();
        }

        private class RsaKeyData
        {
            public string? Modulus   { get; set; }
            public string? Exponent  { get; set; }
            public string? D         { get; set; }
            public string? P         { get; set; }
            public string? Q         { get; set; }
            public string? DP        { get; set; }
            public string? DQ        { get; set; }
            public string? InverseQ  { get; set; }
        }
    }
}