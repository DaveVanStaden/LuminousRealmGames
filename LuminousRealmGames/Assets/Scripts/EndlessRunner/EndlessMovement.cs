using UnityEngine;

public class EndlessMovement : MonoBehaviour
{
    [SerializeField]private float speed = 7f;
    [SerializeField]private float moveSpeed = 10f;  
    [SerializeField]private float xMin = -5f;      
    [SerializeField]private float xMax = 5f;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        
        float horizontalInput = Input.GetAxis("Horizontal");
        
        float newXPosition = transform.position.x - horizontalInput * moveSpeed * Time.deltaTime;
        
        newXPosition = Mathf.Clamp(newXPosition, xMin, xMax);
        
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
    }
}
