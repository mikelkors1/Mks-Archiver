using System;
using System.CommandLine;
using System.Collections.Generic;
using MksArchiver.Core;

namespace MksArchiver.Cli;

public static class Runner
{
    private enum OperationResult
    {
        CreateSuccess,
        ListSuccess,
        ExtractSuccess,
        AppendSuccess,
        DeleteSuccess,
        InfoSuccess,
        Failure
    };
    
    private static List<string> Messeges = new List<string>()
    {
        new string("Successful creation of an archive"),
        new string("Successful List checking of an archive"),
        new string("Successful Extract of an archive"),
        new string("Successful Appending of an archive"),
        new string("Successful Delete of an archive"),
        new string("Successful Info checking of an archive"),
        new string("Failure")
    };
    
    private readonly static Option<bool> createOption = new Option<bool>("-c", "--create")
    {
        Description = "Creates a new archive"
    };
    
    private readonly static Option<string> fileOption = new Option<string>("-f", "--file")
    {
        Description = "Specify the file",
        Required = true
    };
    
    private readonly static Option<bool> listOption = new Option<bool>("-l", "--list")
    {
        Description = "List archive files"
    };
    
    private readonly static Option<bool> extractOption = new Option<bool>("-x", "--extract")
    {
        Description = "Extract archive files"
    };
    
    private readonly static Option<bool> appendOption = new Option<bool>("-a", "--append")
    {
        Description = "Append archive files"
    };
    
    private readonly static Option<bool> deleteOption = new Option<bool>("-d", "--delete")
    {
        Description = "Delete archive files"
    };

    private readonly static Option<bool> infoOption = new Option<bool>("-i", "--info")
    {
        Description = "Show information about the archive"
    };
    
    private readonly static Argument<string[]> positionalArguments = new Argument<string[]>("files")
    {
        Description = "Specify a list of positional arguments"
    };
    
    private static RootCommand CreateRootCommand()
    {
        RootCommand rootCommand = new RootCommand("MksArchiver - simple console archiver")
        {
            createOption,
            fileOption,
            listOption,
            extractOption,
            appendOption,
            deleteOption,
            infoOption,
            positionalArguments
        };
        
        return rootCommand;
    }

    private static void SetActionToRootCommand(ref RootCommand rootCommand)
    {
        rootCommand.SetAction(parsedResult =>
        {
            var options = new Option<bool>[] { createOption, listOption, extractOption, appendOption, deleteOption, infoOption };
            
            int counter = 0;
            
            foreach (var option in options)
            {
                if (parsedResult.GetValue(option)) ++counter;
            }
            
            if (counter != 1) throw new BadArgumentException("Too many non-positional arguments");
            
            var filePath = parsedResult.GetValue(fileOption);
            var positionalArgument = parsedResult.GetValue(positionalArguments);

            try
            {
                if (parsedResult.GetValue(createOption) is true)
                {
                    ArchiverLogic.CreateArchive(filePath, positionalArgument);
                    return (int)OperationResult.CreateSuccess;
                }
                else if (parsedResult.GetValue(listOption) is true)
                {
                    ArchiverLogic.ListArchives(filePath);
                    return (int)OperationResult.ListSuccess;
                }
                else if (parsedResult.GetValue(extractOption) is true)
                {
                    ArchiverLogic.Extract(filePath, positionalArgument);
                    return (int)OperationResult.ExtractSuccess;
                }
                else if (parsedResult.GetValue(appendOption) is true)
                {
                    ArchiverLogic.AppendFiles(filePath, positionalArgument);
                    return (int)OperationResult.AppendSuccess;
                }
                else if (parsedResult.GetValue(deleteOption) is true)
                {
                    ArchiverLogic.DeleteArchives(filePath, positionalArgument);
                    return (int)OperationResult.DeleteSuccess;
                }
                else if (parsedResult.GetValue(infoOption) is true)
                {
                    ArchiverLogic.ReadMetaToTakeInfo(filePath, positionalArgument);
                    return (int)OperationResult.InfoSuccess;
                }
                else
                {
                    throw new BadArgumentException("No arguments provided");
                }
            }
            catch (ArchiverException e)
            {
                Console.WriteLine(e.Message);
                return (int)OperationResult.Failure;
            }
        });
    }
    
    public static void Run(string[] args)
    {
        Greetings.PrintGreeting();
        
        if (args.Length == 0) throw new BadArgumentException("No arguments provided");

        try
        {
            var rootCommand = CreateRootCommand();
            SetActionToRootCommand(ref rootCommand);
            var actionResult = rootCommand.Parse(args).Invoke();

            if ((OperationResult)actionResult == OperationResult.Failure)
            {
                Console.WriteLine(Messeges[actionResult]);
            }
            
            Console.WriteLine();
            Console.WriteLine(Messeges[actionResult]);
        }
        catch (BadArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
