using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;

namespace Shmuelie.CommandLine.Curses.Runner
{
    public class Program
    {
        public static void Main(string[] args) => new CommandLineBuilder()
        {
            EnablePosixBundling = true
        }.CancelOnProcessTermination().
            ParseResponseFileAs(ResponseFileHandling.ParseArgsAsLineSeparated).
            RegisterWithDotnetSuggest().
            UseAnsiTerminalWhenAvailable().
            UseExceptionHandler().
            UseHelp().
            UseTypoCorrections().
            UseVersionOption().
            AddCommandsInAssembly().
            Build().InvokeAsync(args);
    }
}
