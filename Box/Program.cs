using System;
using System.IO;
using System.Linq;
using System.Threading;
using CommandLine;
using Spectre.Console;

namespace Box
{
    public class Options
    {
        [Verb("install", HelpText = "Install a library.")]
        public class InstallOption
        {
            [Value(0, MetaName = "name", HelpText = "The name of the library to install.", Required = true)]
            public string Name { get; set; }

            [Option('g', "global", Default = false, HelpText = "Whether to install the library globally")]
            public bool Global { get; set; }
        }

        [Verb("uninstall", HelpText = "Uninstall a library.")]
        public class UninstallOption
        {
            [Value(0, MetaName = "name", HelpText = "The name of the library to uninstall.", Required = true)]
            public string Name { get; set; }

            [Option('g', "global", Default = false, HelpText = "Whether to uninstall the library globally")]
            public bool Global { get; set; }
        }

        [Verb("list", HelpText = "List all installed libraries.")]
        public class ListOption
        {
            [Option('g', "global", Default = false, HelpText = "Whether to list the globally installed libraries")]
            public bool Global { get; set; }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string[] safeArgs = Connector.ConnectStrings(string.Join(' ', args));
            Parser.Default.ParseArguments<Options.InstallOption, Options.UninstallOption, Options.ListOption>(safeArgs)
                            .WithParsed<Options.InstallOption>(Install)
                            .WithParsed<Options.UninstallOption>(Uninstall)
                            .WithParsed<Options.ListOption>(List)
                            .WithNotParsed(errs => Environment.Exit(1));
        }

        static void Install(Options.InstallOption options)
        {
            string path = options.Global ? Global.LibDataPath : Environment.CurrentDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(path);
            if (files.Contains(options.Name))
            {
                AnsiConsole.MarkupLine($"[red]Library {options.Name} is already installed.[/]");
                return;
            }
            if (options.Global) AnsiConsole.MarkupLine($"[blue]Installing library {options.Name} globally...[/]");
            else AnsiConsole.MarkupLine($"[blue]Installing library {options.Name} locally...[/]");

            // Install library
            if (Box.IsAvailable(Global.PackageDownloadUrl + options.Name + ".dll"))
                Box.Download(Global.PackageDownloadUrl + options.Name + ".dll", Path.Join(path, options.Name + ".dll"));
            else if (Box.IsAvailable(Global.PackageDownloadUrl + options.Name + ".r"))
                Box.Download(Global.PackageDownloadUrl + options.Name + ".r", Path.Join(path, options.Name + ".r"));
            else
            {
                AnsiConsole.MarkupLine($"[red]Library {options.Name} is not available.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]Installed {options.Name}[/]");
        }

        static void Uninstall(Options.UninstallOption options)
        {
            string path = options.Global ? Global.LibDataPath : Environment.CurrentDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(path);
            if (!files.Contains(options.Name))
            {
                AnsiConsole.MarkupLine($"[red]Library {options.Name} is not installed.[/]");
                return;
            }
            if (options.Global) AnsiConsole.MarkupLine($"[blue]Uninstalling library {options.Name} globally...[/]");
            else AnsiConsole.MarkupLine($"[blue]Uninstalling library {options.Name} locally...[/]");

            // Uninstall library
            File.Delete(Path.Join(path, options.Name + ".dll"));
            File.Delete(Path.Join(path, options.Name + ".r"));

            AnsiConsole.MarkupLine($"[green]Uninstalled {options.Name}[/]");
        }

        static void List(Options.ListOption options)
        {
            string path = options.Global ? Global.LibDataPath : Environment.CurrentDirectory;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string[] files = Directory.GetFiles(path);
            if (options.Global) AnsiConsole.MarkupLine($"[blue]Listing globally installed libraries...[/]");
            else AnsiConsole.MarkupLine($"[blue]Listing locally installed libraries...[/]");

            // List libraries
            if (files.Length == 0)
            {
                AnsiConsole.MarkupLine($"[red]No libraries installed.[/]");
                return;
            }
            AnsiConsole.MarkupLine($"[green]Installed libraries:[/]");
            foreach (string file in files)
            {
                AnsiConsole.MarkupLine($"[green]{Path.GetFileNameWithoutExtension(file)}[/]");
            }
        }
    }
}
