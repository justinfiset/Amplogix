using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentParticle : MonoBehaviour
{

    private Vector2 targetPosition;
    public Color negativeColor;
    public Color positiveColor;
    private Connection.Position direction;
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

    public void Create(bool realCurrent, Vector2 targetPosition, Connection.Position direction)
    {
        this.direction = direction;
        this.targetPosition = targetPosition;
        targetDistance = CalculateExpectedDistance();

        WaitToSetColor(realCurrent);
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
        if (direction == Connection.Position.Left || direction == Connection.Position.Right)
        {
            return Math.Abs(targetPosition.x - transform.position.x);
        }
        if (direction == Connection.Position.Top || direction == Connection.Position.Bottom)
        {
            return Mathf.Abs(targetPosition.y - transform.position.y);
        }

        throw new Exception("Direction of particle needs to be set");
    }

    public float GetExpectedTravelTime()
    {
        if (direction == Connection.Position.Left || direction == Connection.Position.Right)
        {
            return Math.Abs(targetDistance / targetSpeed);
        }
        if (direction == Connection.Position.Top || direction == Connection.Position.Bottom)
        {
            return Mathf.Abs(targetDistance / targetSpeed);
        }

        throw new Exception("Direction of particle needs to be set");
    }

    private IEnumerator WaitToSetColor(bool realCurrent)
    {
        yield return new WaitForEndOfFrame();

        SetDirectionColor(realCurrent);
    }
}
