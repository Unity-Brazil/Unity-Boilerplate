public enum SaveResult
{
    Success,
    Error,
    EmptyData
}

namespace GleyAllPlatformsSave
{
    using UnityEngine.Events;
    public interface ISaveClass
    {
        void Save<T>(T dataToSave, string path, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new();
        void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new();
        void Load<T>(string path, UnityAction<T, SaveResult, string> LoadCompleteMethod, bool encrypted) where T : class, new();
        void LoadString<T>(string path, UnityAction<T, SaveResult, string> LoadCompleteMethod, bool encrypted) where T : class, new();
        void ClearAllData(string path);
        void ClearFile(string path);
    }
}
