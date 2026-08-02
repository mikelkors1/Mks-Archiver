using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using MksArchiver.Cli;
using MksArchiver.Core;

public static partial class ArchiverLogic
{
    public static void AppendFiles(string archiveName, string[]? options)
    {
        if (options == null || options.Length == 0)
            throw new ArchiverException("No options provided");
        
        string tmpArchiveName = archiveName + ".tmp";
        
        using FileStream fs = File.OpenRead(archiveName);
        using (BinaryReader br = new BinaryReader(fs))
        {
            (int magicNumber, int version, long fileCnt) = ArchiveMetaData.ReadMetaData(br);

            var entries = new List<FileMetaData>();
            for (int i = 0; i < fileCnt; i++)
            {
                var entry = FileMetaData.ReadFrom(br);
                entries.Add(entry);
            }

            long oldMetaDataEnd = fs.Position;

            var existingNames = new HashSet<string>(entries.Select(e => e.name));
            foreach (var file in options)
            {
                string shortName = new FileInfo(file).Name;
                if (existingNames.Contains(shortName))
                    throw new ArchiverException("File already exists");
            }

            ICompressor compressor = new Compressor();
            var newEntries = new List<FileMetaData>();
            var newFilesCompressedData = new List<byte[]>();

            long newOffset = entries.Sum(e => e.compressedSize);

            foreach (var file in options)
            {
                var fileInfo = new FileInfo(file);
                if (!fileInfo.Exists) throw new ArchiverException("File not found");
                
                byte[] rowData = File.ReadAllBytes(fileInfo.FullName);
                byte[] compressedData = compressor.Compress(rowData);
                
                var meta = new FileMetaData(fileInfo.Name, rowData.Length, compressedData.Length, newOffset);
                newEntries.Add(meta);
                newFilesCompressedData.Add(compressedData);
                
                newOffset += compressedData.Length;
            }
            
            using FileStream newFileStream = File.Create(tmpArchiveName);
            using BinaryWriter bw = new BinaryWriter(newFileStream);
            
            ArchiveMetaData.WriteMetaData(bw, entries.Count + newEntries.Count);

            foreach (var entry in entries)
            {
                entry.WriteTo(bw);
            }

            foreach (var entry in newEntries)
            {
                entry.WriteTo(bw);
            }

            foreach (var oldEntry in entries)
            {
                fs.Seek(oldEntry.offset + oldMetaDataEnd, SeekOrigin.Begin);
                byte[] oldData = br.ReadBytes((int)oldEntry.compressedSize);
                bw.Write(oldData, 0, oldData.Length);
            }
            
            foreach (var newData in newFilesCompressedData)
            {
                bw.Write(newData, 0, newData.Length);
            }
        }
        
        File.Delete(archiveName);
        File.Move(tmpArchiveName, archiveName);
    }
}