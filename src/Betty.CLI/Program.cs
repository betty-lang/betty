using Betty.Core;
using Betty.Core.Interpreter;

namespace Betty.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                // Script file execution mode
                ExecuteScriptFile(args[0]);
            }
            else
            {
                // Interactive program editor mode
                StartProgramEditor();
            }
        }

        static void ExecuteScriptFile(string path)
        {
            try
            {
                string source = File.ReadAllText(path);
                var lexer = new Lexer(source);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);
                interpreter.Interpret();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
            finally
            {
                Console.Write("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        static void StartProgramEditor()
        {
            var programLines = new List<string>();
            bool editing = true;

            Console.WriteLine("Betty");
            Console.WriteLine("Type \":help\" for more information.");

            while (editing)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                // Handle special commands
                switch (input.Trim().ToLower())
                {
                    case ":help":
                        ShowHelp();
                        continue;
                    case ":list":
                        ListProgram(programLines);
                        continue;
                    case ":run":
                        RunProgram(programLines);
                        continue;
                    case ":clear":
                        programLines.Clear();
                        Console.WriteLine("Program cleared.");
                        continue;
                    case ":exit":
                        editing = false;
                        continue;
                }

                // Add line to program
                programLines.Add(input);
            }

            Console.WriteLine("Exiting Betty Program Editor.");
        }

        static void ShowHelp()
        {
            Console.WriteLine("Program Editor Commands:");
            Console.WriteLine("  :help  - Show this help");
            Console.WriteLine("  :list  - List current program");
            Console.WriteLine("  :run   - Execute the current program");
            Console.WriteLine("  :clear - Clear the current program");
            Console.WriteLine("  :exit  - Exit the program editor");
            Console.WriteLine("Enter your program line by line. Use commands to manage the program.");
        }

        static void ListProgram(List<string> programLines)
        {
            if (programLines.Count == 0)
            {
                Console.WriteLine("Program is empty.");
                return;
            }

            Console.WriteLine("Current Program:");
            for (int i = 0; i < programLines.Count; i++)
            {
                Console.WriteLine($"{i + 1}: {programLines[i]}");
            }
        }

        static void RunProgram(List<string> programLines)
        {
            if (programLines.Count == 0)
            {
                Console.WriteLine("No program to run.");
                return;
            }

            try
            {
                // Combine program lines into a single source string
                string source = string.Join(Environment.NewLine, programLines);

                // Create lexer, parser, and interpreter
                var lexer = new Lexer(source);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);

                // Run the program
                Console.WriteLine("--- Program Output ---");
                interpreter.Interpret();
                Console.WriteLine("--- Program Finished ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running program: {ex.Message}");
            }
        }
    }
}