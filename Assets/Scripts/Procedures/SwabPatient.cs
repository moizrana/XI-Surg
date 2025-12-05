using UnityEngine;

public class SwabPatient : MonoBehaviour
{
    public InjectionProcedureManager procedureManager;

    private void OnTriggerEnter(Collider other)
    {
        CottonSwab swab = other.GetComponent<CottonSwab>();
        if (swab != null && swab.isSoaked)
        {
            procedureManager.OnSkinSwabbed();
        }
    }
}
