using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace VideoTeca.Models
{
    public class Util
    {
        public static string BaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl.EndsWith("/"))
            {
                appUrl = appUrl.Substring(0, appUrl.Length - 1);
            }

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }

        public static string BaseUrl(string v)
        {
            if (v.StartsWith("/"))
            {
                return BaseUrl() + v;
            }
            else
            {
                return BaseUrl() + "/" + v;
            }
        }

        public static string hash(String input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(input);
            byte[] hash = sha.ComputeHash(data);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static usuarioDTO Decrypt(string ciphertext)
        {
            using (Aes aesAlg = Aes.Create())
            {
                byte[] ciphertextByte = Convert.FromBase64String(ciphertext);
                byte[] iv = new byte[16];
                byte[] key = new byte[32]; // 256-bit key
                // Key removed for legal reasons. Authentication only works in UNITINS with correct key
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] decryptedBytes;

                using (var msDecrypt = new System.IO.MemoryStream(ciphertextByte))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new System.IO.MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                
                return JsonConvert.DeserializeObject<usuarioDTO>(Encoding.UTF8.GetString(decryptedBytes));
            }
        }
    }
}
