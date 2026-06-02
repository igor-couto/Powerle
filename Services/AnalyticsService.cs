using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace EnergySourceGame.Services;

public sealed class AnalyticsService
{
    private readonly IJSRuntime _js;
    private readonly bool _enabled;
    private readonly string? _measurementId;

    public AnalyticsService(IJSRuntime js, IConfiguration configuration)
    {
        _js = js;
        _enabled = configuration.GetValue<bool>("Analytics:Enabled", false);
        _measurementId = configuration.GetValue<string>("Analytics:MeasurementId");
        if (string.IsNullOrWhiteSpace(_measurementId))
        {
            _measurementId = null;
        }
    }

    public ValueTask InitializeAsync()
    {
        if (!_enabled)
            return ValueTask.CompletedTask;

        return _js.InvokeVoidAsync("powerleAnalytics.initialize", _measurementId);
    }

    public ValueTask TrackPageViewAsync(string pageName, string pagePath)
    {
        if (!_enabled)
            return ValueTask.CompletedTask;

        return _js.InvokeVoidAsync("powerleAnalytics.trackPageView", pageName, pagePath);
    }

    public ValueTask TrackEventAsync(string eventName, object? data = null)
    {
        if (!_enabled)
            return ValueTask.CompletedTask;

        return _js.InvokeVoidAsync("powerleAnalytics.trackEvent", eventName, data);
    }
}
