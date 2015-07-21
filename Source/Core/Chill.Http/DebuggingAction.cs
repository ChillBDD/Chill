namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DebuggingAction : IUserAction
    {
        private readonly Func<Task> _action;

        public DebuggingAction(Func<Task> action)
        {
            _action = action;
        }

        public string Message { get; set; }

        public IEnumerable<ResponseAction> ResultActions
        {
            get { return Enumerable.Empty<ResponseAction>(); }
        }

        public Task Execute()
        {
            return _action();
        }
    }
}