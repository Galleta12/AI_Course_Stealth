using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderUP : MonoBehaviour
{
   public LineRenderer LineBoxUP;

    public Transform BoxCornerLeftUP;

    public Transform BoxCornerRightUP;

    public Transform BoxCornerBottomRight;

    public Transform BoxCornerBottomLeft;




    private void Start() {
        LineBoxUP.positionCount = 5;
    }


    private void Update() {
        LineBoxUP.SetPosition(0,BoxCornerBottomLeft.position);
        LineBoxUP.SetPosition(1,BoxCornerLeftUP.position);
        LineBoxUP.SetPosition(2,BoxCornerRightUP.position);
        LineBoxUP.SetPosition(3,BoxCornerBottomRight.position);
        LineBoxUP.SetPosition(4,BoxCornerBottomLeft.position);


    }
    
    
    
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(BoxCornerBottomLeft.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerLeftUP.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerRightUP.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerBottomRight.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerBottomLeft.position,.2f);
        

        //Gizmos.DrawWireSphere(LineLeft.position,2f);

    }
}
