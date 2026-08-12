using System;
using System.IO;
using System.Collections.Generic;

using MksArchiver.Cli;
using MksArchiver.Core;

public static partial class ArchiverLogic
{
    public static void ListArchives(string fileName, string[]? arguments = null)
    {
        FileInfo fileInfo = new FileInfo(fileName);
        if (!fileInfo.Exists) throw new ArchiverException($"file '{fileName}' not found");
        
        using FileStream fs = File.OpenRead(fileName);
        using BinaryReader br = new BinaryReader(fs);

        (int magicNum, int version, long fileCnt) = ArchiveMetaData.ReadMetaData(br);

        var FileList = new List<FileMetaData>();
        
        for (int i = 0; i < fileCnt; i++)
        {
            FileMetaData currentFileEntry = FileMetaData.ReadFrom(br);
            FileList.Add(currentFileEntry);
        }

        Console.WriteLine($"Archive file '{fileName}' has {FileList.Count} files");
        Console.WriteLine("Archive files:");
        
        foreach (var fileMetaData in FileList)
        {
            Console.WriteLine($"    Name: {fileMetaData.name}, Size: {fileMetaData.size}, CompressedSize: {fileMetaData.compressedSize}");
        }
    }
}