using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {

	
   
    //reference to the grid component
    Grid grid;
    
    PlayerAI playerAI;

    

    
    //this will return the vector moves to where the player AI have to move
    public Vector3[] OnFound;

    //check if the path was found
    public bool PathFound;

    public bool isAStarHeap;
    

     // keep track of the state expansion
    [HideInInspector]
    public int stateExpansion = 0;
    //Stopwatch sw = new Stopwatch();


    private void Awake() {
      
        grid = GetComponent<Grid>();
        playerAI = GameObject.FindGameObjectWithTag("Player_AI").GetComponent<PlayerAI>();

    }


    private void Update() {
           
       
        
       
    }


    
    //this is called on the Player Ai Script.
    public void StartFindPath(Transform start, Transform target)
    {
        //StartCoroutine(A_Star(startNode,targetNode));
        if(isAStarHeap){

            A_Star(start.position,target.position);
        }else{

            A_Star_WithoutHeap(start.position,target.position);
        }

    }

    public void A_Star(Vector3 StartNode, Vector3 EndNode)
    {
        
        
        // Stopwatch sw = new Stopwatch();
		// sw.Start();
        //Initialize the waypoint to get the path
        Vector3[] waypoint = new Vector3[0];
        bool pathSuccess =false;
       
       
        //Get the position in grid base
        Node  startNode = grid.NodeFromWorldPoint(StartNode);
        Node targetNode = grid.NodeFromWorldPoint(EndNode);
      
        //Heap for the nodes that we should visit
        Heap<Node> open_set = new Heap<Node>(grid.MaxSize);
        //Nodes closed
        HashSet<Node> closed_set = new HashSet<Node>();
        //add start node to the open list
        open_set.Add(startNode);
        //loop until the open heap is 0
        while(open_set.Count >0){
            //get the node with the lower F cost
            Node currentNode = open_set.RemoveFirst();
            //add it to the closed list
            closed_set.Add(currentNode);
            stateExpansion++;
            //if this is true we can stop the while loop
            if(currentNode == targetNode){
                // sw.Stop();
                // print("Path found: " + sw.ElapsedMilliseconds + " ms");
                //Get Path to the end node
                pathSuccess = true;
               
                break;
                //return;
            }

            // loop on each of the neighbors and states of each node
            foreach(Node neighbour in grid.GetNeighbours(currentNode)){
                // if the neighbour is not walkable and is on the close set we want to skip
                if(!neighbour.Walkable || closed_set.Contains(neighbour)){
                    continue;
                }
                //we add the weight penalty of moving to that neighbour and we also get the heuristic to moving to neightbour
                int new_g_cost_to_neighbour = currentNode.gCost + HeuristicCost(currentNode, neighbour) + neighbour.weightPenalty;
                //if the new current g score to neighbour is less than the old g score of the neighbour and is not on the open llst
                //we want to evaluate this node
                if(new_g_cost_to_neighbour < neighbour.gCost || !open_set.Contains(neighbour)){
                    //update the costs
                    neighbour.gCost = new_g_cost_to_neighbour;
                    //get heuristic
                    neighbour.hCost = HeuristicCost(neighbour, targetNode);
                    //update parent
                    //The F function is sorted on the node class and the heap class
                    //update the neighbout parent as the current node
                    neighbour.parent = currentNode;
                    // add to the open list
                    if(!open_set.Contains(neighbour)){
                        open_set.Add(neighbour);
                    }else{
                        //we want to update the the neighbour in the Heap so we can sort it regarding the F function cost
                        open_set.UpdateItem(neighbour);
                    }
                }   
            }

        }
		if (pathSuccess) {
            //get the path
            waypoint = Get_Path(startNode,targetNode);
            // OnFound will be used on the player AI to get the path
            OnFound = waypoint;
            //set the pathfound bool as true
            PathFound = true;

		}else{
            PathFound = false;
        }
        //print("State Expansion: " + stateExpansion);

    }


    public void A_Star_WithoutHeap(Vector3 StartNode, Vector3 EndNode)
    {
        
        
        // Stopwatch sw = new Stopwatch();
		// sw.Start();
        //Initialize the waypoint to get the path
        Vector3[] waypoint = new Vector3[0];
        bool pathSuccess =false;
        // keep track of the state expansion
       
       
        //Get the position in grid base
        Node  startNode = grid.NodeFromWorldPoint(StartNode);
        Node targetNode = grid.NodeFromWorldPoint(EndNode);
      
        //Heap for the nodes that we should visit
        List<Node> open_set = new List<Node>();
        //Nodes closed
        HashSet<Node> closed_set = new HashSet<Node>();
        //add start node to the open list
        open_set.Add(startNode);
        //loop until the open heap is 0
        while(open_set.Count >0){
            //get the node with the lower F cost
            Node currentNode = open_set[0];
            for (int i = 1; i < open_set.Count; i ++) {
				if (open_set[i].fCost < currentNode.fCost || open_set[i].fCost == currentNode.fCost) {
					if (open_set[i].hCost < currentNode.hCost)
						currentNode = open_set[i];
				}
			}
            open_set.Remove(currentNode);
			closed_set.Add(currentNode);
            stateExpansion++;
            //if this is true we can stop the while loop
            if(currentNode == targetNode){
                // sw.Stop();
                // print("Path found: " + sw.ElapsedMilliseconds + " ms");
                //Get Path to the end node
                pathSuccess = true;
               
                break;
                //return;
            }

            // loop on each of the neighbors and states of each node
            foreach(Node neighbour in grid.GetNeighbours(currentNode)){
                // if the neighbour is not walkable and is on the close set we want to skip
                if(!neighbour.Walkable || closed_set.Contains(neighbour)){
                    continue;
                }
                //we add the weight penalty of moving to that neighbour and we also get the heuristic to moving to neightbour
                int new_g_cost_to_neighbour = currentNode.gCost + HeuristicCost(currentNode, neighbour) + neighbour.weightPenalty;
                //if the new current g score to neighbour is less than the old g score of the neighbour and is not on the open llst
                //we want to evaluate this node
                if(new_g_cost_to_neighbour < neighbour.gCost || !open_set.Contains(neighbour)){
                    //update the costs
                    neighbour.gCost = new_g_cost_to_neighbour;
                    //get heuristic
                    neighbour.hCost = HeuristicCost(neighbour, targetNode);
                    //update parent
                    //The F function is sorted on the node class and the heap class
                    //update the neighbout parent as the current node
                    neighbour.parent = currentNode;
                    // add to the open list
                    if(!open_set.Contains(neighbour)){
                        open_set.Add(neighbour);
                    }
                }   
            }

        }
		if (pathSuccess) {
            //get the path
            waypoint = Get_Path(startNode,targetNode);
            // OnFound will be used on the player AI to get the path
            OnFound = waypoint;
            //set the pathfound bool as true
            PathFound = true;

		}else{
            PathFound = false;
        }
       

    }


    //Manhattan distance A star


    
    //Manhataan to get the distance of two nodes
    //first we get the difference of moving to a node in the x and y component
    // then we get the ramaing node. This means how many horizontal moves we need
    // we add this to the minimum value of the distance x or y. Since the minimum value means how many diagonal moves we need to move. In order to be on the same line of the node b
    private int HeuristicCost(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
		int dstY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        
        int remaining = Mathf.Abs(dstX-dstY);

		
		return 14 * Mathf.Min(dstX,dstY) + 10 * remaining;
    }



// Heuristic that will detect if the proximity with a enemy or their line of sight and like that add the weights
// With this we want to make A star to get further away, However this approach was not properly working.
     private int HeuristicCostWithEnemyDetection(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
		int dstY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        
        int remaining = Mathf.Abs(dstX-dstY);


		Node newDetectionNode = playerAI.IsNodeWithinEnemyOrLineOfSight();
        if(newDetectionNode !=null){
		    return (14 * Mathf.Min(dstX,dstY) + 10 * remaining) + newDetectionNode.weightPenalty;
        }else{
		    return 14 * Mathf.Min(dstX,dstY) + 10 * remaining;
        }
    }

   

   
    //calculate the path
    private Vector3[] Get_Path(Node startNode, Node targetNode)
    {
        //Save it in a list as a node
        // And save it in a list of waypoint as world position
        List<Node> path = new List<Node>();
		
		
        Node currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		
        Vector3[] waypoints = GetWayPointPath(path);
        Array.Reverse(waypoints);
        //grid.path = path;
        return waypoints;
	
    }


    // this will return the path in world position for the player AI
    Vector3[] GetWayPointPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
				waypoints.Add(path[i].WorldPosition);
		}
		return waypoints.ToArray();
	}


    public int GetStatsState(){
        //sw.Stop();
        return stateExpansion;
    }


  

   
}