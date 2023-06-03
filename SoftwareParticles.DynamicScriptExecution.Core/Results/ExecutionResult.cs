using SoftwareParticles.DynamicScriptExecution.Core.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.Core
{
    public class ExecutionResult
    {
        public List<ExecutionResultEntry> Results { get; }

        public List<DynamicScriptExecutionError> Errors { get; }

        public bool Success => !Errors.Any();

        private ExecutionResult(List<DynamicScriptExecutionError> errors = null)
        {
            Results = new List<ExecutionResultEntry>();
            Errors = errors ?? new List<DynamicScriptExecutionError>();
        }

        public static ExecutionResult WithError(Exception exception)
            => new ExecutionResult(new List<DynamicScriptExecutionError>()
            {
                new DynamicScriptExecutionError(exception.Message)
            });

        public static ExecutionResult Ok()
            => new ExecutionResult();

        public void Add(ExecutionResultEntry entry)
            => Results.Add(entry);

        public T ReturnValueOf<T>() => (T)Results.FirstOrDefault(x => x is MethodReturnValue)?.Value;

        public object ReturnValue => Results.FirstOrDefault(x => x is MethodReturnValue)?.Value;

        public T GetValue<T>(string key)
            => (T)Results.FirstOrDefault(x => x.OutputName == key)?.Value;

        public object this[string key] => GetValue<object>(key);
    }

    public class ExecutionResultEntry
    {
        public string OutputName { get; }
        public object Value { get; }

        public ExecutionResultEntry(string outputName, object value)
        {
            OutputName = outputName;
            Value = value;
        }
    }

    public class MethodReturnValue : ExecutionResultEntry
    {
        public MethodReturnValue(object value) : base("*RETURN_TYPE*", value)
        {
        }
    }
}
