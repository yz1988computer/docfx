﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Markdig.Syntax;

namespace Microsoft.DocAsCode.Build.OverwriteDocuments;

[Serializable]
public class MarkdownPropertyModel
{
    public string PropertyName { get; set; }

    public Block PropertyNameSource { get; set; }

    public List<Block> PropertyValue { get; set; }
}
