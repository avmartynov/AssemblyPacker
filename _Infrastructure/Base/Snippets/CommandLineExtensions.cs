using System;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class CommandLineExtensions
    {
        public static void OnExecute([NotNull] this CommandLineApplication command, 
            [NotNull] CommandOption waitOption, [NotNull] Action action)
        { 
           command.OnExecute(() =>
               {
                   var result = 0;
                   try
                   {
                       (action ?? throw new ArgumentNullException(nameof(action))).Invoke();
                   }
                   catch (Exception e)
                   {
                       command.Error.WriteLine(e.Message);
                       command.ShowHelp(command.Name);
                       result = 1;
                   }
               
                   if (waitOption.HasValue())
                   {
                       Console.WriteLine("Press <Enter> to continue...");
                       Console.ReadLine();
                   }
                   return result;
               });
        }


        public static void OnExecute([NotNull] this CommandLineApplication command,
            [NotNull] CommandOption waitOption, [NotNull] CommandOption codePageOption,
            [NotNull] Func<string, string> textProc)
        {
            command.OnExecute(() => 
            {
                var result = 0;
                try
                {
                    var cp = Encoding.GetEncoding(int.Parse(codePageOption.Value() ?? "1251"));
                    Console.InputEncoding = cp;
                    Console.OutputEncoding = cp;

                    var stdIn = Console.In.ReadToEnd();

                    var stdOut = (textProc ?? throw new ArgumentNullException(nameof(textProc))).Invoke(stdIn);

                    command.Out.Write(stdOut);
                }
                catch (Exception e)
                {
                    command.Error.WriteLine(e.Message);
                    command.ShowHelp(command.Name);
                    result = 1;
                }

                if (waitOption.HasValue())
                {
                    Console.WriteLine("Press <Enter> to continue...");
                    Console.ReadLine();
                }
                return result;
            });
        }

        [NotNull]
        public static CommandOption Option([NotNull] this CommandLineApplication command, string template, 
            CommandOptionType optionType, bool inherited, string description)
            => command.Option(template, description, optionType, inherited);


        [NotNull]
        public static CommandOption Option([NotNull] this CommandLineApplication command, string template, 
            CommandOptionType optionType, string description)
            => command.Option(template, description, optionType);


        [NotNull]
        public static CommandOption NoValueOption([NotNull] this CommandLineApplication command, string template, string description)
            => command.Option(template, description, CommandOptionType.NoValue);
        

        [NotNull]
        public static CommandOption SingleValueOption([NotNull] this CommandLineApplication command, string template, string description)
            => command.Option(template, description, CommandOptionType.SingleValue);


        [NotNull]
        public static CommandOption NoValueInheritedOption([NotNull] this CommandLineApplication command, string template, string description)
            => command.Option(template, description, CommandOptionType.NoValue, inherited: true);
        

        [NotNull]
        public static CommandOption SingleValueInheritedOption([NotNull] this CommandLineApplication command, string template, string description)
            => command.Option(template, description, CommandOptionType.SingleValue, inherited: true);

        
        [NotNull]
        public static string MandatoryValue([NotNull] this CommandOption option, [NotNull] CommandLineApplication command)
            => option.Value() ?? throw new InvalidOperationException($"Option '--{option.LongName}' is mandatory for command '{command.Name}'.");
    }
}
