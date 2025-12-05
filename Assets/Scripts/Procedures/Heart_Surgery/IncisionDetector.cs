using UnityEngine;

public class IncisionDetector : MonoBehaviour
{
    public GameObject scalpel;
    public GameObject skinLayer;
    public GameObject retractorSpawnPointLeft;
    public GameObject retractorSpawnPointRight;
    private bool cutMade = false;

    private void OnTriggerStay(Collider other)
    {
        if (!cutMade && other.gameObject == scalpel)
        {
            // Optionally check scalpel movement distance or time
            cutMade = true;
            Debug.Log("Incision made.");
            skinLayer.SetActive(false); // Hide skin
            retractorSpawnPointLeft.SetActive(true);  // show left visual cue
            retractorSpawnPointRight.SetActive(true); // show right visual cue
            Debug.Log("Incision made. Visual cues for retractors shown.");
        }
    }
}
