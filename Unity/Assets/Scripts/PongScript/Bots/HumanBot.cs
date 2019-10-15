using UnityEngine;

public class HumanBot : IBot
{
    public int Act(ref GameStateScr gs, int[] usableActions)
    {
        if (Input.GetKey(KeyCode.Space)) // SHOOT
        {
            return 3;
        }
        
        if (Input.GetKey(KeyCode.Z)) // UP
        {
            return 1;
        }
        
        if (Input.GetKey(KeyCode.S)) // DOWN
        {
            return 2;
        }
        // IDLE
        return 0;
    }
}
