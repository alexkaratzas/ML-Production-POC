using System;
using MLPoc.Bus.Messages;

namespace MLPoc.TimeSeriesAggregator
{
    public class PartialDataPoint
    {
        private readonly DataPoint _dataPoint;

        private bool _x1Received;
        private bool _x2Received;
        private bool _x3Received;
        private bool _x4Received;
        private bool _x5Received;
        private bool _yReceived;

        public PartialDataPoint(DateTime dateTime)
        {
            _dataPoint = new DataPoint
            {
                DateTime = dateTime
            };
        }

        public bool ResultComplete => _x1Received && _x2Received && _x3Received && _x4Received && _x5Received && _yReceived;

        public void FeatureReceived(TimeSeriesFeature message)
        {
            if (message is X1Message msg1)
            {
                EnsureValidDate(msg1.DateTime);
                _x1Received = true;
                _dataPoint.X1 = msg1.X1;
            }
            else if (message is X2Message msg2)
            {
                EnsureValidDate(msg2.DateTime);
                _x2Received = true;
                _dataPoint.X2 = msg2.X2;
            }
            else if (message is X3Message msg3)
            {
                EnsureValidDate(msg3.DateTime);
                _x3Received = true;
                _dataPoint.X3 = msg3.X3;
            }
            else if (message is X4Message msg4)
            {
                EnsureValidDate(msg4.DateTime);
                _x4Received = true;
                _dataPoint.X4 = msg4.X4;
            }
            else if (message is X5Message msg5)
            {
                EnsureValidDate(msg5.DateTime);
                _x5Received = true;
                _dataPoint.X5 = msg5.X5;
            }
            else if (message is YMessage msgY)
            {
                EnsureValidDate(msgY.DateTime);
                _yReceived = true;
                _dataPoint.Y = msgY.Y;
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