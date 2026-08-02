using System;

namespace MksArchiver.Core;

public sealed class ArchiverException : Exception
{
    private string messege { get; set; }

    public ArchiverException()
    {
        messege = "error with arguments";
    }

    public ArchiverException(string message)
    {
        this.messege = message;
    }
    
    public ArchiverException(string message, Exception inner)
    {
        this.messege = message;
    }
}