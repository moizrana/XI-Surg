using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ReportProcedureStepOnGrabAuto : MonoBehaviour
{
    public InjectionProcedureManager injectionManager;
    public WoundDressingProcedureManager woundDressingManager;

    [Header("Injection Step (if injectionManager is assigned)")]
    public InjectionProcedureManager.Step injectionStep;

    [Header("Wound Dressing Step (if woundDressingManager is assigned)")]
    public WoundDressingProcedureManager.Step woundDressingStep;

    private XRGrabInteractable interactable;

    [System.Obsolete]
    private void Awake()
    {
        // Optional: auto-find managers if not assigned
        if (injectionManager == null)
            injectionManager = FindObjectOfType<InjectionProcedureManager>();

        if (woundDressingManager == null)
            woundDressingManager = FindObjectOfType<WoundDressingProcedureManager>();
    }

    private void OnEnable()
    {
        interactable = GetComponent<XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnGrabbed);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Determine active procedure from central controller
        var activeProc = ProcedureController.ActiveProcedure;

        if (activeProc == ProcedureController.ProcedureType.Injection && injectionManager != null)
        {
            injectionManager.ReportAction(injectionStep);
        }
        else if (activeProc == ProcedureController.ProcedureType.WoundDressing && woundDressingManager != null)
        {
            woundDressingManager.ReportAction(woundDressingStep);
        }
        else
        {
            UINotificationManager.ShowWarning("No active procedure manager or no step assigned for current procedure.");
        }
    }
}
