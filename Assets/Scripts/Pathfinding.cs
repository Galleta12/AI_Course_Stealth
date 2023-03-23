using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {

	
   
    
    Grid grid;

    

    public Vector3[] OnFound;

    public bool PathFound;

    


    private void Awake() {
      
        grid = GetComponent<Grid>();

    }


    private void Update() {
           
       
        
       
    }


    //use courotines to call A star on a set of intervals
    //this is called on the Player Ai Script.
    public void StartFindPath(Transform start, Transform target)
    {
        //StartCoroutine(A_Star(startNode,targetNode));
        A_Star(start.position,target.position);

    }

    public void A_Star(Vector3 StartNode, Vector3 EndNode)
    {
        
        
        Stopwatch sw = new Stopwatch();
		sw.Start();
        //Initialize the waypoint to get the path
        Vector3[] waypoint = new Vector3[0];
        bool pathSuccess =false;
        
      
       
            //Get the position in grid base
        Node  startNode = grid.NodeFromWorldPoint(StartNode);
        Node targetNode = grid.NodeFromWorldPoint(EndNode);
      
        //Open set for the nodes that we should visit
        Heap<Node> open_set = new Heap<Node>(grid.MaxSize);
        //Nodes closed
        HashSet<Node> closed_set = new HashSet<Node>();
        //add start node to the open list
        open_set.Add(startNode);
        //loop until the open list is 0
        while(open_set.Count >0){
            //get the node with the lower F cost
            Node currentNode = open_set.RemoveFirst();
            
         
            
            //add it to the closed list
            closed_set.Add(currentNode);

            if(currentNode == targetNode){
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
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
                //we add the move penalty of moving to that neighbour
                int new_g_cost_to_neighbour = currentNode.gCost + HeuristicOne(currentNode, neighbour) + neighbour.movementPenalty;
                //if the new current g score to neighbour is less than the old g score of the neighbour and is not on the open llst
                //we want to evaluate this node
                if(new_g_cost_to_neighbour < neighbour.gCost || !open_set.Contains(neighbour)){
                    //update the costs
                    neighbour.gCost = new_g_cost_to_neighbour;
                    //get heuristic
                    neighbour.hCost = HeuristicOne(neighbour, targetNode);
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

    }


    //get the distance from one node to the other one

    //Each state in the x and y it will cost 10
    //The diagonals will cost 14
    private int HeuristicOne(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
		int dstY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
    }

    //Heuristic if a node is closer to a red node. We want to increment +10

    //Herusitic if the player not moves for 5 seconds + 5 

   
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
		
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        //grid.path = path;
        return waypoints;
	
    }



    Vector3[] SimplifyPath(List<Node> path) {
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) {
			//Vector2 directionNew = new Vector2(path[i-1].gridPosX - path[i].gridPosX,path[i-1].gridPosY - path[i].gridPosY);
			//if (directionNew != directionOld) {
				waypoints.Add(path[i].WorldPosition);
			//}
			//directionOld = directionNew;
		}
		return waypoints.ToArray();
	}


  

   
}