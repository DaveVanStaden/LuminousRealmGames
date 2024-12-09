using UnityEngine;

public class TriggerPlants : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Animator MyAnimationController;

    // [SerializeField] private bool plants_appears = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            {
                MyAnimationController.SetBool("start_plants_01", true);
                MyAnimationController.SetBool("plants_grow_01", true);
            }
        }
    }

    }
