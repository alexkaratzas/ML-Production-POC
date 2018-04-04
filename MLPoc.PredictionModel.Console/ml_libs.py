from math import sqrt
from numpy import concatenate
from pandas import read_csv
from pandas import DataFrame
from pandas import concat
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import LabelEncoder
from sklearn.metrics import mean_squared_error, r2_score, confusion_matrix, precision_score, recall_score, accuracy_score, roc_auc_score
from keras.models import Sequential
from keras.layers import Dense
from keras.layers import LSTM


def series_to_supervised(data, n_in=1, n_out=1, dropnan=True):
    """
    	Frame a time series as a supervised learning dataset.
    	Arguments:
    		data: Sequence of observations as a list or NumPy array.
    		n_in: Number of lag observations as input (X).
    		n_out: Number of observations as output (y).
    		dropnan: Boolean whether or not to drop rows with NaN values.
    	Returns:
    		Pandas DataFrame of series framed for supervised learning.
    	"""
    n_vars = 1 if type(data) is list else data.shape[1]
    df = DataFrame(data)
    cols, names = list(), list()
    # input sequence (t-n, ... t-1)
    for i in range(n_in, 0, -1):
        cols.append(df.shift(i))
        names += [('var%d(t-%d)' % (j + 1, i)) for j in range(n_vars)]
    # forecast sequence (t, t+1, ... t+n)
    for i in range(0, n_out):
        cols.append(df.shift(-i))
        if i == 0:
            names += [('var%d(t)' % (j + 1)) for j in range(n_vars)]
        else:
            names += [('var%d(t+%d)' % (j + 1, i)) for j in range(n_vars)]
    # put it all together
    agg = concat(cols, axis=1)
    agg.columns = names
    # drop rows with NaN values
    if dropnan:
        agg.dropna(inplace=True)
    return agg

def train_test_split_preserving_order(features, target, train_size=0.7):
    test_size = 1 - train_size
    cut_off = int(len(features) * train_size)
    print(f'Using {cut_off} cut-off index')

    X_train = features.iloc[:cut_off, :]
    X_test = features.iloc[cut_off:, :]
    y_train = target[:cut_off]
    y_test = target[cut_off:]
    return X_train, X_test, y_train, y_test


def print_regression_scores(sl_y_test, lr_y_pred, model_descr):
    lr_rmse = sqrt(mean_squared_error(sl_y_test, lr_y_pred))
    r2 = r2_score(sl_y_test, lr_y_pred)
    print(f'{model_descr} RMSE: %.3f' % lr_rmse)
    print(f'{model_descr} R2: %.3f' % r2)

def print_classification_scores(sl_y_test, lr_y_pred, model_descr):
    classification_result = ClassificationRes(sl_y_test, lr_y_pred, model_descr)
    classification_result.print()
    return classification_result

def print_scores(sl_y_test, lr_y_pred, model_descr):
    print_regression_scores(sl_y_test, lr_y_pred, model_descr)
    print_classification_scores(sl_y_test, lr_y_pred, model_descr)
    print('')


class ClassificationRes(object):
    def __init__(self, y_test, y_pred, model_name):
        self.y_test_class = self.__class_number_as_positive_or_negative(y_test)
        self.y_pred_class = self.__class_number_as_positive_or_negative(y_pred)
        tn, fp, fn, tp = confusion_matrix(self.y_test_class, self.y_pred_class).ravel()

        self.model_name = model_name
        self.true_negatives = tn
        self.false_positives = fp
        self.false_negatives = fn
        self.true_positives = tp
        self.precision = precision_score(self.y_test_class, self.y_pred_class)
        self.accuracy = accuracy_score(self.y_test_class, self.y_pred_class)
        self.recall = recall_score(self.y_test_class, self.y_pred_class)
        self.auc = roc_auc_score(self.y_test_class, self.y_pred_class)

    def __class_number_as_positive_or_negative(self, num_arr):
        num_arr_class = []
        for i in range(0, len(num_arr)-1):
            if num_arr[i] > 0:
                num_arr_class.append(1)
            else:
                num_arr_class.append(0)

        return num_arr_class

    def print(self):
        print(f'{self.model_name} AUC: %.3f' % self.auc)
        print(f'{self.model_name} Accuracy: %.3f' % self.accuracy)
        print(f'{self.model_name} Precision: %.3f' % self.precision)
        print(f'{self.model_name} Recall: %.3f' % self.recall)
        print(f'{self.model_name} TP: {str(self.true_positives).rjust(6)} FP: {str(self.false_positives).rjust(6)}')
        print(f'{self.model_name} TN: {str(self.true_negatives).rjust(6)} FN: {str(self.false_negatives).rjust(6)}')
