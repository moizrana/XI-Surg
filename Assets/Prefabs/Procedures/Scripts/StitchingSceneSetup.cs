using UnityEngine;

/// <summary>
/// Example script showing how to set up the stitching system programmatically
/// Attach this to an empty GameObject to auto-configure a basic scene
/// </summary>
public class StitchingSceneSetup : MonoBehaviour
{
    [Header("Scene Setup")]
    public bool autoSetupOnStart = false;
    public Material defaultThreadMaterial;
    public GameObject needlePrefab;
    
    [Header("Stitch Site Configuration")]
    public int numberOfStitchSites = 5;
    public float stitchSiteSpacing = 0.1f; // 10cm apart
    public Vector3 startPosition = Vector3.zero;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupStitchingScene();
        }
    }
    
    [ContextMenu("Setup Stitching Scene")]
    public void SetupStitchingScene()
    {
        Debug.Log("Setting up stitching scene...");
        
        // Create main container
        GameObject container = new GameObject("LacerationProcedure");
        
        // Create stitch sites container
        GameObject stitchSitesContainer = new GameObject("StitchSites");
        stitchSitesContainer.transform.SetParent(container.transform);
        
        // Create individual stitch sites
        for (int i = 0; i < numberOfStitchSites; i++)
        {
            CreateStitchSite(i, stitchSitesContainer.transform);
        }
        
        // Create and setup manager
        GameObject managerObj = new GameObject("StitchManager");
        managerObj.transform.SetParent(container.transform);
        StitchManager manager = managerObj.AddComponent<StitchManager>();
        
        // Find and assign all stitch sites to manager
        StitchSite[] allStitchSites = stitchSitesContainer.GetComponentsInChildren<StitchSite>();
        manager.stitchSites = new System.Collections.Generic.List<StitchSite>(allStitchSites);
        
        // Create needle if prefab is provided
        if (needlePrefab != null)
        {
            GameObject needle = Instantiate(needlePrefab);
            needle.name = "VR_Needle";
            
            // Add VRStitchTool if not already present
            if (needle.GetComponent<VRStitchTool>() == null)
            {
                needle.AddComponent<VRStitchTool>();
            }
        }
        
        Debug.Log($"Created {numberOfStitchSites} stitch sites with manager");
    }
    
    private void CreateStitchSite(int index, Transform parent)
    {
        // Create main stitch site object
        GameObject stitchSiteObj = new GameObject($"StitchSite_{index + 1}");
        stitchSiteObj.transform.SetParent(parent);
        
        // Position along a line
        Vector3 position = startPosition + Vector3.right * (index * stitchSiteSpacing);
        stitchSiteObj.transform.position = position;
        
        // Create first stitch point
        GameObject firstStitch = new GameObject("FirstStitch");
        firstStitch.transform.SetParent(stitchSiteObj.transform);
        firstStitch.transform.localPosition = Vector3.left * 0.02f; // 2cm to the left
        
        // Create second stitch point
        GameObject secondStitch = new GameObject("SecondStitch");
        secondStitch.transform.SetParent(stitchSiteObj.transform);
        secondStitch.transform.localPosition = Vector3.right * 0.02f; // 2cm to the right
        
        // Add and configure StitchSite component
        StitchSite stitchSite = stitchSiteObj.AddComponent<StitchSite>();
        stitchSite.firstStitch = firstStitch.transform;
        stitchSite.secondStitch = secondStitch.transform;
        
        // Configure settings
        stitchSite.detectionRadius = 0.05f;
        stitchSite.threadWidth = 0.002f;
        
        if (defaultThreadMaterial != null)
        {
            stitchSite.threadMaterial = defaultThreadMaterial;
        }
        
        // Add visual markers (optional)
        CreateVisualMarker(firstStitch.transform, Color.red);
        CreateVisualMarker(secondStitch.transform, Color.blue);
    }
    
    private void CreateVisualMarker(Transform parent, Color color)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.name = "VisualMarker";
        marker.transform.SetParent(parent);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localScale = Vector3.one * 0.01f; // 1cm diameter
        
        // Remove collider to avoid interference
        Collider markerCollider = marker.GetComponent<Collider>();
        if (markerCollider != null)
        {
            DestroyImmediate(markerCollider);
        }
        
        // Set color
        Renderer renderer = marker.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material markerMat = new Material(Shader.Find("Standard"));
            markerMat.color = color;
            markerMat.EnableKeyword("_EMISSION");
            markerMat.SetColor("_EmissionColor", color * 0.5f);
            renderer.material = markerMat;
        }
    }
    
    [ContextMenu("Clear Scene")]
    public void ClearScene()
    {
        GameObject existing = GameObject.Find("LacerationProcedure");
        if (existing != null)
        {
            DestroyImmediate(existing);
            Debug.Log("Cleared existing scene setup");
        }
    }
}
