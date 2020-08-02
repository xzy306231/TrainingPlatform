using System;
using System.Collections.Generic;
using System.Text;


public class AESHelper
{
    /// <summary>
    /// 默认密钥-密钥的长度必须是32
    /// </summary>
    private const string PublicKey = "1234567890123456";

    /// <summary>
    /// 默认向量
    /// </summary>
    private const string Iv = "abcdefghijklmnop";

    /// <summary>  
    /// AES加密  
    /// </summary>  
    /// <param name="str">需要加密字符串</param>  
    /// <returns>加密后字符串</returns>  
    public static string Encrypt(string str)
    {
        return Encrypt(str, PublicKey);
    }

    /// <summary>  
    /// AES解密  
    /// </summary>  
    /// <param name="str">需要解密字符串</param>  
    /// <returns>解密后字符串</returns>  
    public static string Decrypt(string str)
    {
        return Decrypt(str, PublicKey);
    }

    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="str">需要加密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>加密后的字符串</returns>
    public static string Encrypt(string str, string key)
    {
        var keyArray = Encoding.UTF8.GetBytes(key);
        var toEncryptArray = Encoding.UTF8.GetBytes(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged
        {
            Key = keyArray,
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7,
            IV = Encoding.UTF8.GetBytes(Iv)
        };
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateEncryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }


    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="str">需要解密的字符串</param>
    /// <param name="key">32位密钥</param>
    /// <returns>解密后的字符串</returns>
    public static string Decrypt(string str, string key)
    {
        var keyArray = Encoding.UTF8.GetBytes(key);
        var toEncryptArray = Convert.FromBase64String(str);
        var rijndael = new System.Security.Cryptography.RijndaelManaged
        {
            Key = keyArray,
            Mode = System.Security.Cryptography.CipherMode.ECB,
            Padding = System.Security.Cryptography.PaddingMode.PKCS7,
            IV = Encoding.UTF8.GetBytes(Iv)
        };
        System.Security.Cryptography.ICryptoTransform cTransform = rijndael.CreateDecryptor();
        var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        return Encoding.UTF8.GetString(resultArray);
    }
}

