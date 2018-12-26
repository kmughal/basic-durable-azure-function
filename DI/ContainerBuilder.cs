using Microsoft.Extensions.DependencyInjection;

namespace DurableFunctionsSample.DI
{

    public interface ICalculator
    {
        int Add(int input1, int input2);
    }

    public interface IDoubler
    {
        int Double(int value);
    }

    public class Doubler  : IDoubler
    {
        public int Double(int value)
        {
            return value * value;
        }
    }

    public class Calculator : ICalculator
    {
        private readonly IDoubler _doubler;

        public Calculator(IDoubler doubler)
        {
            _doubler = doubler;
        }

        public int Add(int input1, int input2)
        {
            return _doubler.Double(input1) + _doubler.Double(input2);
        }
    }

    public interface IContainerBuilder
    {
        ServiceProvider Build();
    }
    
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection serviceCollection;
        public ContainerBuilder()
        {
            serviceCollection = new ServiceCollection();
            RegisterServices();
        }

        private void RegisterServices()
        {
            serviceCollection.AddScoped(typeof(ICalculator), typeof(Calculator));
            serviceCollection.AddScoped(typeof(IDoubler), typeof(Doubler));
        }


        public ServiceProvider Build()
        {
            return serviceCollection.BuildServiceProvider();
        }
    }
}
