using UnityEngine;

public class LineRendererScript : MonoBehaviour
{
    public static LineRendererScript instance;
    public Material LightBeam;
    internal LineRenderer lr;
    public Transform firstPoint, secondPoint;
    public bool isWorldSpace = true;

    [Header("Line Properties")]
    [Tooltip("Initial width of the line.")]
    [SerializeField] private float lineWidth = 0.15f;

    void Start()
    {
        instance = this;

        if (!gameObject.GetComponent<LineRenderer>())
        {
            lr = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        }
        else
        {
            lr = gameObject.GetComponent<LineRenderer>();
        }

        SetLrProperties();
        ResetRenderedLine();
        InvokeRepeating("DrawLine", 0.01f, 0.001f);
    }

    void SetLrProperties()
    {
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.material = LightBeam;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 0;
        lr.useWorldSpace = isWorldSpace;
    }

    void DrawLine()
    {
        if (firstPoint != null && secondPoint != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, firstPoint.position);
            lr.SetPosition(1, secondPoint.position);
        }
    }

    public void ResetRenderedLine()
    {
        lr.positionCount = 0;
        Debug.Log("Line has been Reset");
    }

 
    public float GetLineWidth()
    {
        return lr.startWidth;
    }

   
    public void SetLineWidth(float newWidth)
    {
        if (newWidth > 0)
        {
            lr.startWidth = newWidth;
            lr.endWidth = newWidth;
            lineWidth = newWidth; 
            Debug.Log($"Line width updated to {newWidth}");
        }
        else
        {
            Debug.LogWarning("Width must be greater than 0.");
        }
    }
}
