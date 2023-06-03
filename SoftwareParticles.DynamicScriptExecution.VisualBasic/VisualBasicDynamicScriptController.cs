using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.VisualBasic
{
    public class VisualBasicDynamicScriptController : DotNetDynamicScriptController
    {
        public VisualBasicDynamicScriptController(CodeTemplate codeTemplate) : base(codeTemplate)
        {
        }

        protected override Compilation GetCompilationForAssembly(string assemblyName)
            => VisualBasicCompilation.Create(assemblyName);

        protected override CompilationOptions GetOptions()
            => new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

        protected override SyntaxTree GetSyntaxTree(string code)
        {
            var options = VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.VisualBasic16_9);
            return VisualBasicSyntaxTree.ParseText(code, options);
        }
    }
}
