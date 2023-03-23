using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderBehind : MonoBehaviour
{
   
    public LineRenderer LineBoxBehind;

    public Transform BoxCornerLeftUPBehind;

    public Transform BoxCornerRightUPBehind;

    public Transform BoxCornerBottomRightBehind;

    public Transform BoxCornerBottomLeftBehind;




    private void Start() {
        LineBoxBehind.positionCount = 5;
    }


    private void Update() {
        LineBoxBehind.SetPosition(0,BoxCornerBottomLeftBehind.position);
        LineBoxBehind.SetPosition(1,BoxCornerLeftUPBehind.position);
        LineBoxBehind.SetPosition(2,BoxCornerRightUPBehind.position);
        LineBoxBehind.SetPosition(3,BoxCornerBottomRightBehind.position);
        LineBoxBehind.SetPosition(4,BoxCornerBottomLeftBehind.position);


    }
    
    
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(BoxCornerBottomLeftBehind.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerLeftUPBehind.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerRightUPBehind.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerBottomRightBehind.position,.2f);
        Gizmos.DrawWireSphere(BoxCornerBottomLeftBehind.position,.2f);
        

        //Gizmos.DrawWireSphere(LineLeft.position,2f);

    }

   



   

  





    
}
