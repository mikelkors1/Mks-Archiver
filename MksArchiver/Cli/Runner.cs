using System;
using System.CommandLine;
using System.Collections.Generic;
using System.IO.Enumeration;
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
    
    private static List<Option<bool>> opts = new List<Option<bool>>()
    {
        createOption,
        listOption,
        extractOption,
        appendOption,
        deleteOption,
        infoOption
    };

    private delegate int ArchiverAction(string fileName, string[]? positionalArguments);
    
    private static Dictionary<Option<bool>, ArchiverAction> actionsTable = new Dictionary<Option<bool>, ArchiverAction>()
    {
        {createOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.CreateArchive(fileName, positionalArgumetns);
            return (int)OperationResult.CreateSuccess;
        }},
        {listOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.ListArchives(fileName, positionalArgumetns);
            return (int)OperationResult.ListSuccess;
        }},
        {extractOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.Extract(fileName, positionalArgumetns);
            return (int)OperationResult.ExtractSuccess;
        }},
        {appendOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.AppendFiles(fileName, positionalArgumetns);
            return (int)OperationResult.AppendSuccess;
        }},
        {deleteOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.DeleteArchives(fileName, positionalArgumetns);
            return (int)OperationResult.DeleteSuccess;
        }},
        {infoOption, (fileName, positionalArgumetns) =>
        {
            ArchiverLogic.ReadMetaToTakeInfo(fileName, positionalArgumetns);
            return (int)OperationResult.InfoSuccess;
        }},
    };
    
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
                foreach (var option in opts)
                {
                    if (parsedResult.GetValue(option) is true)
                    {
                        return actionsTable[option](filePath, positionalArgument);
                    }
                }
                
                throw new BadArgumentException("No arguments provided");
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
