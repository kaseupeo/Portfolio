using System.IO;
using System.IO.Compression;
using UnityEngine;

public class Utils
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();

        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
    
    // 압축
    public static byte[] Compress(byte[] data)
    {
        using MemoryStream compressedStream = new MemoryStream();
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            gzipStream.Write(data, 0, data.Length);
        }
        return compressedStream.ToArray();
    }
    
    // 압축 풀기
    public static byte[] Decompress(byte[] data)
    {
        using MemoryStream compressedStream = new MemoryStream(data);
        using MemoryStream decompressedStream = new MemoryStream();
        using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        {
            gzipStream.CopyTo(decompressedStream);
        }
        return decompressedStream.ToArray();
    }
}