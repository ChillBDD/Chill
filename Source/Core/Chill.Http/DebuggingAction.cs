namespace Chill.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DebuggingAction : IUserAction
    {
        private readonly Action _action;

        public DebuggingAction(Action action)
        {
            _action = action;
        }

        public string Message { get; set; }

        public IEnumerable<ResponseAction> ResultActions
        {
            get { return Enumerable.Empty<ResponseAction>(); }
        }

        public void Execute()
        {
            _action();
        }
    }
}