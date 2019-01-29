using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Twidlle.Infrastructure.CodeAnnotation;
using Twidlle.Infrastructure.Wcf;

namespace Twidlle.Infrastructure.WindowsService
{
    /// <summary> Windows-сервис, хостящий единственный WCF-сервис. </summary>
    public static class WindowsServiceWcfHost<TWcfService>
        where TWcfService: class
    {
        public static void Run([NotNull] string[] args, IWindsorContainer ioc = null)
        {
            using (ioc = ioc ?? new WindsorContainer(new XmlInterpreter()))
            {
                ioc.Register(Component.For<WcfServiceHost<TWcfService>>(),
                    Component.For<TWcfService>());

                WindowsServiceProcess.Run(args, () => ioc.Resolve<WcfServiceHost<TWcfService>>());
            }
        }
    }
}
