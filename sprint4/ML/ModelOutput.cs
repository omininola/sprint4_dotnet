using Microsoft.ML.Data;

namespace MachineLearning;

public class ModelOutput
{
    [ColumnName("Score")]
    public float PredictedMotorcycles { get; set; }
}