using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAI : MonoBehaviour
{
    public LineRenderer Line;
	public float speed = 20;
    public Vector3[] path;
	private int targetIndex;
	private Pathfinding pathfinding;
	public float checkPathExistTime;
	private float remainingTime;
	public Transform Player, Target;

	private Vector3 InitialPos; 



	
	public void Start()
	{
		//find the gameobjact A star to get access to the pathfinding script
		pathfinding = GameObject.FindGameObjectWithTag("A_Star").GetComponent<Pathfinding>();
		//start a couroutine for A start
		StartCoroutine("AstarCourotine",.1f);
		//Set the timer
		remainingTime = checkPathExistTime;
		InitialPos = Player.position;
		
	}



	IEnumerator AstarCourotine(float delay){
        while(true){
            yield return new WaitForSeconds(delay);
            //call the funciton to start finding the path
			pathfinding.StartFindPath(Player, Target);
			//if path is found we want to do the following
			if(pathfinding.PathFound){
				
				path =pathfinding.OnFound;
				StopCoroutine("FollowPath");
				StartCoroutine("FollowPath");
			}else{
				path = new Vector3[0];
			}
			
        }
    }

	IEnumerator FollowPath() {
		
		 while (targetIndex < path.Length) {
            Vector3 targetPosition = path[targetIndex];
            while (transform.position != targetPosition) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }
            targetIndex++;
        }
		
	}



	private void Update() {
		//remainingTime -= Time.deltaTime;
		RenderLinePath();
		
		float dist = Vector3.Distance(Player.position,Target.position);
		//Debug.Log("This is the dist" + dist);
		if(dist <=4){
			//We want to change the target position to the initial position So A star can get back
			Target.position = InitialPos;
		}

		
	}


	//Render a line for the path so we can visualise the path
	private void RenderLinePath(){
		//renderit only if the path is not null and if the lenght is greater than 0
		if (path != null && path.Length > 0) {
			//Enable the line renderer
			Line.enabled = true;
			//set the lenght of the line renderer
			Line.positionCount = path.Length;
			//Line renderer set the position on the path
			Line.SetPositions(path);
			//this is to make sure that the line is drawed from the player position.
			//the target Index will change when the player is moving
			//is  plus one so the line can be set the final positon on the array
			// Vector3[] positions = new Vector3[targetIndex + 1];
			// //start  from the player position
			// positions[0] = transform.position;
			
			// for (int i = 1; i <= targetIndex; i++) {
			// 	positions[i] = path[i];
			// }
			// Line.SetPositions(positions);


		//we want to disable the line renderer if there is not path	
		} else {
			Line.enabled = false;
		}
	}

    
	//Uncomment if you want to see the path on gizmos 
	public void OnDrawGizmosS() {
			if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				// Gizmos.color = Color.black;
				// Gizmos.DrawCube(path[i], Vector3.one);

			}
		}
	}

	


}
