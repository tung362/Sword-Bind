using UnityEngine;
using System.Collections;

public class Comments : MonoBehaviour
{
    /*Dash Fail safe*/
    //how far the player is from origin vs modified limit distance
    //if (distance >= DistanceToWall)
    //{
    //    TheRigidbody.velocity = Vector3.zero;
    //    if (WallPosition != Vector3.zero) transform.position = WallPosition;
    //    print(distance + " VS " + DistanceToWall);
    //}

    //float RayCastWallCheck(Vector3 Start, Vector3 End, float Length)
    //{
    //    ThePlayerController.WallPosition = Vector3.zero;
    //    RaycastHit[] hits = Physics.RaycastAll(Start, End, Length);

    //    //If no collisions then just assume falling
    //    if (hits.Length == 0) return Length;

    //    float ClosestdistanceToWall = Length + Mathf.Epsilon;
    //    for (int i = 0; i < hits.Length; ++i)
    //    {
    //        if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //        {
    //            float distance = Vector3.Distance(transform.position, hits[i].point);
    //            if (ClosestdistanceToWall > distance)
    //            {
    //                ThePlayerController.WallPosition = hits[i].point;
    //                ClosestdistanceToWall = distance;
    //            }
    //        }
    //    }
    //    return ClosestdistanceToWall;
    //}
}
