namespace Project.lib;

public class ValuesAvgCalculator {
    private int _sampleCount;
    private readonly float[] _samples;
    public float Value { get; private set; }

    public ValuesAvgCalculator(int sampleCount) {
        _sampleCount = 0;
        _samples = new float[sampleCount];
    }

    public float AddAndGet(float value) {
        if (_sampleCount < _samples.Length)
            ++_sampleCount;
        else
            Array.Copy(_samples, 1, _samples, 0, _samples.Length - 1);
        _samples[_sampleCount - 1] = value;

        float result = 0;
        for (int i = 0; i < _sampleCount; i++) {
            result += _samples[i];
        }

        return Value = result / _sampleCount;
    }
}