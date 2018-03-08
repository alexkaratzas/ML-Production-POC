using MLPoc.Bus.Messages;

namespace MLPoc.TimeSeriesAggregator
{
    public delegate void FeatureReceivedEventHandler<in T>(object sender, T msg) where T: TimeSeriesFeature;
}