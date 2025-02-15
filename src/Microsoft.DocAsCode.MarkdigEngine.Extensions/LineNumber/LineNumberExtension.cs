// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Microsoft.DocAsCode.MarkdigEngine.Extensions;

public class LineNumberExtension : IMarkdownExtension
{
    private readonly Func<object, string> _getFilePath;

    public LineNumberExtension(Func<object, string> getFilePath = null)
    {
        _getFilePath = getFilePath ?? (file => file?.ToString());
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.PreciseSourceLocation = true;
        pipeline.DocumentProcessed += document =>
        {
            AddSourceInfoInDataEntry(document, _getFilePath(InclusionContext.File));
        };
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {

    }

    /// <summary>
    /// if context.EnableSourceInfo is true: add sourceFile, sourceStartLineNumber, sourceEndLineNumber in each MarkdownObject
    /// </summary>
    /// <param name="markdownObject"></param>
    /// <param name="context"></param>
    private static void AddSourceInfoInDataEntry(MarkdownObject markdownObject, string filePath)
    {
        if (markdownObject == null || filePath == null) return;

        // set linenumber for its children recursively
        if (markdownObject is ContainerBlock containerBlock)
        {
            foreach (var subBlock in containerBlock)
            {
                AddSourceInfoInDataEntry(subBlock, filePath);
            }
        }
        else if (markdownObject is LeafBlock leafBlock)
        {
            if (leafBlock.Inline != null)
            {
                foreach (var subInline in leafBlock.Inline)
                {
                    AddSourceInfoInDataEntry(subInline, filePath);
                }
            }
        }
        else if (markdownObject is ContainerInline containerInline)
        {
            foreach (var subInline in containerInline)
            {
                AddSourceInfoInDataEntry(subInline, filePath);
            }
        }

        // set linenumber for this object
        var htmlAttributes = markdownObject.GetAttributes();
        htmlAttributes.AddPropertyIfNotExist("sourceFile", filePath);
        htmlAttributes.AddPropertyIfNotExist("sourceStartLineNumber", markdownObject.Line + 1);
    }
}
