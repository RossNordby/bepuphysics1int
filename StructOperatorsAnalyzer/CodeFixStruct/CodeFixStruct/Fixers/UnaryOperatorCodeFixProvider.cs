using AsyncUsageAnalyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFixStruct {
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CompoundAssignmentCodeFixProvider)), Shared]
	public class UnaryOperatorCodeFixProvider : CodeFixProvider {
		private const string title = "Replace unary operator with function";

		public sealed override ImmutableArray<string> FixableDiagnosticIds {
			get { return ImmutableArray.Create(UnaryOperatorsAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider() {
			// See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
			//return WellKnownFixAllProviders.BatchFixer; // Breaks parenthesis
			return CustomBatchFixAllProvider.Instance;
		}

		public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var nodeToFix = root.FindNode(context.Span, getInnermostNodeForTie: true);
			if (nodeToFix == null)
				return;

			var title = "Use function";
			var codeAction = CodeAction.Create(
				title,
				ct => Refactor(context.Document, nodeToFix, ct),
				equivalenceKey: title);

			context.RegisterCodeFix(codeAction, context.Diagnostics);
		}

		private static async Task<Document> Refactor(Document document, SyntaxNode nodeToFix, CancellationToken cancellationToken) {
			var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);

			var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
			var generator = editor.Generator;

			var operationA = semanticModel.GetOperation(nodeToFix, cancellationToken);
			if (operationA.Kind == OperationKind.UnaryOperator) {
				var unOp = (IUnaryOperation)operationA;
				UnaryOperatorKind operatorKind = unOp.OperatorKind;
				SyntaxNode rightSyntaxNode = unOp.Operand.Syntax;

				if (!GetFunctionForOperator(operatorKind, out string op))
					return document;

				var newExpression = generator.InvocationExpression(
					generator.MemberAccessExpression(rightSyntaxNode, op));

				editor.ReplaceNode(nodeToFix, newExpression.WithAdditionalAnnotations(Formatter.Annotation));
				return editor.GetChangedDocument();
			}
			else return document;
		}


		static bool GetFunctionForOperator(UnaryOperatorKind binOp, out string functionName) {
			if (binOp == UnaryOperatorKind.Minus) { functionName = "Neg"; return true; }
			functionName = null;
			return false;
		}
	}
}
