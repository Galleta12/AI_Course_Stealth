using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //This is the Script for the enemy waypoint system
    public Transform pathwaypointsEnemy;
    public float speed =5;
    public float waitTime = .5f;

    public float turnSpeed = 90;


 

   

  
    
    private void Start() {
        
        //save childs => waypoints that enemy will follow
        Vector3[] waypoints = new Vector3[pathwaypointsEnemy.childCount];
        for(int i =0; i <waypoints.Length; i++){
            waypoints[i] = pathwaypointsEnemy.GetChild(i).position;
            //assing same height so the enemy stay on the ground
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);

        }
        StartCoroutine(FollowPathEnemy(waypoints));
    
    }



    private void Update() {
        
    }

    //this handle the enemy waypoint movement and repeated  
    IEnumerator FollowPathEnemy(Vector3[] waypoints){
        transform.position = waypoints[0];
        int nextTargetIndex = 1;
        Vector3 targetWaypoint = waypoints[nextTargetIndex];
        //start facing first waypoint
        transform.LookAt(targetWaypoint);
        while(true){
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if(transform.position == targetWaypoint){
                //Modulus operator if the index is the same to lenght, it will go back to 0
                nextTargetIndex = (nextTargetIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[nextTargetIndex];
                //pause on the current waypoint. This is set on the inspector how long the enemy should wait
                yield return new WaitForSeconds(waitTime);
                //wait 
                yield return StartCoroutine(TurnToWaypoint(targetWaypoint));
            }
            //wait a frame after each etaration of the loop
            yield return null;
        }
    }

    IEnumerator TurnToWaypoint(Vector3 lookWaypoint){
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
            yield return null;
        }
    }
    
    
    
    //This is for the unity editor. It will draw the waypoints of the enemy
    private void OnDrawGizmos() {
        
        
        Gizmos.color = Color.magenta;
        Vector3 startPosition = pathwaypointsEnemy.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        
        foreach(Transform waypoint in pathwaypointsEnemy){
            Gizmos.DrawSphere(waypoint.position,.3f);
            
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;    

        }
        Gizmos.DrawLine(previousPosition, startPosition);
    }



    


}
