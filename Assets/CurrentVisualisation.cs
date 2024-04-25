using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ElectricComponent))]
public class CurrentVisualisation : MonoBehaviour
{
    private bool isEmitting = false;
    private bool realCurrent = false;
    private GameObject originalParent;
    private GameObject ballParent;

    public GameObject particlePrefab;

    private void Start()
    {
        originalParent = new GameObject("ball parent");

        
        ballParent = Instantiate(originalParent, transform);

    }

    public void UpdateCurrent(float current)
    {
        ElectricComponent electricComponent = GetComponent<ElectricComponent>();

        Connection connection = GetComponent<Connection>();
    }

    #region Starting and killing emission
    public void StartParticleEmission(Vector2 targetPosition)
    {
        isEmitting = true;

        StartCoroutine(BallShootingCoroutine(targetPosition));
    }

    public void KillParticleEmission()
    {
        print("killing particle emission for " + gameObject);
        isEmitting= false;
        StopAllCoroutines();
        KillExistingParticles();
    }
    #endregion

    #region Pausing and resuming animation
    public void PauseParticles()
    {
        PauseParticleEmission();
        PauseParticleMovements();
    }

    private void PauseParticleEmission()
    {
        isEmitting = false;
    }

    private void PauseParticleMovements()
    {
        CurrentParticle particle;
        for (int i = 0; i < ballParent.transform.childCount; i++)
        {
            particle = ballParent.transform.GetChild(i).GetComponent<CurrentParticle>();
            particle.StopMovement();
        }
    }

    public void ResumeParticleMovements()
    {
        isEmitting = true;
        CurrentParticle particle;
        for (int i = 0; i < ballParent.transform.childCount; i++)
        {
            particle = ballParent.transform.GetChild(i).GetComponent<CurrentParticle>();
            particle.ResumeMovement();
        }
    }
    #endregion

    public void SetIsRealCurrent(bool realCurrent)
    {
        this.realCurrent = realCurrent;
        KillExistingParticles();
    }

    private IEnumerator BallShootingCoroutine(Vector2 targetPosition)
    {
        yield return new WaitForSeconds(0.5f);

        if (isEmitting)
        {
            ShootBall(targetPosition);
            StartCoroutine(BallShootingCoroutine(targetPosition));
        }
    }

    private void ShootBall(Vector2 targetPosition)
    {
        CurrentParticle currentParticle = Instantiate(particlePrefab, ballParent.transform).GetComponent<CurrentParticle>();

        currentParticle.Create(realCurrent, targetPosition);
    }

    private void KillExistingParticles()
    {
        if (ballParent != null)
        {
            Destroy(ballParent);
            ballParent = Instantiate(originalParent, transform);
        }
    }


}
