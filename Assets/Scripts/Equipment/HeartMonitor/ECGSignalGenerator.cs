using UnityEngine;
using System.Collections.Generic;

public class ECGSignalGenerator : MonoBehaviour
{
    public enum HeartCondition { Normal, Bradycardia, Tachycardia, Arrhythmia, Flatline }

    public HeartCondition condition = HeartCondition.Normal;
    public int samplesPerSecond = 250;

    private int sampleIndex = 0;  // track current sample index
    private float bpm;
    private float samplesPerBeat;

    void Start()
    {
        bpm = GetHeartRate(condition);
        samplesPerBeat = samplesPerSecond / (bpm / 60f);
    }

    public float GenerateNextSample()
    {
        float value = 0f;
        switch (condition)
        {
            case HeartCondition.Normal:
                value = SimulatePQRST(sampleIndex % (int)samplesPerBeat, samplesPerBeat);
                break;
            case HeartCondition.Bradycardia:
                value = SimulatePQRST(sampleIndex % (int)(samplesPerBeat * 1.5f), samplesPerBeat * 1.5f);
                break;
            case HeartCondition.Tachycardia:
                value = SimulatePQRST(sampleIndex % (int)(samplesPerBeat * 0.6f), samplesPerBeat * 0.6f);
                break;
            case HeartCondition.Arrhythmia:
                float offset = Mathf.Sin(sampleIndex * 0.01f) * Random.Range(0.8f, 1.2f);
                value = SimulatePQRST(sampleIndex % (int)(samplesPerBeat * offset), samplesPerBeat * offset);
                break;
            case HeartCondition.Flatline:
                value = 0f;
                break;
        }
        sampleIndex++;
        return value;
    }

    float GetHeartRate(HeartCondition condition)
    {
        switch (condition)
        {
            case HeartCondition.Bradycardia: return 45f;
            case HeartCondition.Tachycardia: return 120f;
            case HeartCondition.Arrhythmia: return 80f;
            case HeartCondition.Flatline: return 0f;
            default: return 75f;
        }
    }

    float SimulatePQRST(float index, float beatLength)
    {
        float t = index / beatLength;
        return Gaussian(t, 0.1f, 0.01f) * 0.2f +
               Gaussian(t, 0.25f, 0.005f) * -0.15f +
               Gaussian(t, 0.3f, 0.008f) * 1.0f +
               Gaussian(t, 0.35f, 0.005f) * -0.25f +
               Gaussian(t, 0.6f, 0.02f) * 0.35f;
    }

    float Gaussian(float t, float mean, float stdDev)
    {
        float a = (t - mean) / stdDev;
        return Mathf.Exp(-0.5f * a * a);
    }
}
