using System;
using System.IO;

public static class ArchiveMetaData
{
    private const int MagicNum = 0x4D4B5300;
    private const int Version = 1;
    
    public static void WriteMetaData(BinaryWriter bw, long fileCnt)
    {
        bw.Write(MagicNum);
        bw.Write(Version);
        bw.Write(fileCnt);
    }

    public static (int magicNum, int version, long fileCnt) ReadMetaData(BinaryReader br)
    {
        int magicN = br.ReadInt32();
        if (magicN != MagicNum) 
            throw new InvalidDataException("Invalid magic number");
        int version =  br.ReadInt32();
        long fileCnt = br.ReadInt64();
        return (magicN, version, fileCnt);
    }
}