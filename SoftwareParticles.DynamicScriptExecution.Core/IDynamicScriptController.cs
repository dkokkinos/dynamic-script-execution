using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.Core.Results;
using System;
using System.Collections.Generic;

namespace SoftwareParticles.DotNetEngine
{
    public interface IDynamicScriptController<T, C>
        where T : IDynamicScriptParameter
        where C : CallArguments
    {
        EvaluationResult Evaluate(T t);
        ExecutionResult Execute(C callingArgs = null, List<ParameterArgument> methodArgs = null);
    }
}
