namespace Chill.Http
{
    using System.Collections.Generic;

    public interface IUserAction
    {
        string Message { get; }
        IEnumerable<ResponseAction> ResultActions { get; }

        void Execute();
    }

    public interface IUserAction<TResult> : IUserAction
    {
        
    }
}