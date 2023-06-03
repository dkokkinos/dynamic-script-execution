using SoftwareParticles.DynamicScriptExecution.Core;
using SoftwareParticles.DynamicScriptExecution.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftwareParticles.DynamicScriptExecution.VisualBasic
{
    public class VisualBasicMethodBodyCodeTemplate : MethodBodyCodeTemplate
    {
        protected override List<string> DefaultImports => new List<string>()
        {
            "System",
            "System.IO",
            "System.Collections.Generic",
            "System.Linq"
        };

        protected override string GetCodeTemplate()
        => @$"{{imports}}

Namespace {GeneratedCodeNamespaceName}
    Public Class [{GeneratedCodeClassName}]
        Public Sub {GeneratedCodeMethodName}({{methodParameters}})
            {{code}}
        End Sub
    End Class
End Namespace";

        public override int GetCodeLineOffset(string code) =>
            code.Substring(0, code.IndexOf($"Public Sub {GeneratedCodeMethodName}"))
                .Count(x => x == '\n');

        protected override string DoBuildMethodParameters(List<ParameterDefinition> parameterDefinitions)
        {
            var methodParams = string.Join(", ", parameterDefinitions.Select(x => $"{(x.Direction == ParameterDirection.Output || x.Direction == ParameterDirection.InputOutput ? "ByRef" : string.Empty)} {x.Key} As {MapToVBType(x.Type)}"));
            return methodParams;
        }

        private string MapToVBType(ParameterDefinitionType type)
        {
            return type switch
            {
                ParameterDefinitionType.Bool => "Boolean",
                ParameterDefinitionType.Int => "Integer",
                ParameterDefinitionType.Datetime => "DateTime",
                ParameterDefinitionType.Datatable => "DataTable",
                ParameterDefinitionType.Double => "Double",
                ParameterDefinitionType.Decimal => "Decimal",
                ParameterDefinitionType.Long => "Long",
                ParameterDefinitionType.String => "String",
                ParameterDefinitionType.Dynamic => "Object",
                ParameterDefinitionType.List => "Object",
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }

        protected override void BuildImports(List<string> imports, ref string code)
        {
            code = code.Replace("{imports}", string.Join(Environment.NewLine, imports.Select(x => $"Imports {x}")));
        }
    }
}
