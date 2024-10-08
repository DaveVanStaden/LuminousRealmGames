using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector2 GetMoveInput()
    {
        // Get input from both keyboard and joystick
        float horizontal = Input.GetAxis("Horizontal");  // "Horizontal" is for keyboard and joystick
        float vertical = Input.GetAxis("Vertical");      // "Vertical" is for keyboard and joystick
        return new Vector2(horizontal, vertical);
    }

    public bool GetJumpInput()
    {
        // Return true if the jump button is held down
        return Input.GetButton("Jump"); // Works for keyboard (space) and controller (button A by default)
    }

    public bool GetJumpInputDown()
    {
        // Return true if the jump button is pressed this frame
        return Input.GetButtonDown("Jump"); // Works for both keyboard and controller
    }

    public bool GetSprintInput()
    {
        // Return true if the sprint button is held down
        return Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint"); // Handle both keyboard and controller sprinting
    }
}
