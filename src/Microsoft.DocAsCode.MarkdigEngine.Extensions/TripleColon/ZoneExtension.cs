// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace Microsoft.DocAsCode.MarkdigEngine.Extensions;

public class ZoneExtension : ITripleColonExtensionInfo
{
    private static readonly Regex pivotRegex = new(@"^\s*(?:[a-z0-9-]+)(?:\s*,\s*[a-z0-9-]+)*\s*$");
    private static readonly Regex pivotReplaceCommasRegex = new(@"\s*,\s*");
    public string Name => "zone";
    public bool SelfClosing => false;

    public bool Render(HtmlRenderer renderer, MarkdownObject markdownObject, Action<string> logWarning)
    {
        return false;
    }

    public bool TryProcessAttributes(IDictionary<string, string> attributes, out HtmlAttributes htmlAttributes, out IDictionary<string, string> renderProperties, Action<string> logError, Action<string> logWarning, MarkdownObject markdownObject)
    {
        htmlAttributes = null;
        renderProperties = null;
        var target = string.Empty;
        var pivot = string.Empty;
        foreach (var attribute in attributes)
        {
            var name = attribute.Key;
            var value = attribute.Value;
            switch (name)
            {
                case "target":
                    if (value != "docs" && value != "chromeless" && value != "pdf")
                    {
                        logError($"Unexpected target \"{value}\". Permitted targets are \"docs\", \"chromeless\" or \"pdf\".");
                        return false;
                    }
                    target = value;
                    break;
                case "pivot":
                    if (!pivotRegex.IsMatch(value))
                    {
                        logError($"Invalid pivot \"{value}\". Pivot must be a comma-delimited list of pivot names. Pivot names must be lower-case and contain only letters, numbers or dashes.");
                        return false;
                    }
                    pivot = value;
                    break;
                default:
                    logError($"Unexpected attribute \"{name}\".");
                    return false;
            }
        }

        if (target == string.Empty && pivot == string.Empty)
        {
            logError("Either target or privot must be specified.");
            return false;
        }
        if (target == "pdf" && pivot != string.Empty)
        {
            logError("Pivot not permitted on pdf target.");
            return false;
        }

        htmlAttributes = new HtmlAttributes();
        htmlAttributes.AddClass("zone");
        if (target != string.Empty)
        {
            htmlAttributes.AddClass("has-target");
            htmlAttributes.AddProperty("data-target", target);
        }
        if (pivot != string.Empty)
        {
            htmlAttributes.AddClass("has-pivot");
            htmlAttributes.AddProperty("data-pivot", pivot.Trim().ReplaceRegex(pivotReplaceCommasRegex, " "));
        }
        return true;
    }
    public bool TryValidateAncestry(ContainerBlock container, Action<string> logError)
    {
        while (container != null)
        {
            if (container is TripleColonBlock && ((TripleColonBlock)container).Extension.Name == this.Name)
            {
                logError("Zones cannot be nested.");
                return false;
            }
            container = container.Parent;
        }
        return true;
    }
}
