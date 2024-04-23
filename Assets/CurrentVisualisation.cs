using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(ElectricComponent))]
public class CurrentVisualisation : MonoBehaviour
{
    private bool isEmitting = false;
    private bool realCurrent;
    private GameObject ballParent;
    private Vector2 targetPosition;

    public GameObject particlePrefab;

    public void SetupTarget(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public void UpdateCurrent(float current)
    {
        ElectricComponent electricComponent = GetComponent<ElectricComponent>();

        Connection connection = GetComponent<Connection>();
    }

    #region Starting and killing emission
    public void StartParticleEmission()
    {
        isEmitting = true;

        StartCoroutine(BallShootingCoroutine());
    }

    public void KillParticleEmission()
    {
        isEmitting= false;
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

    private void ResumeParticleMovements()
    {
        CurrentParticle particle;
        for (int i = 0; i < ballParent.transform.childCount; i++)
        {
            particle = ballParent.transform.GetChild(i).GetComponent<CurrentParticle>();
            particle.StartMovement();
        }
    }
    #endregion

    public void SetIsRealCurrent(bool realCurrent)
    {
        this.realCurrent = realCurrent;
        KillExistingParticles();
    }

    private IEnumerator BallShootingCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        ShootBall();

        if (isEmitting)
        {
            StartCoroutine(BallShootingCoroutine());
        }
    }

    private void ShootBall()
    {
        CurrentParticle currentParticle = Instantiate(particlePrefab, ballParent.transform)
            .GetComponent<CurrentParticle>();

        currentParticle.Create(realCurrent, targetPosition);
    }

    private void KillExistingParticles()
    {
        if (ballParent != null)
        {
            Destroy(ballParent);
        }
    }


}
