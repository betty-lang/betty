using Betty.Core;
using Betty.Core.Interpreter;

namespace Betty.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path = args.Length > 0 && !string.IsNullOrEmpty(args[0])
                    ? args[0]
                    : throw new ArgumentException("No script file specified.");
                string source = File.ReadAllText(path);
                var lexer = new Lexer(source);
                var parser = new Parser(lexer);
                var interpreter = new Interpreter(parser);
                interpreter.Interpret();
            }
            catch (Exception ex)
            {
                Console.Write($"\nError: {ex.Message}");
            }
            finally
            {
                Console.Write("\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }
    }
}