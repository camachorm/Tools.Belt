using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tools.Belt.Common.Abstractions.Configuration;

namespace Tools.Belt.Common.Services
{
    public interface IService
    {
        Task ExecuteAsync(ILogger logger, IConfigurationService config);
    }

    public interface IService<TOutput, in T1>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1);
    }

    public interface IService<TOutput, in T1, in T2>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2);
    }

    public interface IService<TOutput, in T1, in T2, in T3>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2, T3 input3);
    }

    public interface IService<TOutput, in T1, in T2, in T3, in T4>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2, T3 input3,
            T4 input4);
    }

    public interface IService<TOutput, in T1, in T2, in T3, in T4, in T5>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2, T3 input3,
            T4 input4, T5 input5);
    }

    public interface IService<TOutput, in T1, in T2, in T3, in T4, in T5, in T6>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2, T3 input3,
            T4 input4, T5 input5, T6 input6);
    }

    public interface IService<TOutput, in T1, in T2, in T3, in T4, in T5, in T6, in T7>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config, T1 input1, T2 input2, T3 input3,
            T4 input4, T5 input5, T6 input6, T7 input7);
    }

    public interface IService<TOutput>
    {
        Task<TOutput> ExecuteAsync(ILogger logger, IConfigurationService config);
    }
}