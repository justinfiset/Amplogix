using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentParticle : MonoBehaviour
{

    private Vector2 targetPosition;
    public Color negativeColor;
    public Color positiveColor;
    private float targetDistance;

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
        LeanTween.move(gameObject, targetPosition, GetExpectedTravelTime()).setDestroyOnComplete(true);
    }

    public void StopMovement()
    {
        LeanTween.cancel(gameObject);
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
        yield return new WaitForEndOfFrame();

        SetDirectionColor(realCurrent);
    }
}
