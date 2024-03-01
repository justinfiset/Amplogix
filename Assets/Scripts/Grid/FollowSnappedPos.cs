using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSnappedPos : MonoBehaviour
{
    public bool snapToGrid = true;

    private void Update()
    {
        transform.position = GetMousePos();
    }

    private Vector3 GetMousePos()
    {
        return snapToGrid ? GridSettings.GetCurrentSnapedPosition()
            : GridSettings.MouseInputToWorldPoint();
    }
}   
