﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Json.Schema;
using Microsoft.DocAsCode.Common;

namespace Microsoft.DocAsCode.Build.SchemaDriven;

public class SchemaValidator
{
    private readonly JsonSchema _schema;

    static SchemaValidator()
    {
        SchemaRegistry.Global.Register(new("http://dotnet.github.io/docfx/schemas/v1.0/schema.json#"), MetaSchemas.Draft7);
        SchemaRegistry.Global.Register(new("https://dotnet.github.io/docfx/schemas/v1.0/schema.json#"), MetaSchemas.Draft7);
    }

    public SchemaValidator(string json)
    {
        _schema = JsonSchema.FromText(json, new() { AllowTrailingCommas = true });
    }

    public void Validate(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var result = _schema.Evaluate(JsonDocument.Parse(json), new EvaluationOptions
        {
            ValidateAgainstMetaSchema = false,
            OutputFormat = OutputFormat.List,
        });

        if (result.IsValid)
            return;

        foreach (var detail in result.Details)
        {
            if (detail.HasErrors)
            {
                foreach (var (type, message) in detail.Errors)
                {
                    Logger.LogError($"[{detail.InstanceLocation}] {type}: {message} ", code: "ViolateSchema");
                }
            }
        }
    }
}
