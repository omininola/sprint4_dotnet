using Microsoft.ML.Data;

namespace MachineLearning;

public class ModelInput
{
    [LoadColumn(0)]
    public float Area { get; set; }

    [LoadColumn(1)]
    public float Motorcycles { get; set; }
}