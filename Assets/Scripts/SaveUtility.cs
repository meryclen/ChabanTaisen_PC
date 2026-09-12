using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveUtility
{
    readonly string key_hexStr = "5EEB37D1EC0033280D10B1740E6A26E04C4FBE852C2234C9F9F9B8557EA5A492";
    readonly byte[] key;
    
    public SaveUtility()
    {
        key = new byte[key_hexStr.Length / 2];
        for (int i=0; i<key.Length; i++)
        {
            key[i] = Convert.ToByte(key_hexStr.Substring(i * 2, 2), 16);
        }
    }

    public void Save<T>(string path, T obj)
    {

        string json = JsonUtility.ToJson(obj);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        string iv_strings = null;
        foreach (var item in aes.IV)
        {
            var iv_string = item.ToString();
            iv_strings += iv_string;
        }

        byte[] encrypted;

        using (MemoryStream ms = new())
        {
            ms.Write(aes.IV, 0, 16);
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                byte[] byteJson = Encoding.UTF8.GetBytes(json);
                cs.Write(byteJson, 0, byteJson.Length);
            }
            encrypted = ms.ToArray();
        }

        File.WriteAllBytes(path, encrypted);
    }

    public T Load<T>(string path) where T : new()
    {
        if (!File.Exists(path))
        {
            var newT = new T();
            Save(path, newT);
            return newT;
        }
        
        byte[] data = File.ReadAllBytes(path);

        using Aes aes = Aes.Create();
        aes.Key = key;

        byte[] iv = new byte[16];
        Array.Copy(data, 0, iv, 0, 16);
        aes.IV = iv;

        using MemoryStream ms = new(data, 16, data.Length - 16);
        using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new (cs);

        string json = sr.ReadToEnd();

        return JsonUtility.FromJson<T>(json);
    }
}
