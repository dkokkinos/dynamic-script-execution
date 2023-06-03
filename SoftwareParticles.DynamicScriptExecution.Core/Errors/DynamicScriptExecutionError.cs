using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.Core.Errors
{
    public class DynamicScriptExecutionError
    {
        public string Message { get; }

        public DynamicScriptExecutionError(string message)
        {
            Message = message;
        }
    }
}
