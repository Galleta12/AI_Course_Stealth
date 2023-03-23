using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineSightAndCollider : MonoBehaviour
{
   
    public LineRenderer LineSight;

    public Transform LineLeft;

    public Transform LineRight;




    private void Start() {
        LineSight.positionCount = 4;
    }


    private void Update() {
        LineSight.SetPosition(0,transform.parent.position);
        LineSight.SetPosition(1,LineLeft.position);
        LineSight.SetPosition(2,LineRight.position);
         LineSight.SetPosition(3,transform.parent.position);

    }
    
    
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LineLeft.position,.2f);
        Gizmos.DrawWireSphere(LineRight.position,.2f);
        //Gizmos.DrawWireSphere(LineLeft.position,2f);

    }


}
