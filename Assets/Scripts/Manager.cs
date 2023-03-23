using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    
    
    private Grid grid;

    public LayerMask layer;
    
    
    private void Awake() {
        grid = GameObject.FindGameObjectWithTag("A_Star").GetComponent<Grid>();
    }
    
    private void Start()
    {
        
    }

    //For now we only check the movement penalty of where the player is pressing.
    private void Update()
    {
       
       
        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            //Debug.Log(mousePosition);

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            Vector3 clickPosition;

            // If the raycast hits an object, get the world position of the hit point
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
            {
                clickPosition = hit.point;
            }
            else // Otherwise, use the intersection point of the ray with the z=0 plane
            {
                clickPosition = ray.origin - ray.direction * ray.origin.z / ray.direction.z;
            }
            Node node = grid.NodeFromWorldPoint(clickPosition);

            Debug.Log("Clicked at node move penalty" + node.movementPenalty);
        }
    
    }
}
