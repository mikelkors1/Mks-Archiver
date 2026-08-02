using System;
using System.IO;
using System.IO.Compression;

using MksArchiver.Cli;
using MksArchiver.Core;

public class Compressor : ICompressor
{
    public byte[] Compress(byte[] data)
    {
        using MemoryStream ms = new MemoryStream();
        using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, true))
        {
            ds.Write(data, 0, data.Length);
        }
        
        return ms.ToArray();
    }

    public byte[] Decompress(byte[] data)
    {
        using MemoryStream iMemStream = new MemoryStream(data);
        using DeflateStream ds = new DeflateStream(iMemStream, CompressionMode.Decompress);
        using MemoryStream oMemStream = new MemoryStream();
        
        ds.CopyTo(oMemStream);
        return oMemStream.ToArray();
    }
}