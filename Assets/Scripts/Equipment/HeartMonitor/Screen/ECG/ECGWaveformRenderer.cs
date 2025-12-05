using System.Collections.Generic;
using UnityEngine;

public class ECGWaveformRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int maxPoints = 500;         // More points = smoother line
    public float xSpacing = 0.005f;     // Space between points (smaller for continuity)
    public float amplitude = 0.3f;
    public float speed = 1f;

    private Queue<float> ecgValues = new Queue<float>();
    private float scrollWidth;

    void Start()
    {
        if (!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();

        scrollWidth = maxPoints * xSpacing;

        for (int i = 0; i < maxPoints; i++)
            ecgValues.Enqueue(0);

        // Start centered in container
        transform.localPosition = new Vector3(-scrollWidth * 0.5f, 0, 0.1f);

        // Optional: line appearance
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.useWorldSpace = false;
    }

    void Update()
    {
        float value = GenerateFakeECGValue();

        ecgValues.Enqueue(value);
        if (ecgValues.Count > maxPoints)
            ecgValues.Dequeue();

        lineRenderer.positionCount = ecgValues.Count;

        int i = 0;
        foreach (var v in ecgValues)
        {
            float x = i * xSpacing;
            lineRenderer.SetPosition(i, new Vector3(x, v, 0));
            i++;
        }

        // Smooth scroll
        float offset = -Time.time * speed % scrollWidth;
        transform.localPosition = new Vector3(offset, 0, 0);
    }

    float GenerateFakeECGValue()
    {
        float t = Time.time * 5f;

        // Strong spike occasionally
        if (Mathf.Sin(t) > 0.99f)
            return amplitude;

        // Flat, but add tiny noise so it doesn’t look dead
        return Mathf.PerlinNoise(t, 0) * 0.01f;
    }
}
