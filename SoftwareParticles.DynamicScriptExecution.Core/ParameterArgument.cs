using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.Core
{
    public class ParameterArgument
    {
        public string Key { get; }
        public object Value { get; }

        public ParameterArgument(string key, object value = null)
        {
            Key = key;
            Value = value;
        }
    }

    public class CallArguments
    {
        public string MethodName { get; }

        public CallArguments(string methodName = null)
        {
            MethodName = methodName;
        }
    }
}
