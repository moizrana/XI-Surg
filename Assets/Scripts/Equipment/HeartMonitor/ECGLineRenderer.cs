using UnityEngine;
using System.Collections.Generic;

public class ECGLineRenderer : MonoBehaviour
{
    public ECGSignalGenerator generator;
    private LineRenderer lineRenderer;

    public float xSpacing = 0.01f;         // Horizontal spacing between points
    public int maxSamples = 500;            // Number of samples visible on screen (~2 seconds)
    public float samplesPerSecond = 250f;  // Sample rate for ECG data

    private Queue<float> dataBuffer = new Queue<float>();
    private float timer = 0f;
    private float sampleInterval;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (generator == null)
        {
            Debug.LogError("ECG Generator not assigned.");
            enabled = false;
            return;
        }

        lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 0.005f);
        sampleInterval = 1f / samplesPerSecond;

        // Initialize buffer with zeros for a clean start
        for (int i = 0; i < maxSamples; i++)
            dataBuffer.Enqueue(0f);

        DrawWave(new List<float>(dataBuffer));
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Generate as many samples as needed to keep up with real time
        while (timer >= sampleInterval)
        {
            timer -= sampleInterval;
            float newSample = generator.GenerateNextSample();

            if (dataBuffer.Count >= maxSamples)
                dataBuffer.Dequeue();

            dataBuffer.Enqueue(newSample);
        }

        DrawWave(new List<float>(dataBuffer));
    }

    void DrawWave(List<float> data)
    {
        lineRenderer.positionCount = data.Count;
        int i = 0;
        foreach (var val in data)
        {
            lineRenderer.SetPosition(i, new Vector3(i * xSpacing, val, 0));
            i++;
        }
    }
}
