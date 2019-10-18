using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HumanAgent : IAgent
{
    private IAgent _agentImplementation;
    private int action = ButtonScriptCopy.action;
    public int Act(ref SpaceInvadersGameState gs, NativeArray<int> availableActions, int plyId)
    {
        
        if (Input.GetKey(KeyCode.Space)&&plyId==1)
        {
            return 3;
        }
        
        if (Input.GetKey(KeyCode.Z)&&plyId==1)
        {
            return 1;
        }
        
        if (Input.GetKey(KeyCode.S)&&plyId==1)
        {
            return 2;
        }
        if (Input.GetKey(KeyCode.UpArrow)&& gs.sndPlayer&&plyId==2)
        {
            return 5;
        }
        
        if (Input.GetKey(KeyCode.DownArrow)&& gs.sndPlayer&&plyId==2)
        {
            return 6;
        }
        
        if (Input.GetKey(KeyCode.KeypadEnter)&& gs.sndPlayer&&plyId==2)
        {
            return 7;
        }

        if (action != 0)
            return action;

        return 0;


    }
}