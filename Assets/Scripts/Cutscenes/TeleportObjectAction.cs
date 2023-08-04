using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObjectAction : CutsceneAction
{
    [SerializeField] GameObject gameObject;
    [SerializeField] Vector2 position;

    public override IEnumerator Play()
    {
        gameObject.transform.position = position;
        yield break; 
    }
}
