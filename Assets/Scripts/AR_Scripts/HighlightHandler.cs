using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightHandler : MonoBehaviour
{
    public Material highlightMaterial;
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private Renderer[] renderers;
    private static HighlightHandler currentlyHighlightedObject = null;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            originalMaterials[renderer] = renderer.material;
        }
    }

    public void HighlightForSeconds(float duration)
    {
        if (highlightMaterial != null)
        {
            if (currentlyHighlightedObject != null && currentlyHighlightedObject != this)
            {
                currentlyHighlightedObject.ResetHighlight();
            }

            StartCoroutine(HighlightObject(duration));
            currentlyHighlightedObject = this;
        }
    }

    private IEnumerator HighlightObject(float duration)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = highlightMaterial;
        }

        yield return new WaitForSeconds(duration);

        ResetHighlight();
    }

    private void ResetHighlight()
    {
        foreach (Renderer renderer in renderers)
        {
            if (originalMaterials.ContainsKey(renderer))
            {
                renderer.material = originalMaterials[renderer];
            }
        }
    }
}
