using UnityEngine;

public class RotateInLoop : MonoBehaviour
{
    public float rotationSpeed = 50f; // Speed of rotation
    public GameObject rotateZObject;
    public GameObject rotateXObject;
    public GameObject[] rotateYObject;

    void Update()
    {
        
        if (rotateZObject != null)
        {
            rotateZObject.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

       
        if (rotateXObject != null)
        {
            rotateXObject.transform.Rotate(-rotationSpeed * Time.deltaTime, 0, 0);
        }


        if (rotateYObject != null)
            foreach (GameObject obj in rotateYObject)
            {
                {
                    obj.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                }
            }
    }
}