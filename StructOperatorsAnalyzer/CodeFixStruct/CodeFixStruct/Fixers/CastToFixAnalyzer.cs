using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace CodeFixStruct {
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CastToFixAnalyzer : DiagnosticAnalyzer {
		public const string DiagnosticId = "Cast to Fix()";
		public const string Title = "Implicit cast not supported";
		public const string MessageFormat = "Replace implicit cast with function";
		public const string Category = "Warnings"; // Explicit cast to make constants is intended, but when transitioning from structs isn't
		public const DiagnosticSeverity Severity = DiagnosticSeverity.Error;

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, Severity, true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

		public override void Initialize(AnalysisContext context) {
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.RegisterOperationAction(AnalyzeNode, OperationKind.Conversion);
		}

		const string StructStartName = "Fix";

		private void AnalyzeNode(OperationAnalysisContext context) {
			var operation = (IConversionOperation) context.Operation;
			if (operation.Type.Name.StartsWith(StructStartName)) {
				context.ReportDiagnostic(Diagnostic.Create(Rule, operation.Syntax.GetLocation()));
			}
			Debug.WriteLine(context.ToString());
		}
	}
}
