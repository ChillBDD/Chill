namespace Chill.Http
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserAction
    {
        string Message { get; }
        IEnumerable<ResponseAction> ResultActions { get; }

        Task Execute();
    }

    public interface IUserAction<TResult> : IUserAction
    {
        
    }
}