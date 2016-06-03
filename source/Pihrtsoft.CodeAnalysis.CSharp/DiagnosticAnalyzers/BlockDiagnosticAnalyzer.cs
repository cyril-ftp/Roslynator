﻿// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis.CSharp.Analyzers;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatBlock,
                    DiagnosticDescriptors.FormatEachStatementOnSeparateLine,
                    DiagnosticDescriptors.RemoveRedundantEmptyLine);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeBlock(f), SyntaxKind.Block);
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var block = (BlockSyntax)context.Node;

            RedundantEmptyLineAnalyzer.AnalyzeBlock(context, block);

            FormatEachStatementOnSeparateLineAnalyzer.AnalyzeStatements(context, block.Statements);

            if (block.Statements.Count == 0)
            {
                int startLineIndex = block.OpenBraceToken.GetSpanStartLine();
                int endLineIndex = block.CloseBraceToken.GetSpanEndLine();

                if ((endLineIndex - startLineIndex) != 1
                    && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                    && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatBlock,
                        block.GetLocation());
                }
            }
        }
    }
}
