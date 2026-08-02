using System;
using System.Collections.Generic;
using System.IO;

using MksArchiver.Core;
using MksArchiver.Cli;

public static partial class ArchiverLogic
{
    public static void CreateArchive(string fileName, string[]? options)
    {
        if (options is null || options.Length == 0) throw new ArchiverException(fileName);
        
        var filesInformation = new List<FileInfo>();

        foreach (var option in options)
        {
            FileInfo fileInfo = new FileInfo(option);
            if (!fileInfo.Exists) throw new ArchiverException(fileInfo.Name);
            filesInformation.Add(fileInfo);
        }
        
        ICompressor compressor = new Compressor();
        
        var compressed = new List<byte[]>();
        var metaDataList = new List<FileMetaData>();
        long offset = 0;

        foreach (var fileInfo in filesInformation)
        {
            byte[] data = File.ReadAllBytes(fileInfo.FullName);
            byte[] compressedData = compressor.Compress(data);
            
            var meta = new FileMetaData(fileInfo.Name, data.Length, compressedData.Length, offset);
            metaDataList.Add(meta);
            compressed.Add(compressedData);
            offset += compressedData.Length;
        }
        
        using FileStream fs = File.Create(fileName);
        using BinaryWriter bw = new BinaryWriter(fs);
        
        ArchiveMetaData.WriteMetaData(bw,  metaDataList.Count);

        foreach (var meta in metaDataList)
        {
            meta.WriteTo(bw);
        }

        foreach (var file in compressed)
        {
            bw.Write(file);
        }
    }
}
