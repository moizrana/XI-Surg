using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class XRDeviceSimulatorDebug : MonoBehaviour
{
    public GameObject leftHandVisual;
    public GameObject rightHandVisual;

    private XRDeviceSimulator simulator;

    void Start()
    {
        simulator = GetComponent<XRDeviceSimulator>();
        if (simulator == null)
            Debug.LogError("XRDeviceSimulator component not found on this GameObject.");
    }

    void Update()
    {
        if (simulator == null) return;

        // Show which hand is active: T=left, Y=right
        bool leftActive = Keyboard.current.tKey.isPressed;
        bool rightActive = Keyboard.current.yKey.isPressed;

        leftHandVisual.SetActive(leftActive);
        rightHandVisual.SetActive(rightActive);

        // Change color when grab is pressed

        // Left hand grab = Left Ctrl
        if (leftActive && Keyboard.current.leftCtrlKey.isPressed)
            SetColor(leftHandVisual, Color.green);
        else if (leftActive)
            SetColor(leftHandVisual, Color.red);

        // Right hand grab = Left Mouse Button
        if (rightActive && Mouse.current.leftButton.isPressed)
            SetColor(rightHandVisual, Color.green);
        else if (rightActive)
            SetColor(rightHandVisual, Color.blue);
    }

    private void SetColor(GameObject obj, Color color)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;
    }
}
