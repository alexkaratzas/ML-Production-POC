using System;
using MLPoc.Common.Domain;
using MLPoc.Common.Messages;

namespace MLPoc.TimeSeriesAggregator
{
    public class PartialDataPoint
    {
        private readonly DataPoint _dataPoint;

        private bool _spotPriceReceived;
        private bool _windForecastReceived;
        private bool _pvForecastReceived;
        private bool _priceDeviationReceived;

        public PartialDataPoint(DateTime dateTime)
        {
            _dataPoint = new DataPoint
            {
                DateTime = dateTime
            };
        }

        public bool ReadyToSave => ReadyForPrediction && _priceDeviationReceived;

        public bool ReadyForPrediction =>
            _spotPriceReceived && _windForecastReceived && _pvForecastReceived;

        public void FeatureReceived(TimeSeriesFeature message)
        {
            if (message is SpotPriceMessage spotPriceMessage)
            {
                EnsureValidDate(spotPriceMessage.DateTime);
                _spotPriceReceived = true;
                _dataPoint.SpotPrice = spotPriceMessage.SpotPrice;
            }
            else if (message is WindForecastMessage windForecastMessage)
            {
                EnsureValidDate(windForecastMessage.DateTime);
                _windForecastReceived = true;
                _dataPoint.WindForecast = windForecastMessage.WindForecast;
            }
            else if (message is PvForecastMessage pvForecastMessage)
            {
                EnsureValidDate(pvForecastMessage.DateTime);
                _pvForecastReceived = true;
                _dataPoint.PvForecast = pvForecastMessage.PvForecast;
            }
            else if (message is PriceDeviationMessage priceDeviationMessage)
            {
                EnsureValidDate(priceDeviationMessage.DateTime);
                _priceDeviationReceived = true;
                _dataPoint.PriceDeviation = priceDeviationMessage.PriceDeviation;
            }

            void EnsureValidDate(DateTime dateTime)
            {
                if (dateTime != _dataPoint.DateTime)
                {
                    throw new Exception("Result received for different data point");
                }
            }
        }

        public DataPoint GetDataPoint()
        {
            return _dataPoint;
        }
    }
}