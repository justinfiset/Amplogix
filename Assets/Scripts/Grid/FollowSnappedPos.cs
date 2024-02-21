using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSnappedPos : MonoBehaviour
{
    private void Update()
    {
        transform.position = GridSettings.GetCurrentSnapedPosition();
    }
}   
