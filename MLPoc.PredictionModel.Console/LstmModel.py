from ml_libs import series_to_supervised
from sklearn.preprocessing import MinMaxScaler
#from matplotlib import pyplot
from pandas import read_csv
from pandas import DataFrame
from pandas import concat
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import mean_squared_error, r2_score
from keras.models import Sequential
from keras.layers import Dense
from keras.layers import LSTM
import math
import numpy as np

class LstmModel(object):
    """description of class"""

    def predict(self, record):
        pass

    def train(self, df):
        df = self.__prepare_dataframe(df)

        test_X, test_y, y_pred, inv_yhat, inv_y, r2, rmse, model, scaler = self.__train_and_score_lstm_with_T0_data(df, log_verbose=True)
        
        self.model = model
        self.scaler = scaler
        self.r2 = r2
        self.rmse = rmse

    def __prepare_dataframe(self, df):
        return df.loc[:, df.columns != 'DateTime']

    def __train_and_score_lstm_with_T0_data(self, data_frame, train_perc = 0.7, look_ahead=0, epochs = 50, optimizer='adam', pred_future=1, log_verbose=True, plot_charts = False):
        
        # load dataset
        dataset = data_frame.copy()
        
        dataset['YTminus1'] = dataset['y'].shift(1)
        dataset = dataset.dropna(axis=0, how='any')
        
        if pred_future == 1:
            values = dataset.iloc[:, dataset.columns != 'y'].values
            yvalues = dataset[['y']].values
        else:
            values = dataset.iloc[:1-pred_future, dataset.columns != 'y'].values
            yvalues = dataset[['y']].shift(1-pred_future).dropna(axis=0, how='any').values
        
        if log_verbose == True:
            print('Dataframe values:')
            print('Shape of values: ', values.shape)
            print(values[0:5,:])
            print('Shape of yvalues: ', yvalues.shape)
            print(yvalues[0:5,:])    
        
        # integer encode direction
        #encoder = LabelEncoder()
        #values[:,4] = encoder.fit_transform(values[:,4])
        # ensure all data is float
        values = values.astype('float32')
        # normalize features
        scaler = MinMaxScaler(feature_range=(0, 1))
        scaled = scaler.fit_transform(values)
        
        yscaled= scaler.fit_transform(yvalues)
        
        if log_verbose == True:
            print('Scaled values:')
            print(scaled[0:5,:])
        
        # specify the number of lag hours
        n_hours = look_ahead + 1;
        n_features = len(dataset.iloc[:, dataset.columns != 'y'].columns);
        
        # frame as supervised learning
        reframed = series_to_supervised(scaled, look_ahead, 1)
        
        # implicit shift so that the y from future_pred is used as target after series_to_supervised folds past window into each row
        yscaled = yscaled[len(yscaled) - len(reframed):]
        
        if log_verbose == True:
            print(reframed.shape)
            print(reframed.head())
        
        # split into train and test sets
        values = reframed.values
        train_sample_perc = train_perc;
        n_train_sample = math.floor(values.shape[0]*train_sample_perc)
        train = values[:n_train_sample, :]
        trainy_ = yscaled[:n_train_sample, :]
        test = values[n_train_sample:, :]
        testy_= yscaled[n_train_sample:, :]
        
        if log_verbose == True:
            print('Train')
            print(train[0:5,:])
            print(trainy_[0:5,:])
            print('Test')
            print(test[0:5,:])
            print(testy_[0:5,:])
    
        # split into input and outputs
        n_obs = (n_hours + 1) * n_features
        
        min_common_size = np.min([len(test), len(testy_)])-1
        train_X, train_y = train[:, :n_obs], trainy_[:,0]
        test_X, test_y = test[:min_common_size, :n_obs], testy_[:min_common_size, 0]
        
        if log_verbose == True:
            print(train_X.shape, len(train_X), train_y.shape)
            print('Train and test before 3D')
            print('Train X')
            print(train_X[0:5,:])
            print('Train Y')
            print(train_y[0:5])
            print('Test X')
            print(test_X[0:5,:])
            print('Test Y')
            print(test_y[0:5])
        
        # reshape input to be 3D [samples, timesteps, features]
        train_X = train_X.reshape((train_X.shape[0], n_hours, n_features))
        test_X = test_X.reshape((test_X.shape[0], n_hours, n_features))
        
        if log_verbose == True:
            print(train_X.shape, train_y.shape, test_X.shape, test_y.shape)
            print('Train and test after 3D')
            print('Train X')
            print(train_X[0:5,:])
            print('Train Y')
            print(train_y[0:5])
            print('Test X')
            print(test_X[0:5,:])
            print('Test Y')
            print(test_y[0:5])
        
        # design network
        model = Sequential()
        model.add(LSTM(50, input_shape=(train_X.shape[1], train_X.shape[2])))
        model.add(Dense(1))
        model.compile(loss='mae', optimizer=optimizer)
        # fit network
        model_verbosity = 2
        if log_verbose == False:
            model_verbosity = 0
        history = model.fit(train_X, train_y, epochs=epochs, batch_size=72, validation_data=(test_X, test_y), verbose=model_verbosity, shuffle=False)

        if plot_charts:
            #plot history
            pyplot.plot(history.history['loss'], label='train')
            pyplot.plot(history.history['val_loss'], label='test')
            pyplot.legend()
            pyplot.show()
        
        # make a prediction
        y_pred = model.predict(test_X)
        
        test_X = test_X.reshape((test_X.shape[0], n_hours*n_features))
        # invert scaling for forecast
        inv_yhat = np.concatenate((y_pred, test_X[:, -(n_features-1):]), axis=1)
        inv_yhat = scaler.inverse_transform(inv_yhat)
        inv_yhat = inv_yhat[:,0]
        # invert scaling for actual
        test_y = test_y.reshape((len(test_y), 1))
        inv_y = np.concatenate((test_y, test_X[:, -(n_features-1):]), axis=1)
        inv_y = scaler.inverse_transform(inv_y)
        inv_y = inv_y[:,0]
        # calculate RMSE
        rmse = math.sqrt(mean_squared_error(inv_y, inv_yhat))
        print('Test RMSE: %.3f' % rmse)
    
        r2 = r2_score(inv_y, inv_yhat)
        
        print('Test R2: %.3f' % r2)
        
        return test_X, test_y, y_pred, inv_yhat, inv_y, r2, rmse, model, scaler