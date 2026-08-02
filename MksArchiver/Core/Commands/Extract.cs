using System;
using System.Collections.Generic;
using System.IO;

using MksArchiver.Core;
using MksArchiver.Cli;

public static partial class ArchiverLogic
{
    public static void Extract(string fileName, string[]? files)
    {
        bool extractAll = files is null || files.Length == 0;
        ICompressor compressor = new Compressor();
        
        if (extractAll)
        {
            ExtractAll(fileName, shouldEx => true, compressor);
        }
        else
        {
            var hashSet = new HashSet<string>(files!);
            ExtractAll(fileName, entryFile => hashSet.Contains(entryFile), compressor);
        }
    }
    
    private static void ExtractAll(string fileName, Func<string, bool> shouldExtract, ICompressor compressor)
    {
        using FileStream fs = File.OpenRead(fileName);
        using BinaryReader br = new BinaryReader(fs);
        
        (int magicNum, int version, long fileCnt) = ArchiveMetaData.ReadMetaData(br);

        var entries = new List<FileMetaData>();

        for (int i = 0; i < fileCnt; i++)
        {
            entries.Add(FileMetaData.ReadFrom(br));
        }
        
        long endOfEntries = fs.Position;

        foreach (var entry in entries)
        {
            if (!shouldExtract(entry.name)) continue;
            ExtractSingleFile(fs, br, entry, endOfEntries, compressor);
        }
    }

    private static void ExtractSingleFile(FileStream fs, BinaryReader br, FileMetaData entry, long endOfEntries, ICompressor compressor)
    {
        if (entry.compressedSize > int.MaxValue)
            throw new ArchiverException($"Файл {entry.name} слишком большой");
        
        long absOffsetBegin = entry.offset + endOfEntries;
        fs.Seek(absOffsetBegin, SeekOrigin.Begin);
        
        byte[] commpressedData = br.ReadBytes((int)entry.compressedSize);
        byte[] originalData = compressor.Decompress(commpressedData);
            
        string outPath = Path.Combine(Directory.GetCurrentDirectory(), entry.name);
        File.WriteAllBytes(outPath, originalData);        
    }
}
