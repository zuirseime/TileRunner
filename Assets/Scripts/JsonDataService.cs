using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class JsonDataService : IDataService
{
    public bool SaveData<T>(T data, string relativePath, bool encrypted) {
        string path = Application.persistentDataPath + relativePath;

        try {
            if (File.Exists(path)) {
                Debug.Log("Data exists. Deleting old file and writing a new one!");
                File.Delete(path);
            } else Debug.Log("Writing file for the first time!");

            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
            return true;
        } catch (Exception ex) {
            Debug.LogError($"Unable to save data due to: {ex.Message} {ex.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string relativePath, bool encrypted) {
        string path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path)) {
            Debug.LogError($"Cannot load file at {path}. File doesn't exist!");
            throw new FileNotFoundException($"{path} doesn't exist!");
        }

        try {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            return data;
        } catch (Exception ex) {
            Debug.LogError($"Failed to load date due to: {ex.Message} {ex.StackTrace}");
            throw ex;
        }
    }
}
