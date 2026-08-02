using System;
using System.IO;
using System.IO.Compression;

using MksArchiver.Cli;
using MksArchiver.Core;

public interface ICompressor
{
    byte[] Compress(byte[] data);
    byte[] Decompress(byte[] data);
}