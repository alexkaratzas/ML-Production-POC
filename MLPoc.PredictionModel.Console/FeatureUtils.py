from pandas import DataFrame

def convert_json_to_dataframe(json_list):
    df = DataFrame(json_list)

    numeric_columns = ['SpotPrice', 'WindForecast', 'PvForecast', 'PriceDeviation']
    ml_columns = ['DateTime', 'SpotPrice', 'WindForecast', 'PvForecast', 'PriceDeviation']

    df[numeric_columns] = df[numeric_columns].astype(float)

    df = df[ml_columns]

    return df