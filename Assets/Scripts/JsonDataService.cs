using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

public class JsonDataService : IDataService
{
    const string KEY = "Ve1rAgYQYaO6sVTYQK+YYrJBIky/Ns8wcUumGH60+cU=";
    const string IV = "u7CljjuLi3UVDiiBq+BW5Q==";

    public bool SaveData<T>(T data, string relativePath, bool encrypted) {
        string path = Application.persistentDataPath + relativePath;

        try {
            if (File.Exists(path)) {
                Debug.Log("Data exists. Deleting old file and writing a new one!");
                File.Delete(path);
            } else Debug.Log("Writing file for the first time!");

            using FileStream stream = File.Create(path);
            if (encrypted) {
                WriteEncryptedData(data, stream);
            } else {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            return true;
        } catch (Exception ex) {
            Debug.LogError($"Unable to save data due to: {ex.Message} {ex.StackTrace}");
            return false;
        }
    }

    private void WriteEncryptedData<T>(T data, FileStream stream) {
        using Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(KEY);
        aes.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aes.CreateEncryptor();

        using CryptoStream cryptoStream = new(stream, cryptoTransform, CryptoStreamMode.Write);

        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented)));
    }

    public T LoadData<T>(string relativePath, bool encrypted) {
        string path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path)) {
            Debug.LogError($"Cannot load file at {path}. File doesn't exist!");
            throw new FileNotFoundException($"{path} doesn't exist!");
        }

        try {
            T data = default;
            if (encrypted) {
                data = ReadEncryptedData<T>(path);
            } else {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return data;
        } catch (Exception ex) {
            Debug.LogError($"Failed to load data due to: {ex.Message} {ex.StackTrace}");
            throw ex;
        }
    }

    private T ReadEncryptedData<T>(string path) {
        byte[] fileBytes = File.ReadAllBytes(path);

        using Aes aes = Aes.Create();
        aes.Key = Convert.FromBase64String(KEY);
        aes.IV = Convert.FromBase64String(IV);
        
        using ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream decryptionStream = new(fileBytes);

        using CryptoStream cryptoStream = new(decryptionStream, cryptoTransform, CryptoStreamMode.Read);

        using StreamReader reader = new(cryptoStream);

        string result = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<T>(result);
    }
}
