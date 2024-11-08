// <copyright file="GraphRenderer.cs" company="Lukas Holecy">
// Copyright (c) Lukas Holecy. All rights reserved.
// </copyright>

namespace Holecy.Console.Dependencies.Graph;

using System;
using System.IO;
using System.Reflection;
using Jint;

public class GraphRenderer
{
    private readonly Engine _engine;

    public GraphRenderer()
    {
        var vizJsCode = LoadEmbeddedVizJs();
        this._engine = new Engine(cfg => cfg.AllowClr());
        this._engine.Execute(vizJsCode);
    }

    public string RenderGraph(string dotGraph)
    {
        // JavaScript code to create Viz instance and render SVG
        // var jsScript = $@"
        //     const viz = new Viz();
        //     viz.renderSVGElement(`
        //         {dotGraph}
        //     `)
        //     .catch(error => {{ throw error; }});
        // ";

        var jsScript = @"
            import { instance } from ""@viz-js/viz"";
            instance().then(viz => {
                const svg = viz.renderSVGElement(""dotContent"");
                return svg;
            });
            ";

        // Execute and get the result as a JavaScript Promise
        this._engine.SetValue("dotContent", dotGraph);
        var svgPromise = this._engine.Evaluate(jsScript).AsString();

        // Wait for the Promise to resolve and retrieve the SVG as a string
        // var svgResult = svgPromise.ToObject() as string;
        return svgPromise;
    }

    static string LoadEmbeddedVizJs()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "Holecy.Console.Dependencies.Resources.viz-standalone.js";

        using Stream stream = assembly.GetManifestResourceStream(resourceName) ?? new MemoryStream();
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
