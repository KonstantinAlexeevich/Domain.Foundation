using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Domain.Foundation.SourceGenerator
{
    //[Generator]
    public class EventsAggregateImplementationGenerator: ISourceGenerator
    {
        private string _interface = "Domain.Foundation.Tactical.IEventsAggregate`2";
        private string _baseClass = "Domain.Foundation.Tactical.EventsAggregate`2";
        private string _defaultImplementation = "Domain.Foundation.DefaultImplementationAttribute";
        
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var eventsAggregateType = compilation.GetTypeByMetadataName(_baseClass) ?? throw null;
            var iEventsAggregateInterface = compilation.GetTypeByMetadataName(_interface) ?? throw null;
            var defaultImplementation = compilation.GetTypeByMetadataName(_defaultImplementation) ?? throw null;

            var attributedInterfaces = compilation.SemanticTrees()
                .SelectMany(x => x.SyntaxTree
                    .GetRoot()
                    .DescendantNodesAndSelf()
                    .OfType<ClassDeclarationSyntax>()
                    .Select(y => x.SemanticModel.GetDeclaredSymbol(y))
                    .Where(y => y.ImplementInterface(iEventsAggregateInterface))
                    .Where(y => !y.InheritType(eventsAggregateType))
                    .Where(y => y.GetAttributes().Any(z => SymbolEqualityComparer.Default.Equals(defaultImplementation, z.AttributeClass)))
                ).ToArray();
            
            foreach (var type in attributedInterfaces)
            {
                
            }
        }

        string GetAggregatePartial(string @namespace, string @class, string baseEvent)
        {
            return @$"
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace {@namespace}
{{
    public partial class {@class}
    {{
        void Emit({baseEvent} evt) {{
            _changes.Add(evt);
            Apply(evt);
        }}

        public Task Restore(IEnumerable<{baseEvent}> events)
        {{
            foreach (var @event in events) {{
                _existing.Add(@event);
                Apply(@event);
                Version++;
            }}
            
            return Task.CompletedTask;
        }}

        public IReadOnlyCollection<{baseEvent}> Changes => _changes.AsReadOnly();
        protected IReadOnlyCollection<{baseEvent}> Existing => _existing.AsReadOnly();
        public void ClearChanges() => _changes.Clear();
        public int Version {{ get; private set; }} = -1;

        readonly List<{baseEvent}> _existing = new();
        readonly List<{baseEvent}> _changes = new();
        public Task<IEnumerable<{baseEvent}>> Store() => Task.FromResult(Changes.AsEnumerable());
    }}
}}
";
        }

    }
}