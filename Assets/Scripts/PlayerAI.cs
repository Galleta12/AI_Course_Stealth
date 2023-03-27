using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;

public class PlayerAI : MonoBehaviour
{
    
	public GameObject startCastPos;
	
	private Grid grid;
	//Line to draw the path for the player
	public LineRenderer Line;
	//speef for the player
	public float speed = 20;

	public float turnSpeed = 120;
    //the path
	public Vector3[] path;
	//FOr moving the player along the waypoint 
	private int targetIndex;
	//reference to the pathfinding
	private Pathfinding pathfinding;
	
	public Transform Player, Target;

	//save the initial pos
	//Hence once the player reach the target node we can return to the initial pos
	private Vector3 InitialPos; 

	//to check the distance to the enemy
	public float enemyProximityDistance;

	public float sphereRadiusCast;

	public bool enemyFound;

	public LayerMask layerCast;

	private bool gameComplete = false;

	private bool targetFound = false; 
	Stopwatch sw = new Stopwatch();
	public void Start()
	{
		//find the gameobjact A star to get access to the pathfinding script
		pathfinding = GameObject.FindGameObjectWithTag("A_Star").GetComponent<Pathfinding>();
		grid = GameObject.FindGameObjectWithTag("A_Star").GetComponent<Grid>();
		//start a couroutine for A start
		StartCoroutine("AstarCourotine",.1f);
		//Set the timer
		sw.Start();
		InitialPos = Player.position;

		
	}



	IEnumerator AstarCourotine(float delay){
        while(true){
            yield return new WaitForSeconds(delay);
            //call the funciton to start finding the path
			pathfinding.StartFindPath(Player, Target);
			//if path is found we want to do the following
			if(pathfinding.PathFound){
				//set the path
				path =pathfinding.OnFound;
				//stop curretn courotine
				StopCoroutine("FollowPath");
				//and restart it
				StartCoroutine("FollowPath");
			}else{
				//path doesnt exist
				path = new Vector3[0];
			}
			
        }
    }


	//this will make the player to follow the path
	//using the target indix and updating it once the player position is the same as the target position
	//target position is the next move to where the player AI need to move
	IEnumerator FollowPath() {
		
		 targetIndex =0;
		 while (targetIndex < path.Length) {
            Vector3 targetPosition = path[targetIndex];
			TurnToWaypoint(targetPosition);

            while (transform.position != targetPosition) {
                transform.position = Vector3.MoveTowards(transform.position, 
				new Vector3(targetPosition.x, transform.position.y, targetPosition.z), speed * Time.deltaTime);
                yield return null;
            }
            targetIndex++;
        }
		
	}

	  private void TurnToWaypoint(Vector3 lookWaypoint){
        Vector3 dir = (lookWaypoint - transform.position).normalized;
        //target angle
        float targetAngle = 90-Mathf.Atan2(dir.z,dir.x) * Mathf.Rad2Deg;

        //stop when is facing waypoint target
        //use math function to tell how apart is from the target angle
        // small threshold
        // absolute value to deal with anti clockwise 
        while(Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle))>0.05f){
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y,targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            
        }
    }
    




	//Render the line
	private void Update() {
		
		//IsNodeWithinEnemyLineOfSight();
		//remainingTime -= Time.deltaTime;
		RenderLinePath();
		//check the distance with the target position to check if the player has reach the target
		float dist = Vector3.Distance(Player.position,Target.position);
		//Debug.Log("This is the dist" + dist);
		if(dist <=3 && !targetFound){
			//stop the timer since we found it
			targetFound = true;
			print("Target found");
			Target.position = InitialPos;
			
		}
		else if(targetFound){
			//print("Whaaaa");
			if(dist <=3 && !gameComplete){
				sw.Stop();
				//get the elapsed time in seconds
				double elapsedSeconds = (double)sw.ElapsedTicks / Stopwatch.Frequency;
				print("Path found: " + elapsedSeconds.ToString("F4") + " seconds");
				print("State expansion: " + pathfinding.GetStatsState());
				print("State expansion per second: " + pathfinding.GetStatsState()/elapsedSeconds);

				gameComplete = true;
				
			}
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
		//we want to disable the line renderer if there is not path	
		} else {
			Line.enabled = false;
		}
	}

	public Node IsNodeWithinEnemyOrLineOfSight()
	{
		RaycastHit hit;
		
		if (Physics.SphereCast(startCastPos.transform.position, sphereRadiusCast, startCastPos.transform.forward * enemyProximityDistance, out hit, layerCast))
		{
			// if (hit.collider.gameObject == node.gameObject)
			// {
				//get the postion if nodes
				Node node = grid.NodeFromWorldPoint(hit.collider.transform.position);
				if(node !=null){

					//Debug.Log(node.weightPenalty);
					enemyFound = true;
					return node;
				}
			//}
		}

		enemyFound = false;
		return null;
	}

   
    
	//Uncomment if you want to see the path on gizmos 
	public void OnDrawGizmos() {
			
			
			Gizmos.color = Color.green;
    		Gizmos.DrawLine(startCastPos.transform.position, startCastPos.transform.position+ startCastPos.transform.forward * enemyProximityDistance);
    		Gizmos.color = enemyFound?Color.red:Color.black;
			Gizmos.DrawWireSphere(startCastPos.transform.position + startCastPos.transform.forward * enemyProximityDistance, sphereRadiusCast);
			
			if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				// Gizmos.color = Color.black;
				// Gizmos.DrawCube(path[i], Vector3.one);

			}
		}




	}

	


}
