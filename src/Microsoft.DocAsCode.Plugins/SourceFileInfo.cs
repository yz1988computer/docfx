﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.DocAsCode.Plugins;

public class SourceFileInfo
{
    public string DocumentType { get; private set; }

    public string SourceRelativePath { get; private set; }

    public static SourceFileInfo FromManifestItem(ManifestItem manifestItem)
    {
        return new SourceFileInfo
        {
            DocumentType = manifestItem.DocumentType,
            SourceRelativePath = manifestItem.SourceRelativePath,
        };
    }
}
