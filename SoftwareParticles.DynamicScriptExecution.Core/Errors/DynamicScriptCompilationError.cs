using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareParticles.DynamicScriptExecution.Core.Errors
{
    public class DynamicScriptCompilationError
    {
        public string ErrorMessage { get; }
        public int FromLine { get; }
        public int ToLine { get; }
        public int FromCharacter { get; }
        public int Length { get; }

        public DynamicScriptCompilationError(string errorMessage, int fromLine, int toLine, int fromCharacter, int length)
        {
            ErrorMessage = errorMessage;
            FromLine = fromLine;
            ToLine = toLine;
            FromCharacter = fromCharacter;
            Length = length;
        }
    }
}
