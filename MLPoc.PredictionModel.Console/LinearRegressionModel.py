from sklearn.linear_model import LinearRegression
from ml_libs import print_scores, train_test_split_preserving_order

class LinearRegressionModel(object):
    """description of class"""

    def __init__(self):
        self.__model = LinearRegression()

    def train(self, features_df, target_df):
        X_train, X_test, y_train, y_test = train_test_split_preserving_order(features_df, target_df)

        y_train = y_train.values.reshape(len(y_train))
        y_test = y_test.values.reshape(len(y_test))

        self.__model.fit(X_train, y_train)

        y_pred = self.__model.predict(X_test)
    
        # Calculate error
        print_scores(y_test, y_pred, 'Linear Regression')

    def predict(self, df):
        return self.__model.predict(df)




