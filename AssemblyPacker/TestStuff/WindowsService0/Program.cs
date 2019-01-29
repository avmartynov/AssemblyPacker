using Twidlle.Infrastructure.WindowsService;

namespace WindowsService0
{
    public class Program
    {
        public static void Main(string[] args)
            => WindowsServiceProcess.Run(args, () => new StartupObject());
    }
}
