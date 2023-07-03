// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Esprima;
using Microsoft.DocAsCode.Common;

namespace Microsoft.DocAsCode.Build.Engine;

internal class PreprocessorWithResourcePool : ITemplatePreprocessor
{
    private readonly ResourcePoolManager<ITemplatePreprocessor> _preprocessorPool;

    public PreprocessorWithResourcePool(Func<ITemplatePreprocessor> creater, int maxParallelism)
    {
        _preprocessorPool = ResourcePool.Create(creater, maxParallelism);
        try
        {
            using var preprocessor = _preprocessorPool.Rent();
            var inner = preprocessor.Resource;
            ContainsGetOptions = inner.ContainsGetOptions;
            ContainsModelTransformation = inner.ContainsModelTransformation;
            Path = inner.Path;
            Name = inner.Name;
        }
        catch (Exception e)
        {
            _preprocessorPool = null;
            Logger.LogWarning(
                e.InnerException is ParserException parserEx
                ? $"\"{parserEx.Source}\" not a valid template preprocessor, ignored: {parserEx.Message}"
                : $"Not a valid template preprocessor, ignored: {e.Message}"
            );
        }
    }

    public bool ContainsGetOptions { get; }

    public bool ContainsModelTransformation { get; }

    public string Path { get; }

    public string Name { get; }

    public object GetOptions(object model)
    {
        if (!ContainsGetOptions)
        {
            return null;
        }

        using var lease = _preprocessorPool.Rent();
        try
        {
            return lease.Resource.GetOptions(model);
        }
        catch (Exception e)
        {
            throw new InvalidPreprocessorException($"Error running GetOptions function inside template preprocessor: {e.Message}");
        }
    }

    public object TransformModel(object model)
    {
        if (!ContainsModelTransformation)
        {
            return model;
        }

        using var lease = _preprocessorPool.Rent();
        try
        {
            return lease.Resource.TransformModel(model);
        }
        catch (Exception e)
        {
            throw new InvalidPreprocessorException($"Error running Transform function inside template preprocessor: {e.Message}");
        }
    }
}
