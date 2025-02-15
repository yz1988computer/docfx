// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.DocAsCode.DataContracts.ManagedReference;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace Microsoft.DocAsCode.Build.ManagedReference.BuildOutputs;

[Serializable]
public class ApiExceptionInfoBuildOutput
{
    [YamlMember(Alias = "type")]
    [JsonProperty("type")]
    public ApiNames Type { get; set; }

    [YamlMember(Alias = "description")]
    [JsonProperty("description")]
    public string Description { get; set; }

    private bool _needExpand = true;

    public static ApiExceptionInfoBuildOutput FromModel(ExceptionInfo model)
    {
        if (model == null)
        {
            return null;
        }
        return new ApiExceptionInfoBuildOutput
        {
            Type = ApiNames.FromUid(model.Type),
            Description = model.Description,
        };
    }

    public static ApiExceptionInfoBuildOutput FromModel(ExceptionInfo model, Dictionary<string, ApiReferenceBuildOutput> references, string[] supportedLanguages)
    {
        if (model == null)
        {
            return null;
        }
        return new ApiExceptionInfoBuildOutput
        {
            Type = ApiBuildOutputUtility.GetApiNames(model.Type, references, supportedLanguages),
            Description = model.Description,
            _needExpand = false,
        };
    }

    public void Expand(Dictionary<string, ApiReferenceBuildOutput> references, string[] supportedLanguages)
    {
        if (_needExpand)
        {
            _needExpand = false;
            Type = ApiBuildOutputUtility.GetApiNames(Type?.Uid, references, supportedLanguages);
        }
    }
}
