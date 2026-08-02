using System;

using MksArchiver.Cli;

public sealed class BadArgumentException : Exception
{
    private string messege { get; set; }

    public BadArgumentException()
    {
        messege = "error with arguments";
    }

    public BadArgumentException(string message)
    {
        this.messege = message;
    }
    
    public BadArgumentException(string message, Exception inner)
    {
        this.messege = message;
    }
}