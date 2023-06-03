using Microsoft.Scripting.Hosting;
using SoftwareParticles.DotNetEngine;
using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.Core.Results;
using System;
using System.Collections.Generic;

namespace SoftwareParticles.DynamicScriptExecution.Python
{
    public class PythonEvaluationParameters : IDynamicScriptParameter
    {
        public string Script { get; }

        public PythonEvaluationParameters(string script)
        {
            Script = script;
        }

    }

    public class PythonEvaluationResult
    { 

    }

    public class PythonDynamicScriptController : IDynamicScriptController<PythonEvaluationParameters, CallArguments>
    {
        ScriptEngine eng;
        ScriptScope scope;

        public EvaluationResult Evaluate(PythonEvaluationParameters t)
        {
            eng = IronPython.Hosting.Python.CreateEngine();
            scope = eng.CreateScope();
            eng.Execute(t.Script, scope);
            return EvaluationResult.Ok();
        }

        public ExecutionResult Execute(CallArguments callArgs, List<ParameterArgument> arguments)
        {
            dynamic greetings = scope.GetVariable(callArgs.MethodName);
            var res = greetings();
            return ExecutionResult.Ok();
        }

    }
}
