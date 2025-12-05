using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class HeartMonitorDisplay : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    public string apiUrl = "https://smarthospitalbackend.onrender.com";

    private Coroutine fetchRoutine;

    void Start()
    {
        fetchRoutine = StartCoroutine(GetHeartData());
    }

    void OnDestroy()
    {
        if (fetchRoutine != null)
            StopCoroutine(fetchRoutine);
    }

    IEnumerator GetHeartData()
    {
        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                HeartData data = JsonUtility.FromJson<HeartData>(json);
                if (displayText != null)
                    displayText.text = $"HR: {data.heart_rate} bpm\nSpO₂: {data.spo2}%";
            }
            else
            {
                if (displayText != null)
                    displayText.text = "Error fetching data";
            }

            yield return new WaitForSeconds(1);
        }
    }
}

[System.Serializable]
public class HeartData
{
    public int heart_rate;
    public int spo2;
}
