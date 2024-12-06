using UnityEngine;

public class TriggerPPS : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator MyAnimationController;

    // [SerializeField] private bool afterlife_activated_00 = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                MyAnimationController.SetBool("afterlife_activated_01", true);
            }
        }
    }

    }
