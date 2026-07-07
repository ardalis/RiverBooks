using System.Diagnostics;
using System.Diagnostics.Metrics;
using Ardalis.Result;
using Mediator;
using NSubstitute;
using Shouldly;
using Xunit;

namespace RiverBooks.SharedKernel.Tests;

public class OpenTelemetryBehavior_RecordsTelemetry
{
    [Fact]
    public async Task RecordsDurationMetricAndActivityTags()
    {
        // Arrange
        var request = new CreateTelemetryWidgetCommand("Widget");
        Activity? stoppedActivity = null;
        double? recordedDuration = null;
        KeyValuePair<string, object?>[] recordedTags = [];

        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == OpenTelemetryBehaviorInstrumentation.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => stoppedActivity = activity
        };
        ActivitySource.AddActivityListener(activityListener);

        using var meterListener = new MeterListener();
        meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == OpenTelemetryBehaviorInstrumentation.MeterName
            && instrument.Name == OpenTelemetryBehaviorInstrumentation.RequestDurationMetricName)
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };
        meterListener.SetMeasurementEventCallback<double>((_, measurement, tags, _) =>
        {
            recordedDuration = measurement;
            recordedTags = tags.ToArray();
        });
        meterListener.Start();

        var behavior = new OpenTelemetryBehavior<CreateTelemetryWidgetCommand, Result<string>>();

        var next = Substitute.For<MessageHandlerDelegate<CreateTelemetryWidgetCommand, Result<string>>>();
        next(Arg.Any<CreateTelemetryWidgetCommand>(), Arg.Any<CancellationToken>())
          .Returns(new ValueTask<Result<string>>(Result<string>.Success("widget-created")));

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldBe("widget-created");
        recordedDuration.ShouldNotBeNull();
        recordedDuration.Value.ShouldBeGreaterThanOrEqualTo(0);
        recordedTags.ShouldContain(tag => tag.Key == OpenTelemetryBehaviorInstrumentation.RequestNameTag
          && Equals(tag.Value, nameof(CreateTelemetryWidgetCommand)));
        stoppedActivity.ShouldNotBeNull();
        stoppedActivity.DisplayName.ShouldBe(nameof(CreateTelemetryWidgetCommand));
        stoppedActivity.TagObjects.ShouldContain(tag => tag.Key == OpenTelemetryBehaviorInstrumentation.DurationMillisecondsTag);
    }
}

internal record CreateTelemetryWidgetCommand(string Name) : IRequest<Result<string>>;
