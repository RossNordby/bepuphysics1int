using AsyncUsageAnalyzers.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace CodeFixStruct {
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CastToFixFixProvider)), Shared]
	public class CastToFixFixProvider : CodeFixProvider {
		private const string title = "Replace cast with function";

		public sealed override ImmutableArray<string> FixableDiagnosticIds {
			get { return ImmutableArray.Create(CastToFixAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider() {
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

			var operationA = semanticModel.GetTypeInfo(nodeToFix, cancellationToken);
			string castMethod;
			if (operationA.ConvertedType.Name.StartsWith("Fix")) {
				castMethod = "ToFix";
			}
			else {
				return document;
			}

			SyntaxNode toReplace = nodeToFix is CastExpressionSyntax bes ? bes.Expression : nodeToFix;
			SyntaxNode replacement = generator.InvocationExpression(
				generator.MemberAccessExpression(toReplace.WithoutTrivia(), castMethod));

			editor.ReplaceNode(nodeToFix, replacement);
			return editor.GetChangedDocument();
		}
	}
}
