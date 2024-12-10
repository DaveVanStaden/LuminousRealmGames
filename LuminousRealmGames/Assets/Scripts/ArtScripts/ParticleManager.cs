using UnityEngine;
using MalbersAnimations;
using MalbersAnimations.Controller;
using System.Collections;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private MAnimal malbers;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem DustParticle;
    private int jumpValue;
    private bool dustActive;
    
    public void PlayJumpParticle()
    {
        if (jumpValue < 2)
        {
            jumpValue++;
            jumpParticle.Play();
        }
        if(malbers.Grounded == true)
        {
            jumpValue = 0;
        }
        
    }

    public void PlayDustParticle()
    {
        if(malbers.Grounded == true && dustActive == false)
        {
            dustActive = true;
            StartCoroutine(PlayDustParticleCD());
        }
    }

    IEnumerator PlayDustParticleCD()
    {
        yield return new WaitForSeconds(0.1f);
        DustParticle.Play();
        dustActive = false;
    }
}
