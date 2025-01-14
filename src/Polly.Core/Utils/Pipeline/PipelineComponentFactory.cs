﻿using System;
using System.Collections.Generic;
using Polly.Telemetry;

namespace Polly.Utils.Pipeline;

internal static class PipelineComponentFactory
{
    public static PipelineComponent FromPipeline(ResiliencePipeline pipeline) => pipeline.Component;

    public static PipelineComponent FromPipeline<T>(ResiliencePipeline<T> pipeline) => pipeline.Component;

    public static PipelineComponent FromStrategy(ResilienceStrategy strategy) => new BridgeComponent(strategy);

    public static PipelineComponent FromStrategy<T>(ResilienceStrategy<T> strategy) => new BridgeComponent<T>(strategy);

    public static PipelineComponent WithDisposableCallbacks(PipelineComponent component, IEnumerable<Action> callbacks)
    {
        if (!callbacks.Any())
        {
            return component;
        }

        return new ComponentWithDisposeCallbacks(component, callbacks.ToList());
    }

    public static PipelineComponent CreateComposite(
        IReadOnlyList<PipelineComponent> components,
        ResilienceStrategyTelemetry telemetry,
        TimeProvider timeProvider) => CompositeComponent.Create(components, telemetry, timeProvider);

    public static PipelineComponent CreateReloadable(
        ReloadableComponent.Entry initial,
        Func<ReloadableComponent.Entry> factory,
        ResilienceStrategyTelemetry telemetry) => new ReloadableComponent(initial, factory, telemetry);
}
