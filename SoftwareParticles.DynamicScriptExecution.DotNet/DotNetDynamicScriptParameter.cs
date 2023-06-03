using SoftwareParticles.DynamicScriptExecution.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.DotNet
{
    public class DotNetDynamicScriptParameter : IDynamicScriptParameter
    {
        public string Script { get; }
        public string AssemblyName { get; }
        public List<string> Imports { get; }
        public List<string> References { get; }
        public List<ParameterDefinition> Parameters { get; }

        public DotNetDynamicScriptParameter(string script, string assemblyName = null, List<string> imports = null, List<string> references = null, List<ParameterDefinition> parameters = null)
        {
            Script = script;
            AssemblyName = assemblyName ?? "GeneratedAssembly";
            Imports = imports ?? new List<string>();
            References = references ?? new List<string>();
            Parameters = parameters ?? new List<ParameterDefinition>();
        }
    }
}
