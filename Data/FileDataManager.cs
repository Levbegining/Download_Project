using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using project_download.Models;

namespace project_download.Data;

public static class FileDataManager
{
    private static string filePath = Path.Combine(Directory.GetCurrentDirectory(), "fileData.json");

    // получить все файлы
    public static List<FileData> GetAllFiles()
    {
        if (!File.Exists(filePath))
        {
            return new List<FileData>();
        }

        var json = File.ReadAllText(filePath);
        if (json != "")
        {
            return JsonSerializer.Deserialize<List<FileData>>(json);
        }
        return new List<FileData>();
    }

    // 
    public static void AddFile(FileData fileData)
    {
        var files = GetAllFiles();
        files.Add(fileData);
        // save to json
        SaveAllFiles(files);
    }
    private static void SaveAllFiles(List<FileData> files)
    {
        var json = JsonSerializer.Serialize(files, new JsonSerializerOptions()
        {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            WriteIndented = true
        });
        File.WriteAllText(filePath, json);
    }
    //search
    public static FileData? GetFile(string fileName)
    {
        return GetAllFiles().FirstOrDefault(x => x.FileName == fileName);
    }
}
