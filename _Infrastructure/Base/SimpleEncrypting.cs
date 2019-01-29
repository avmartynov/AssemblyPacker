using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    // http://tekeye.biz/2015/encrypt-decrypt-c-sharp-string
    public static class SimpleEncrypting
    {
        [Pure]
        public static string Encrypt([NotNull] this string text, [NotNull] string key)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var password        = new Rfc2898DeriveBytes(key, saltSize: 0);
            var keyBytes        = password.GetBytes(keySize / 8);
            var symmetricKey    = new RijndaelManaged {Mode = CipherMode.CBC };
            var transform       = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    memoryStream = null; // avoiding of CA2202

                    var plainTextBytes = Encoding.UTF8.GetBytes(text);
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(cryptoStream.ReadAllBytes());
                }
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }


        [Pure]
        public static string Decrypt([NotNull] this string text, [NotNull] string key)
        {
            var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            var cipherTextBytes = Convert.FromBase64String(text);
            var password        = new Rfc2898DeriveBytes(key, saltSize: 0);
            var keyBytes        = password.GetBytes(keySize / 8);
            var symmetricKey    = new RijndaelManaged { Mode = CipherMode.CBC };
            var transform       = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();

                using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
                {
                    memoryStream = null; // avoiding of CA2202

                    var plainTextBytes = new byte[cipherTextBytes.Length];
                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                }
            }
            finally
            {
                memoryStream?.Dispose();
            }
        }

        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "pemgail9uzpgzl88";

        // This constant is used to determine the keysize of the encryption algorithm
        private const int keySize = 256;
	}
}