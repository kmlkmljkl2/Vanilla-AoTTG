using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharacterBase : Photon.MonoBehaviour
{
    public Animation baseA;
    public GameObject baseG;
    public Transform baseGT;
    public Rigidbody baseR;
    public Transform baseT;

    public bool IsLocal { get; private set; }

    protected virtual void Cache()
    {
        baseA = base.animation;
        baseG = base.gameObject;
        baseGT = baseG.transform;
        baseR = base.rigidbody;
        baseT = base.transform;
        IsLocal = IN_GAME_MAIN_CAMERA.GameType == GameType.Single || photonView.IsMine;
    }
}

