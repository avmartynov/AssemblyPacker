using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.CommandLineUtils;
using Twidlle.AssemblyPacker.Core;
using Twidlle.AssemblyPacker.Interface;
using Twidlle.Infrastructure;

namespace Twidlle.AssemblyPacker.CmdLineApp
{
    public sealed class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                _logger.Info("Start application.");

                var packer = (IPacker) new Packer();

                var rootCmd = new CommandLineApplication(throwOnUnexpectedArg: true)
                                  {
                                      Name        = ApplicationInfo.FileName,
                                      Description = ApplicationInfo.ProductName,
                                      FullName    = ApplicationInfo.FileName,
                                  };
                                   rootCmd.NoValueInheritedOption(    "-? | -h | --help",       "Show command line options");
                var outputDirOpt = rootCmd.SingleValueInheritedOption("-o | --outputDirectory", "Output directory");
                var waitOpt      = rootCmd.NoValueInheritedOption(    "-w | --wait",            "Wait for input before closing console window.");
                rootCmd.OnExecute(waitOpt, () => rootCmd.ShowHelp(rootCmd.Name));

                { 
                    var cmd = rootCmd.Command("pack", c => c.Description = "Pack assembly package.");
                    var inputDirOpt  = cmd.SingleValueOption("-i | --inputDirectory", "Input directory");
                    var asmNameOpt   = cmd.SingleValueOption("-a | --assemblyName",   "Packed assembly name");
                    var scriptingOpt = cmd.NoValueOption(    "-s | --scripting",      "Generate build script");
                    cmd.OnExecute(waitOpt, () => packer.Pack( inputDirOpt.Value() ?? Directory.GetCurrentDirectory(),
                                                             outputDirOpt.MandatoryValue(cmd),
                                                               asmNameOpt.Value() ?? "PackedAssembly",
                                                             scriptingOpt.HasValue()));
                }
                { 
                    var cmd = rootCmd.Command("unpack", c => c.Description = "Unpack assembly package.");
                    var packedAsmPathOpt = cmd.SingleValueOption("-p | --packedAssembly",    "Packed assembly file path");
                    var assemblyDirOpt   = cmd.NoValueOption(    "-d | --directoryAssembly", "Create directory for assembly");
                    cmd.OnExecute(waitOpt, () => packer.Unpack(packedAsmPathOpt.MandatoryValue(cmd),
                                                                   outputDirOpt.MandatoryValue(cmd),
                                                                 assemblyDirOpt.HasValue()));
                }
                return rootCmd.Execute(args);
            }
            catch (Exception x)
            {
                _logger.Error(x);
                Console.Error.WriteLine(x.Message);
                return 1;
            }
            finally
            {
                _logger.Info($"Finish application.{Environment.NewLine}");
            }
        }

        private static readonly TraceSource _logger =  MethodBase.GetCurrentMethod().GetTraceSource();
    }
}
