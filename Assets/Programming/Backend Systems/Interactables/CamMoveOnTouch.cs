using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMoveOnTouch : Interactable
{
    public Vector3 newCamPosition = new Vector3();
    public bool global = false;

    public override void OnContact(GameObject player)
    {
        Debug.Log("Entering area " + gameObject.name);
        if (global)
        {
            FollowOffset.main.SetAndLockPosition(newCamPosition);
        }
        else
        {
            FollowOffset.main.SetLocalPosition(newCamPosition);
        }
    }

    public override void OnExit(GameObject player)
    {
        FollowOffset.main.ResetPosition();
    }

    public override void OnHold(GameObject player)
    {
        if (FollowOffset.main.currentRequest.to != newCamPosition)
        {
            OnContact(player);
        }
    }
}
