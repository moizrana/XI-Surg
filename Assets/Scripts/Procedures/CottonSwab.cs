using UnityEngine;

public class CottonSwab : MonoBehaviour
{
    public bool isSoaked = false;

    public void SetSoaked()
    {
        isSoaked = true;

        // Optional: Visual cue when soaked
        GetComponent<Renderer>().material.color = Color.cyan;
    }
}
