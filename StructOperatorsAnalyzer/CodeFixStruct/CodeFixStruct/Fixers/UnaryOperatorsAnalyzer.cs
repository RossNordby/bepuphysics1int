using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace CodeFixStruct {
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UnaryOperatorsAnalyzer : DiagnosticAnalyzer {
        public const string DiagnosticId = "UnaryOperator";
        public const string Title = "Unary operator not supported.";
        public const string MessageFormat = "Replace unary operator with method.";
        public const string Category = "Errors";
        public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, Severity, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context) {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterOperationAction(AnalyzeInvocationUnary, OperationKind.UnaryOperator); // -x
        }

        const string StructStartName = "Fix";

        private void AnalyzeInvocationUnary(OperationAnalysisContext context) {
            var operation = (IUnaryOperation)context.Operation;
            if (operation.OperatorKind == UnaryOperatorKind.Minus) {
                if (operation.Type.Name.StartsWith(StructStartName)) {
                    context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation(), $"{operation.OperatorKind} operator"));
                }
            }
        }
    }
}
