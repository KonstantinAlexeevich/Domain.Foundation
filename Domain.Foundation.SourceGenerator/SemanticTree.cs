using Microsoft.CodeAnalysis;

namespace Domain.Foundation.SourceGenerator
{
    public class SemanticTree
    {
        public SemanticModel SemanticModel { get; set; }
        public SyntaxTree SyntaxTree { get; set; }
    }
}