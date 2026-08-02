using System;
using System.Collections.Generic;
using System.IO;

using MksArchiver.Core;
using MksArchiver.Cli;

public static partial class ArchiverLogic
{
    public static void DeleteArchives(string archiveName, string[]? options)
    {
        if (options == null || options.Length == 0)
        {
            File.Delete(archiveName);
            return;
        }
        
        string tmpArchiveName = archiveName + ".tmp";
        
        using FileStream fs = File.OpenRead(archiveName);
        using (BinaryReader br = new BinaryReader(fs))
        {

            (int magicNumber, int version, long fileCnt) = ArchiveMetaData.ReadMetaData(br);

            var entries = new List<FileMetaData>();
            var fileNamesHashSet = new HashSet<string>();

            for (int i = 0; i < fileCnt; i++)
            {
                var meta = FileMetaData.ReadFrom(br);
                entries.Add(meta);
                fileNamesHashSet.Add(meta.name);
            }

            long endOfEntries = fs.Position;

            foreach (var fileName in options)
            {
                if (!fileNamesHashSet.Contains(fileName))
                    throw new ArchiverException(
                        "Archive does not contain entry " + fileName + " in file " + archiveName);
            }

            var newEntries = new List<FileMetaData>();
            var namesToDelete = new HashSet<string>(options);

            foreach (var entry in entries)
            {
                if (!namesToDelete.Contains(entry.name)) newEntries.Add(entry);
            }

            using FileStream newStream = File.Create(tmpArchiveName);
            using BinaryWriter bw = new BinaryWriter(newStream);

            ArchiveMetaData.WriteMetaData(bw, newEntries.Count);

            long newOffset = 0;
            var recalculatedMetaList = new List<FileMetaData>();

            foreach (var entry in newEntries)
            {
                var recalcFileMeta = new FileMetaData(entry.name, entry.size, entry.compressedSize, newOffset);
                recalculatedMetaList.Add(recalcFileMeta);
                newOffset += entry.compressedSize;
            }

            foreach (var recalcFileMeta in recalculatedMetaList)
            {
                recalcFileMeta.WriteTo(bw);
            }

            foreach (var oldEntry in newEntries)
            {
                if (oldEntry.compressedSize > int.MaxValue)
                    throw new ArchiverException($"Файл {oldEntry.name} слишком большой");
                
                long absOffset = endOfEntries + oldEntry.offset;
                fs.Seek(absOffset, SeekOrigin.Begin);

                byte[] oldData = br.ReadBytes((int)oldEntry.compressedSize);
                bw.Write(oldData, 0, oldData.Length);
            }
        }
        
        File.Delete(archiveName);
        File.Move(tmpArchiveName, archiveName);
    }
}