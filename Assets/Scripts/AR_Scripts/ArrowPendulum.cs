using UnityEngine;

public class ArrowPendulum : MonoBehaviour
{
    public float minAngle = -31f;
    public float maxAngle = 32f;
    public float speed = 2f;

    private void Update()
    {
        float angle = Mathf.Lerp(minAngle, maxAngle, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
