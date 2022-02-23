using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Domain.Foundation.SourceGenerator
{
    [Generator]
    public class ApplyEventsGenerator: ISourceGenerator
    {
        private string _autoApplyInterfaceAttribute = "Domain.Foundation.AutoApplyInterfaceAttribute";
        private string _event = "Domain.Foundation.Events.IEvent";
        
        public void Initialize(GeneratorInitializationContext context)
        { }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var autoAttributeSymbol = compilation.GetTypeByMetadataName(_autoApplyInterfaceAttribute) ?? throw null;
            var iEventSymbol = compilation.GetTypeByMetadataName(_event) ?? throw null;

            var attributedInterfaces = compilation.SemanticTrees()
                .SelectMany(x => x.SyntaxTree
                    .GetRoot()
                    .DescendantNodesAndSelf()
                    .OfType<InterfaceDeclarationSyntax>()
                    .Select(y => x.SemanticModel.GetDeclaredSymbol(y))
                    .Where(y => y.ImplementInterface(iEventSymbol))
                    .Where(y => y.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(autoAttributeSymbol, z.AttributeClass)))
                ).ToArray();
            
            if (!attributedInterfaces.Any())
                return;

            var subTypes = compilation.SemanticTrees()
                .SelectMany(x => x.SyntaxTree
                    .GetRoot()
                    .DescendantNodesAndSelf()
                    .OfType<ClassDeclarationSyntax>()
                    .Select(y => x.SemanticModel.GetDeclaredSymbol(y))
                    .Where(y => attributedInterfaces.Any(y.ImplementInterface))
                ).ToArray();
            
            foreach (var baseEvent in attributedInterfaces)
            {
                var generatedInterface = baseEvent.GetIApplyInterfaceName();
                
                var subEvents = subTypes
                    .Where(x => x.ImplementInterface(baseEvent))
                    .ToArray();

                var switchMethod = GetSwitch(baseEvent.Name, subEvents);

                var generatedSource = GetIApplyEventInterface(
                    baseEvent.ContainingNamespace.ToString(),
                    generatedInterface,
                    subEvents.GetIApplyInterfacesDeclaration(),
                    switchMethod
                );
                
                context.AddSource($"{generatedInterface}.cs", generatedSource);
            }
        }
        
        string GetIApplyEventInterface(string @namespace, string @class, string inherits, string @switch) => 
            @$"using Domain.Foundation.Tactical;
namespace {@namespace} 
{{
    internal interface {@class}: 
        {inherits}
    {{
        {@switch}
    }}
}}
";

        string GetSwitch(string baseEvent, params INamedTypeSymbol[] subEvents) => 
            $@"
        void ApplyEvent({baseEvent} evt)
        {{
            switch (evt)
            {{  {GetSwitchCases(subEvents)}
                default:
                    throw new System.InvalidOperationException(nameof(evt));
            }}
        }}
";
        string GetSwitchCases(params INamedTypeSymbol[] subEvents)
        {
            var cases = subEvents.Select(x => 
                @$"
                case {x.OriginalDefinition} x: 
                    Apply(x);
                break;");

            return string.Join(Environment.NewLine, cases);
        }
    }
}