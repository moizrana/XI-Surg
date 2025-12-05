using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReportOnGrab : MonoBehaviour
{
    public InjectionProcedureManager.Step stepToReport;
    public InjectionProcedureManager procedureManager;

    private void OnEnable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        interactable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (procedureManager != null)
        {
            procedureManager.ReportAction(stepToReport);
        }
    }
}
