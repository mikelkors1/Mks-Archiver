using System;
using System.IO;
using System.Collections.Generic;

using MksArchiver.Core;
using MksArchiver.Cli;

public static partial class ArchiverLogic
{
    public static void ReadMetaToTakeInfo(string fileName, string[]? options)
    {
        using var fs = new FileStream(fileName, FileMode.Open);
        using var br = new BinaryReader(fs);

        (int magicNum, int version, long fileCnt) = ArchiveMetaData.ReadMetaData(br);
        
        char firstByte = (char)(magicNum & 0xFF);
        char secondByte = (char)((magicNum >> 8) & 0xFF);
        char thirdByte = (char)((magicNum >> 16) & 0xFF);
        char fourthByte = (char)((magicNum >> 24) & 0xFF);
        
        // i use back procedure for string because of little endian bytes procedure of archive
        string magicS = fourthByte.ToString() + thirdByte.ToString() + secondByte.ToString() + firstByte.ToString();
        
        Console.WriteLine($"Magic Number: {magicS}, Version: {version}, File Count: {fileCnt}");
    }
}