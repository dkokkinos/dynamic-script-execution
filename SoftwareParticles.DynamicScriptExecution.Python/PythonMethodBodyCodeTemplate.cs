using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.Python
{
    public class PythonMethodBodyCodeTemplate : MethodBodyCodeTemplate
    {
        protected override List<string> DefaultImports => throw new NotImplementedException();

        public override int GetCodeLineOffset(string code)
        {
            throw new NotImplementedException();
        }

        protected override void BuildImports(List<string> imports, ref string code)
        {
            throw new NotImplementedException();
        }

        protected override string DoBuildMethodParameters(List<ParameterDefinition> parameterDefinitions)
        {
            throw new NotImplementedException();
        }

        protected override string GetCodeTemplate()
        {
            throw new NotImplementedException();
        }
    }
}
