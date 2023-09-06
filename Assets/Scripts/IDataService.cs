public interface IDataService {
    bool SaveData<T>(T data, string relativePath, bool encrypted);
    T LoadData<T>(string relativePath, bool encrypted);
}