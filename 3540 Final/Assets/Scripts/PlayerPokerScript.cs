using UnityEngine;

public class PlayerProximityUI : MonoBehaviour
{
    public Transform target; 
    public float proximityDistance = 5f; 
    public GameObject canvasObject; 

    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < proximityDistance)
        {
            if (!canvasObject.activeSelf)
            {
                canvasObject.SetActive(true);
            }
        }
        else
        {
            if (canvasObject.activeSelf)
            {
                canvasObject.SetActive(false);
            }
        }
    }
}
