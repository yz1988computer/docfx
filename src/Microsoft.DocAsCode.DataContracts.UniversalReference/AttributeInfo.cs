﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DocAsCode.DataContracts.Common;
using Microsoft.DocAsCode.YamlSerialization;

using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Microsoft.DocAsCode.DataContracts.UniversalReference;

[Serializable]
public class AttributeInfo
{
    [YamlMember(Alias = "type")]
    [JsonProperty("type")]
    [UniqueIdentityReference]
    public string Type { get; set; }

    [YamlMember(Alias = "ctor")]
    [JsonProperty("ctor")]
    public string Constructor { get; set; }

    [YamlMember(Alias = "arguments")]
    [JsonProperty("arguments")]
    public List<ArgumentInfo> Arguments { get; set; }

    [YamlMember(Alias = "namedArguments")]
    [JsonProperty("namedArguments")]
    public List<NamedArgumentInfo> NamedArguments { get; set; }

    [ExtensibleMember]
    [JsonExtensionData]
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}
