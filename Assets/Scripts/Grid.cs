using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {


//Turn on if you want to display the grid on the gizmos in the inspector
public bool displayGridGizmos;


// lets remember that on the world position. The z component is the y component on our grid
//Layer to check unwakableMask
public LayerMask UnwalkableMask;
//the size of the grid
public Vector2 GridWorldSize;
//radius of node. Space of each node will cover
public float NodeRadius;
// all the nodes that are walkable by the enemy. Where we want to add move panalty for the weight
public NodeType[] walkableRegions;
//obtacle proximity. Each node that collides with an obstacle we want to add it an obstacle proximity penalty
// THis is not working properly this value is only added to the layer unwalkable (enemies, obstacles)
public int obstacleProximityPenalty = 10;
//keep track of the panalty of each layer
private Dictionary<int,int> walkableRegionsDictionary = new Dictionary<int, int>();
//this will just get a reference of all the mask where we can walk and like that apply the collisions and movement penalty
public LayerMask walkableMask;
//two 2d array that represents the gird
Node[,] grid;

//Varaible to keep update of the node diamater and the grid size

private float nodeDiameter;
private int gridSizeX, gridSizeY;
// keep track of the penaly min and max. This will be used on the gizmos for visualising the grid with colors

private int weightMin = int.MaxValue;
private int weightMax = int.MinValue;


//Base on node radius how many nodes can we fit on the grid
private void Awake() {
    nodeDiameter = NodeRadius*2;
    // how many nodes we can fit on the world size x
    // integer
    gridSizeX = Mathf.RoundToInt(GridWorldSize.x/nodeDiameter);
    gridSizeY = Mathf.RoundToInt(GridWorldSize.y/nodeDiameter);
    
    //add each layer and define the layer index as a Key    
    foreach(NodeType n in walkableRegions){
       //Math way to detect the layers and detecting the name of the layer
       // we are going to add the layer as a key and the weight will be the value
        walkableMask = walkableMask | n.terrainMask.value;
        walkableRegionsDictionary.Add((int)Mathf.Log(n.terrainMask.value,2),n.weight);     
    }
    //Create the grid
    CreateGrid();
    
   
    //StartCoroutine("CheckChangesGrid",2f);

}




    // IEnumerator CheckChangesGrid(float delay){
    //         while(true){
    //             yield return new WaitForSeconds(delay);
    //             ChangesGrid();
    //         }
    // }



private void Update() {
    
    CreateGrid();
}


//get max size of the node 
public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
}





//Function to create the grid. This will also be called every frame so we can update the changes that happen on the environment
private void CreateGrid()
{
   
   if(grid == null){
    // set the grid array size with these varaibles
    grid = new Node[gridSizeX,gridSizeY];
    //Debug.Log("Helloda");
   
   }
    //bottom left corner of our world
    Vector3 worldBottomLeft = transform.position - Vector3.right * GridWorldSize.x/2 - Vector3.forward * GridWorldSize.y/2; 
   //Loop through all the position to check collision and check unwakable nodes
   for(int x =0; x<gridSizeX; x++){
        for(int y=0; y<gridSizeY; y++){
             // get the world position
             // Each point that the node will occupy in the world
             Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + NodeRadius) + Vector3.forward * (y* nodeDiameter + NodeRadius);
             //Check collision with unwalkable mask
             bool walkable = CheckCollisionNode(worldPoint, UnwalkableMask);
             //for check penalty. This wil change acordingly to the penalty of the layers
             int weightMovement = 0;
             //raycast to detect collision with the layers
			Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
			RaycastHit hit;
			//We just want to check the raycast on layers where it can walk. Since the walkable layers are the ones with penalty moves (weights)
            if (Physics.Raycast(ray,out hit, 100, walkableMask)) {
				//the movement penaly is assigned to the value with the key of the layers where the raycast detects a collision
                walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out weightMovement);
               // Debug.Log(weightMovement);
			}
			//if is not walkable we want to add an obstacle proximity penalty.
            //However this is not working as expected
            if (!walkable) {
					weightMovement += obstacleProximityPenalty;
			}

            //this is to check the max and min move penalties
            //this is useful to use in the gizmos for lerping between to colors and check which zones of the grid are dangerous(The zone that have a bigger weight)
            if (weightMovement > weightMax) {
					weightMax = weightMovement;
			}
			if (weightMovement < weightMin) {
					weightMin = weightMovement;
			}
             // create new Node
             Node new_node = new Node(walkable, worldPoint,x,y,weightMovement);
             grid[x,y] = new_node;
             
           

        }
   }

    // Debug.Log("This is max" + weightMax);
    // Debug.Log("This is min" + weightMin);
    
    

}





private bool CheckCollisionNode(Vector3 worldPoint, LayerMask maskcollison){
    //true if we don't collide with anything
    //if there is a collison walkable will be false
    return !(Physics.CheckSphere(worldPoint,NodeRadius,UnwalkableMask)); 
}

	



//Get the node position from the world point
//First get the percentage of how far along from the grid is. So for example if it is on the further left positon the percentage is 0. Middle 0. Far Right 1
//like that we can now in whic node a world point belongs to
public Node NodeFromWorldPoint(Vector3 worldPosition){

    float percentX = (worldPosition.x + GridWorldSize.x/2) / GridWorldSize.x;
    float percentY = (worldPosition.z + GridWorldSize.y/2) / GridWorldSize.y;
    //we just want values between 0 and 1
    // If the worldpositon is outside the grid. We don't want to get errors
    percentX = Mathf.Clamp01(percentX);
    percentY=  Mathf.Clamp01(percentY);

    //Get x and y indix of the grid array
    int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
    int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
    return grid[x,y];



}
    public List<Node> GetNeighbours(Node node) {
            
            //this loop in a 3 way on the x and y axis
            // We this we can also get the neighbors on the diagonals
            //Since we start -1 and we start on 0 we want skip that
            
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    
                    int checkX = node.gridPosX + x;
                    int checkY = node.gridPosY + y;
                    //dont get out of the grid range
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                        neighbours.Add(grid[checkX,checkY]);
                    }
                }
            }

            return neighbours;
    }




// get the path of the node
//public List<Node> path;





//Draw a representation of the grid for debuggin
 private void OnDrawGizmos()
{
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(transform.position,new Vector3(GridWorldSize.x,1,GridWorldSize.y));

        if(grid !=null && displayGridGizmos){
            
            foreach(Node n in grid){
                Gizmos.color = Color.Lerp (Color.white, Color.cyan, Mathf.InverseLerp (weightMin, weightMax, n.weightPenalty));
                //if there is collision red otherwise withe
                Gizmos.color = (n.Walkable)?Gizmos.color:Color.red;
                //Gizmos.color = (n.Walkable)?Color.white:Color.red;
                // if(path !=null){
                //     if(path.Contains(n)){
                //         Gizmos.color = Color.black;
                //     }
                // }
                // // to have a bit of space on each node we can substract the size of the node
                //Gizmos.DrawCube(n.WorldPosition,Vector3.one * (nodeDiameter-.1f));     
                Gizmos.DrawWireCube(n.WorldPosition,Vector3.one * (nodeDiameter));     

            }
        }
   

}



//Class for the layer to detect the layer of each node and their weight
[System.Serializable]
public class NodeType{
    public LayerMask terrainMask;
    public int weight;
}


}