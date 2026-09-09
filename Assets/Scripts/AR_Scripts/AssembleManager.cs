using UnityEngine;

public class AssembleManager : MonoBehaviour
{
    public Transform[] parts;
    private Vector3[] originalPos;
    public float distance = 0.2f;

    void Start()
    {
        originalPos = new Vector3[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            originalPos[i] = parts[i].localPosition;
    }

    public void Disassemble()
    {
        for (int i = 0; i < parts.Length; i++)
            parts[i].localPosition = originalPos[i] + Vector3.up * distance * i;
    }

    public void Assemble()
    {
        for (int i = 0; i < parts.Length; i++)
            parts[i].localPosition = originalPos[i];
    }
}
