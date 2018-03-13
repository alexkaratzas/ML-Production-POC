from pandas import DataFrame

def convert_json_to_dataframe(json_list):
    df = DataFrame(json_list)

    numeric_columns = ['X1', 'X2', 'X3', 'X4', 'X5', 'Y']
    ml_columns = ['DateTime', 'x1', 'x2', 'x3', 'x4', 'x5', 'y']

    df[numeric_columns] = df[numeric_columns].astype(float)

    df = df.rename(columns=dict(zip(numeric_columns, [col.lower() for col in numeric_columns])))

    df = df[ml_columns]

    return df;