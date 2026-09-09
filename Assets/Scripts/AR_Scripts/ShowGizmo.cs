using UnityEngine;

public class ShowGizmo : MonoBehaviour
{
    [SerializeField]
    bool isIcon;
    [SerializeField]
    string IconName;
    [SerializeField]
    GizmoType ThisGizmo = GizmoType.Cube;
    [SerializeField]
    float Radius = 1;
    [SerializeField]
    Vector3 Offset = Vector3.zero;
    [SerializeField]
    Color GizmoColor = Color.white;
    [SerializeField]
    Color LineColor = Color.green;
    [SerializeField]
    bool GizmoChildren;

    void OnDrawGizmos()
    {
        Vector3 GizmoPosition = transform.position + Offset;

        Gizmos.color = GizmoColor;
        if(ThisGizmo==GizmoType.Cube) Gizmos.DrawCube(GizmoPosition, new Vector3(Radius, Radius, Radius));
        if(ThisGizmo==GizmoType.Sphere) Gizmos.DrawSphere(GizmoPosition, Radius);
        if(ThisGizmo==GizmoType.WireCube) Gizmos.DrawWireCube(GizmoPosition, new Vector3(Radius, Radius, Radius));
        if(ThisGizmo==GizmoType.WireSphere) Gizmos.DrawWireSphere(GizmoPosition, Radius);
        if (ThisGizmo == GizmoType.Bounds)
        {
            var r = GetComponent<Renderer>();
            if (r == null)
                return;
            var bounds = r.localBounds;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(bounds.center + new Vector3(0, bounds.extents.y,0), bounds.extents * 2);
        }
            //Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + transform.localScale.y/2, transform.position.z), transform.localScale);

        if (isIcon && IconName.Length>0) Gizmos.DrawIcon(transform.position, IconName, true);

        if(transform.childCount>0 && GizmoChildren)
        {
            Gizmos.color = GizmoColor;
            Vector3 childGizmoPosition = transform.GetChild(0).position + Offset;

            if (ThisGizmo == GizmoType.Cube) Gizmos.DrawCube(childGizmoPosition, new Vector3(Radius, Radius, Radius));
            if (ThisGizmo == GizmoType.Sphere) Gizmos.DrawSphere(childGizmoPosition, Radius);
            if (ThisGizmo == GizmoType.WireCube) Gizmos.DrawWireCube(childGizmoPosition, new Vector3(Radius, Radius, Radius));
            if (ThisGizmo == GizmoType.WireSphere) Gizmos.DrawWireSphere(childGizmoPosition, Radius);
            if (ThisGizmo == GizmoType.Bounds)
            {
                var r = GetComponent<Renderer>();
                if (r == null)
                    return;
                var bounds = r.localBounds;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(bounds.center + new Vector3(0, bounds.extents.y, 0), bounds.extents * 2);
            }
            Gizmos.color = LineColor;
            Gizmos.DrawLine(transform.position, transform.GetChild(0).position);
        }
    }

    enum GizmoType
    {
        Cube,
        Sphere,
        WireCube,
        WireSphere,
        Bounds
    }
}
