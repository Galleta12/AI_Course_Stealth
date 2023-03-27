using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Manager class to get access to some data
//You can click on the grid in the play scene and print the data of the nodes
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
            else 
            {
                clickPosition = ray.origin - ray.direction * ray.origin.z / ray.direction.z;
            }
            //get the node from the world position
            Node node = grid.NodeFromWorldPoint(clickPosition);

            Debug.Log("Clicked at node move penalty" + node.weightPenalty);
            Debug.Log("Clicked at node " + node.fCost);
            Debug.Log("Clicked at node move penalty" + node.hCost);
            Debug.Log("Clicked at node move penalty" + node.gCost);

        }
    
    }
}
