// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;

namespace Microsoft.DocAsCode.MarkdigEngine.Extensions;

public class NestedColumnParser : BlockParser
{
    private const string StartString = "column";
    private const string EndString = "column-end:::";
    private const char Colon = ':';

    private readonly MarkdownContext _context;

    public NestedColumnParser(MarkdownContext context)
    {
        OpeningCharacters = new[] { ':' };
        _context = context;
    }

    public override BlockState TryOpen(BlockProcessor processor)
    {
        if (processor.IsBlankLine)
        {
            return BlockState.Continue;
        }

        var slice = processor.Line;
        var column = processor.Column;
        var sourcePosition = processor.Start;
        var colonCount = 0;

        ExtensionsHelper.SkipSpaces(ref slice);

        var columnWidth = StringBuilderCache.Local();
        var c = slice.CurrentChar;

        while (c == Colon)
        {
            c = slice.NextChar();
            colonCount++;
        }

        if (colonCount < 3) return BlockState.None;

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, StartString, false))
        {
            return BlockState.None;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        if (ExtensionsHelper.MatchStart(ref slice, "span=\"", false))
        {
            c = slice.CurrentChar;

            while (c != '"')
            {
                columnWidth.Append(c);
                c = slice.NextChar();
            }

            if (!ExtensionsHelper.MatchStart(ref slice, "\"", false))
            {
                return BlockState.None;
            }

        }
        else
        {
            columnWidth.Append("1"); // default span is one
        }

        while (c.IsSpace())
        {
            c = slice.NextChar();
        }

        if (!ExtensionsHelper.MatchStart(ref slice, ":::", false)) //change
        {
            return BlockState.None;
        }

        processor.NewBlocks.Push(new NestedColumnBlock(this)
        {
            Line = processor.LineIndex,
            ColumnWidth = columnWidth.ToString(),
            ColonCount = colonCount,
            Column = column,
            Span = new SourceSpan(sourcePosition, slice.End),
        });

        return BlockState.ContinueDiscard;
    }

    public override BlockState TryContinue(BlockProcessor processor, Block block)
    {
        if (processor.IsBlankLine)
        {
            return BlockState.Continue;
        }

        var slice = processor.Line;
        var NestedColumn = (NestedColumnBlock)block;

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, new string(':', NestedColumn.ColonCount)))
        {
            return BlockState.Continue;
        }

        ExtensionsHelper.SkipSpaces(ref slice);

        if (!ExtensionsHelper.MatchStart(ref slice, EndString, false))
        {
            return BlockState.Continue;
        }

        var c = ExtensionsHelper.SkipSpaces(ref slice);

        if (!c.IsZero())
        {
            _context.LogWarning("invalid-nested-column", "NestedColumn have some invalid chars in the ending.", block);
        }

        block.UpdateSpanEnd(slice.End);

        return BlockState.BreakDiscard;
    }
}
