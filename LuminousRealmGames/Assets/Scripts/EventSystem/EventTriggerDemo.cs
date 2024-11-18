using UnityEngine;

public class EventTriggerDemo : MonoBehaviour
{
    [SerializeField] private GameEvent someEvent;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            someEvent.Trigger();
        }
    }
}
