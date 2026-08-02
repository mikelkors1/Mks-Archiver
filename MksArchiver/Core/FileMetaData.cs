using System;
using System.IO;

using MksArchiver.Core;
using MksArchiver.Cli;

public class FileMetaData
{
    public string name { get; set; }
    public long size { get; set; }
    public long compressedSize { get; set; }
    public long offset { get; set; }
    
    public long offsetEnd => offset + size;
    
    public FileMetaData(FileInfo file, long offset)
    {
        name = file.Name;
        size = file.Length;
        compressedSize = 0;
        this.offset = offset;
    }

    public FileMetaData(string name, long fileSize, long compressedSize, long offset)
    {
        this.name = name;
        this.size = fileSize;
        this.compressedSize = compressedSize;
        this.offset = offset;
    }
    
    private FileMetaData() {}
    
    public void WriteTo(BinaryWriter bw)
    {
        bw.Write(name);
        bw.Write(size);
        bw.Write(compressedSize);
        bw.Write(offset);
    }

    public static FileMetaData ReadFrom(BinaryReader br)
    {
        return new FileMetaData
        {
            name = br.ReadString(),
            size = br.ReadInt64(),
            compressedSize = br.ReadInt64(),
            offset = br.ReadInt64(),
        };
    }
}
