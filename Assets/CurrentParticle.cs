using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CurrentParticle : MonoBehaviour
{

    private Vector2 targetPosition;
    public Color negativeColor;
    public Color positiveColor;
    private float targetDistance;
    private int tweenID;

    public float targetSpeed = 1;


    // Update is called once per frame
    void Update()
    {

    }

    /*
    private float GetTraveledDistance()
    {
        if (direction == Connection.Position.Left || direction == Connection.Position.Right)
        {
            return Math.Abs(startingPosition.x - transform.position.x);
        }
        if (direction == Connection.Position.Top || direction == Connection.Position.Bottom)
        {
            return Mathf.Abs(startingPosition.y - transform.position.y);
        }

        throw new Exception("Direction of particle needs to be set");
    }
    */

    private void SetDirectionColor(bool realCurrent)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (realCurrent)
        {
            spriteRenderer.color = negativeColor;
        } else
        {
            spriteRenderer.color = positiveColor;
        }
    }

    public void Create(bool realCurrent, Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        targetDistance = CalculateExpectedDistance();

        StartSetColorCoroutine(realCurrent);
        StartMovement();
    }

    public void StartMovement()
    {
        tweenID = LeanTween.move(gameObject, targetPosition, GetExpectedTravelTime()).setDestroyOnComplete(true).id;
    }

    public void ResumeMovement()
    {
        print("resuming particle movement");
        LeanTween.resume(tweenID);
    }

    public void StopMovement()
    {
        LeanTween.pause(tweenID);
    }

    private float CalculateExpectedDistance()
    {
        double diffX = Math.Abs(targetPosition.x - transform.position.x);
        double diffY = Math.Abs(targetPosition.y - transform.position.y);
        return (float) Math.Sqrt(Math.Pow(diffX, 2d) + Math.Pow(diffY, 2d));
    }


        /*
        if (direction == Connection.Position.Left || direction == Connection.Position.Right)
        {
            return Math.Abs(targetPosition.x - transform.position.x);
        }
        if (direction == Connection.Position.Top || direction == Connection.Position.Bottom)
        {
            return Mathf.Abs(targetPosition.y - transform.position.y);
        }
        */



    public float GetExpectedTravelTime()
    {
        return Mathf.Abs(targetDistance / targetSpeed);
    }

    private IEnumerator StartSetColorCoroutine(bool realCurrent)
    {
        yield return new WaitForNextFrameUnit();

        SetDirectionColor(realCurrent);
    }
}
