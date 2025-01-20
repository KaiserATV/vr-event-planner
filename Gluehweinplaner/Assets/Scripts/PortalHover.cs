using UnityEngine;

public class PortalVisualFeedback : MonoBehaviour
{
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer portalRenderer;

    private void Start()
    {
        portalRenderer = GetComponent<Renderer>();
        if (portalRenderer != null)
        {
            originalMaterial = portalRenderer.material;
        }
    }

    public void OnHoverEnter()
    {
        if (portalRenderer != null && highlightMaterial != null)
        {
            portalRenderer.material = highlightMaterial;
        }
    }

    public void OnHoverExit()
    {
        if (portalRenderer != null && originalMaterial != null)
        {
            portalRenderer.material = originalMaterial;
        }
    }
}
