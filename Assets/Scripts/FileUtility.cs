using UnityEngine;
using UnityEditor;
using System.IO;

public static class FileUtility
{
    public static void WriteString(string path, string text)
    {
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(text);
        writer.Close();
    }

    public static string ReadString(string  path)
    {
        string data = "";

        StreamReader reader = new StreamReader(path);
        data = reader.ReadToEnd();
        reader.Close();

        return data;
    }
}