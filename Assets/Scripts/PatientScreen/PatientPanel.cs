using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PatientPanel : MonoBehaviour
{
    public PatientInfoPanel panel;

    // Flask server base URL
    private string baseUrl = "https://smarthospitalbackend.onrender.com";

    // Patient ID to fetch
    public string patientId = "patient_1";

    private void Start()
    {
        StartCoroutine(FetchPatientDataRoutine());
    }

    private IEnumerator FetchPatientDataRoutine()
    {
        while (true)
        {
            yield return FetchPatientData(patientId);
            yield return new WaitForSeconds(5f); // update every 5 seconds
        }
    }

    private IEnumerator FetchPatientData(string id)
    {
        string url = $"{baseUrl}/patients/{id}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to fetch patient data: {www.error}");
            }
            else
            {
                string json = www.downloadHandler.text;
                Debug.Log("Received JSON: " + json);

                PatientData patient = JsonUtility.FromJson<PatientData>(json);

                if (patient == null || patient.baseline == null)
                {
                    Debug.LogError("Patient or baseline data is null!");
                }
                else
                {
                    panel.DisplayPatient(patient); // Delegate display to UI panel
                }
            }
        }
    }
}
