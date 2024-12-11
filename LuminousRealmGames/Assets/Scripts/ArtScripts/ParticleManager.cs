using UnityEngine;
using MalbersAnimations;

public class ParticleManager : MonoBehaviour
{
    //[SerializeField] private  malbers;
    [SerializeField] private ParticleSystem jumpParticle;
    [SerializeField] private ParticleSystem DustParticle;
    private int jumpValue;
    
    public void PlayJumpParticle()
    {
        if (jumpValue < 2)
        {
            jumpValue++;
            jumpParticle.Play();
        }
        
    }
}
