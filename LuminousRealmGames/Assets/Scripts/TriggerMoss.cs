using UnityEngine;

public class TriggerMoss : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator MyAnimationController;

    // [SerializeField] private bool moss_appears = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                MyAnimationController.SetBool("start_moss", true);
            }
        }
    }

    }
