using Microsoft.CodeAnalysis.CodeFixes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Ifp.Analyzers {
    internal class SerialFixAllProvider : FixAllProvider {
        const string fixAllTitle = "Apply fixes in Sequence per document.";

        public static readonly FixAllProvider Instance = new SerialFixAllProvider();
        public override async Task<CodeAction> GetFixAsync(FixAllContext fixAllContext) {
            switch (fixAllContext.Scope) {
                case FixAllScope.Document:
                    return await FixDocument(fixAllContext).ConfigureAwait(false);
                case FixAllScope.Project:
                    return await FixDocuments(fixAllContext, context => context.Project.Documents).ConfigureAwait(false);
                case FixAllScope.Solution:
                    return await FixDocuments(fixAllContext, context => context.Solution.Projects.SelectMany(p => p.Documents)).ConfigureAwait(false);
                case FixAllScope.Custom:
                default:
                    throw new NotSupportedException();
            }
        }

        private Task<CodeAction> FixDocuments(FixAllContext fixAllContext, Func<FixAllContext, IEnumerable<Document>> documentsSelector) {
            var ca = CodeAction.Create(fixAllTitle, async c =>
            {
                var solution = fixAllContext.Solution;
                var oldDocuments = documentsSelector(fixAllContext).ToImmutableArray();
                List<Task<Document>> changedDocuments = new List<Task<Document>>();
                foreach (var document in oldDocuments) {
                    var newDocumentTask = Task.Run(() => GetChangedDocument(document, fixAllContext), fixAllContext.CancellationToken);
                    changedDocuments.Add(newDocumentTask);
                }
                await Task.WhenAll(changedDocuments);
                fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                foreach (var document in changedDocuments.Select(t => t.Result)) {
                    var originalDocument = fixAllContext.Solution.GetDocument(document.Id);
                    if (originalDocument == document)
                        continue;
                    solution = solution.WithDocumentSyntaxRoot(document.Id, await document.GetSyntaxRootAsync());
                    fixAllContext.CancellationToken.ThrowIfCancellationRequested();
                }
                return solution;
            });
            return Task<CodeAction>.FromResult(ca);
        }

        private Task<CodeAction> FixDocument(FixAllContext fixAllContext) {
            return Task<CodeAction>.FromResult(GetDocumentCodeAction(fixAllContext.Document, fixAllContext));
        }

        private static CodeAction GetDocumentCodeAction(Document document, FixAllContext fixAllContext) {
            var ca = CodeAction.Create(fixAllTitle, async c => {
                return await GetChangedDocument(document, fixAllContext);
            });
            return ca;
        }

        private static async Task<Document> GetChangedDocument(Document document, FixAllContext fixAllContext) {
            var cancellationToken = fixAllContext.CancellationToken;
            var diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document).ConfigureAwait(false);
            while (diagnostics.Any()) {
                cancellationToken.ThrowIfCancellationRequested();
                var firstDiag = diagnostics.First();
                CodeAction currentAction = null;
                var context = new CodeFixContext(document, firstDiag, (action, diagnostic) => currentAction = action, cancellationToken);
                await fixAllContext.CodeFixProvider.RegisterCodeFixesAsync(context);
                cancellationToken.ThrowIfCancellationRequested();
                var operations = await currentAction.GetOperationsAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                var solution = operations.OfType<ApplyChangesOperation>().Single().ChangedSolution;
                document = solution.GetDocument(document.Id);
                diagnostics = await fixAllContext.GetDocumentDiagnosticsAsync(document);
            }
            return document;
        }
    }
}
