using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class SimpleHighlightOnClick : MonoBehaviour
{
    public Color highlightColor = Color.yellow;

    [Header("Blink Settings")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 3f;
    public float blinkSpeed = 2f;

    private Material mat;
    private Color originalEmission;
    private bool highlightLocked;

    private float blinkTime;

    void Awake()
    {
        mat = GetComponent<Renderer>().material;

        // Store original emission
        if (mat.HasProperty("_EmissionColor"))
            originalEmission = mat.GetColor("_EmissionColor");
        else
            originalEmission = Color.black;
    }

    void Update()
    {
        if (highlightLocked)
        {
            DisableHighlight();
            return;
        }

        BlinkHighlight();
    }

    void BlinkHighlight()
    {
        blinkTime += Time.deltaTime * blinkSpeed;

        float intensity = Mathf.Lerp(
            minIntensity,
            maxIntensity,
            Mathf.PingPong(blinkTime, 1f)
        );

        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", highlightColor * intensity);
    }

    void DisableHighlight()
    {
        mat.SetColor("_EmissionColor", originalEmission);
    }

    // 👉 AR me tap / click hone par ye chalega
    void OnMouseDown()
    {
        highlightLocked = true;
        DisableHighlight();
    }

    // 🔓 Agar future me fir se enable karna ho
    public void EnableHighlightAgain()
    {
        highlightLocked = false;
        blinkTime = 0f;
    }
}
