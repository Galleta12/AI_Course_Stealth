using UnityEngine;
using System.Collections;


//Class that holds the node object

//Implement the heap class to get acces to its index
public class Node : IHeapItem<Node> {
	
    //check is a node is walkable
    public bool Walkable;
    //position of the node in world space
    public Vector3 WorldPosition;

    //position in grid space
    public int gridPosX;
    public int gridPosY;

    //g cost of the node
    public int gCost;
    // heuristic cost of the node
    public int hCost;
    //reference to its parent
    public Node parent;

    

    public int weightPenalty;

    int heapIndex;

    


    public Node(bool walkable, Vector3 worldPosition, int pos_x, int pos_y, int weight){
        Walkable = walkable;
        WorldPosition = worldPosition;
        gridPosX = pos_x;
        gridPosY = pos_y;
        weightPenalty = weight;

    }


  // get the F cost. The sum of the g cost and the heuristic
    public int fCost {
		get {
			return gCost + hCost;
		}
	}

  	
  //Index of the heap    
    public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}
    
    //Function to compare the f cost and h cost and sort the heap to get the lower f cost
    public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		// if the two f cost are equally we want to compare the h cost
    if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
    //return the lower 
		return -compare;
	}
}

