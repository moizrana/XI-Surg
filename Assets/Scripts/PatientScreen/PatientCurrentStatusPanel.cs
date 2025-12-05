using UnityEngine;
using TMPro;

public class PatientCurrentStatusPanel : MonoBehaviour
{
    public Transform contentHolder;     // The vertical layout group
    public GameObject rowPrefab;        // Assign the row prefab in Inspector

    public void AddRow(string label, string value)
    {
        Debug.Log($"Adding Row: {label} = {value}");

        if (rowPrefab == null || contentHolder == null)
        {
            Debug.LogError("RowPrefab or ContentHolder not assigned!");
            return;
        }

        GameObject row = Instantiate(rowPrefab, contentHolder);
        TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
        Debug.Log($"Row prefab instantiated with {texts.Length} Text components");

        if (texts.Length >= 2)
        {
            texts[0].text = label;
            texts[1].text = value;
        }
        else
        {
            Debug.LogError("Row prefab doesn't have two TextMeshProUGUI children!");
        }
    }


    public void DisplayPatient(PatientData data)
    {
        Debug.Log("DisplayPatient() called");

        ClearPanel();

        Debug.Log("Adding rows...");
        AddRow("Name", data.name);
        AddRow("Age", data.age.ToString());
        AddRow("Diagnosis", data.currentStatus?.diagnosis ?? "null");
        AddRow("Status", data.currentStatus?.status ?? "null");
        AddRow("Mobility", data.currentStatus?.mobility ?? "null");
        AddRow("Consciousness", data.currentStatus?.consciousness ?? "null");
        // Add more rows as per need:
    }


    public void ClearPanel()
    {
        foreach (Transform child in contentHolder)
        {
            Destroy(child.gameObject);
        }
    }
}
