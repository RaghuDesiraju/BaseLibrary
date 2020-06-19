using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseLibrary.EncryptDecrypt
{
    public class EncryptDecrypt
    {
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "x1$@PXYzAe156YzY";
        private const string passwordPhrase = "i!aCoVi$Ta";
        // This constant is used to determine the keysize of the encryption algorithm
        private const int keysize = 256;
        //Encrypt
        public static string EncryptString(string input)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passwordPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(cryptoStream))
                    {
                        swEncrypt.Write(input);
                    }
                    byte[] cipherTextBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(cipherTextBytes);
                }
            }
                
            //cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            //cryptoStream.FlushFinalBlock();
            //byte[] cipherTextBytes = memoryStream.ToArray();
            //memoryStream.Close();
            //cryptoStream.Close();
            //return Convert.ToBase64String(cipherTextBytes);
        }


        /// <summary>
        /// Decrypt string
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="passPhrase"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptText)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(encryptText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passwordPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Padding = PaddingMode.PKCS7;
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(cryptoStream))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            
            //byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            //int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            //memoryStream.Close();
            //cryptoStream.Close();
            //string decrypt = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).Trim();
            //decrypt = Regex.Replace(decrypt, @"[^\u0000-\u007F]+", string.Empty);
            //return decrypt;
            ////string hex = ConvertStringToHex(decrypt, System.Text.Encoding.Unicode);
            ////return  ConvertHexToString(hex, System.Text.Encoding.Unicode);
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }
    }
}
