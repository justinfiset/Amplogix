using UnityEngine;
using UnityEditor;
using System.IO;

public static class FileUtility
{
    static void WriteString(string path, string text)
    {
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(text);
        writer.Close();
    }

    static string ReadString(string  path,  string text)
    {
        string data = "";

        StreamReader reader = new StreamReader(path);
        data = reader.ReadToEnd();
        reader.Close();

        return data;
    }
}