using MachineLearning;
using Microsoft.ML;

public class PredictionService : IDisposable
{
    private readonly string _dataPath;
    private readonly string _modelPath;
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private PredictionEngine<ModelInput, ModelOutput> _predEngine;

    public PredictionService(IHostEnvironment env)
    {
        var mlFolder = Path.Combine(env.ContentRootPath, "ML");
        Directory.CreateDirectory(mlFolder);
        _dataPath = Path.Combine(mlFolder, "data.csv");
        _modelPath = Path.Combine(mlFolder, "motorcycle_parking_model.zip");

        _mlContext = new MLContext(seed: 0);

        if (File.Exists(_modelPath))
        {
            LoadModel();
        }
        else
        {
            if (!File.Exists(_dataPath))
                throw new FileNotFoundException($"Arquivo de dados não encontrado em: {_dataPath}. Coloque data.csv na pasta ML.");

            TrainAndSaveModel();
        }

        _predEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(_model);
    }

    private void LoadModel()
    {
        using var fs = File.OpenRead(_modelPath);
        _model = _mlContext.Model.Load(fs, out _);
    }

    private void TrainAndSaveModel()
    {
        var dataView = _mlContext.Data.LoadFromTextFile<ModelInput>(_dataPath, hasHeader: true, separatorChar: ',');

        var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(ModelInput.Motorcycles))
            .Append(_mlContext.Transforms.Concatenate("Features", nameof(ModelInput.Area)))
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features"));

        _model = pipeline.Fit(dataView);

        using var fs = File.Create(_modelPath);
        _mlContext.Model.Save(_model, dataView.Schema, fs);
    }

    public ModelOutput Predict(float area)
    {
        var input = new ModelInput { Area = area };
        return _predEngine.Predict(input);
    }

    public void Dispose()
    {
        _predEngine = null;
        _model = null;
    }
}