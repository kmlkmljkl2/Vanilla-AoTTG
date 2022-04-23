using ExitGames.Client.Photon;
using UnityEngine;

public partial class HERO
{
    [RPC]
    private void setMyTeam(int val)
    {
        UnityEngine.MonoBehaviour.print("set team " + val);
        myTeam = val;
        checkBoxLeft.GetComponent<TriggerColliderWeapon>().myTeam = val;
        checkBoxRight.GetComponent<TriggerColliderWeapon>().myTeam = val;
    }

    [RPC]
    private void netGrabbed(int id, bool leftHand)
    {
        titanWhoGrabMeID = id;
        grabbed(PhotonView.Find(id).gameObject, leftHand);
    }

    [RPC]
    private void netSetIsGrabbedFalse()
    {
        state = HERO_STATE.Idle;
    }

    [RPC]
    private void netUngrabbed()
    {
        ungrabbed();
        netPlayAnimation(standAnimation);
        falseAttack();
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        currentAnimation = aniName;
        if (base.animation != null)
        {
            base.animation.Play(aniName);
        }
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        currentAnimation = aniName;
        if (base.animation != null)
        {
            base.animation.Play(aniName);
            base.animation[aniName].normalizedTime = normalizedTime;
        }
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        currentAnimation = aniName;
        if (base.animation != null)
        {
            base.animation.CrossFade(aniName, time);
        }
    }

    [RPC]
    private void backToHumanRPC()
    {
        titanForm = false;
        eren_titan = null;
        base.gameObject.GetComponent<SmoothSyncMovement>().disabled = false;
    }

    [RPC]
    private void whoIsMyErenTitan(int id)
    {
        eren_titan = PhotonView.Find(id).gameObject;
        titanForm = true;
    }

    [RPC]
    private void netPauseAnimation()
    {
        foreach (AnimationState item in base.animation)
        {
            item.speed = 0f;
        }
    }

    [RPC]
    private void netContinueAnimation()
    {
        foreach (AnimationState item in base.animation)
        {
            if (item.speed == 1f)
            {
                return;
            }
            item.speed = 1f;
        }
        playAnimation(currentPlayingClipName());
    }

    [RPC]
    private void RPCHookedByHuman(int hooker, Vector3 hookPosition)
    {
        hookBySomeOne = true;
        badGuy = PhotonView.Find(hooker).gameObject;
        if (Vector3.Distance(hookPosition, base.transform.position) < 15f)
        {
            launchForce = PhotonView.Find(hooker).gameObject.transform.position - base.transform.position;
            base.rigidbody.AddForce(-base.rigidbody.velocity * 0.9f, ForceMode.VelocityChange);
            float num = Mathf.Pow(launchForce.magnitude, 0.1f);
            if (grounded)
            {
                base.rigidbody.AddForce(Vector3.up * Mathf.Min(launchForce.magnitude * 0.2f, 10f), ForceMode.Impulse);
            }
            base.rigidbody.AddForce(launchForce * num * 0.1f, ForceMode.Impulse);
            if (state != HERO_STATE.Grab)
            {
                dashTime = 1f;
                crossFade("dash", 0.05f);
                base.animation["dash"].time = 0.1f;
                state = HERO_STATE.AirDodge;
                falseAttack();
                facingDirection = Mathf.Atan2(launchForce.x, launchForce.z) * 57.29578f;
                Quaternion quaternion = Quaternion.Euler(0f, facingDirection, 0f);
                base.gameObject.transform.rotation = quaternion;
                quaternion = quaternion;
                base.rigidbody.rotation = quaternion;
                targetRotation = quaternion;
            }
        }
        else
        {
            hookBySomeOne = false;
            badGuy = null;
            PhotonView.Find(hooker).RPC("hookFail", PhotonView.Find(hooker).owner);
        }
    }

    [RPC]
    public void hookFail()
    {
        hookTarget = null;
        hookSomeOne = false;
    }

    [RPC]
    public void badGuyReleaseMe()
    {
        hookBySomeOne = false;
        badGuy = null;
    }

    [RPC]
    public void netDie(Vector3 v, bool isBite, int viewID = -1, string titanName = "", bool killByTitan = true)
    {
        if (base.photonView.isMine && titanForm && eren_titan != null)
        {
            eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
        }
        if ((bool)bulletLeft)
        {
            bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if ((bool)bulletRight)
        {
            bulletRight.GetComponent<Bullet>().removeMe();
        }
        meatDie.Play();
        if (!useGun && (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine))
        {
            leftbladetrail.Deactivate();
            rightbladetrail.Deactivate();
            leftbladetrail2.Deactivate();
            rightbladetrail2.Deactivate();
        }
        falseAttack();
        breakApart(v, isBite);
        if (base.photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: false);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().myRespawnTime = 0f;
        }
        hasDied = true;
        Transform transform = base.transform.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (base.photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(base.photonView);
            PhotonNetwork.Player.SetCustomProperties(new Hashtable {
            {
                PhotonPlayerProperty.dead,
                true
            } });
            PhotonNetwork.Player.SetCustomProperties(new Hashtable {
            {
                PhotonPlayerProperty.deaths,
                (int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.deaths] + 1
            } });
            GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, (!(titanName == string.Empty)) ? 1 : 0);
            if (viewID != -1)
            {
                PhotonView photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(killByTitan, (string)photonView.owner.CustomProperties[PhotonPlayerProperty.name], t2: false, (string)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
                    photonView.owner.SetCustomProperties(new Hashtable {
                    {
                        PhotonPlayerProperty.kills,
                        (int)photonView.owner.CustomProperties[PhotonPlayerProperty.kills] + 1
                    } });
                }
            }
            else
            {
                GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo((!(titanName == string.Empty)) ? true : false, titanName, t2: false, (string)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
            }
        }
        if (base.photonView.isMine)
        {
            PhotonNetwork.Destroy(base.photonView);
        }
    }

    [RPC]
    private void netDie2(int viewID = -1, string titanName = "")
    {
        if (base.photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(base.photonView);
            if (titanForm && eren_titan != null)
            {
                eren_titan.GetComponent<TITAN_EREN>().lifeTime = 0.1f;
            }
        }
        meatDie.Play();
        if ((bool)bulletLeft)
        {
            bulletLeft.GetComponent<Bullet>().removeMe();
        }
        if ((bool)bulletRight)
        {
            bulletRight.GetComponent<Bullet>().removeMe();
        }
        Transform transform = base.transform.Find("audio_die");
        transform.parent = null;
        transform.GetComponent<AudioSource>().Play();
        if (base.photonView.isMine)
        {
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: true);
            currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
            GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().myRespawnTime = 0f;
        }
        falseAttack();
        hasDied = true;
        base.gameObject.GetComponent<SmoothSyncMovement>().disabled = true;
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.MULTIPLAYER && base.photonView.isMine)
        {
            PhotonNetwork.RemoveRPCs(base.photonView);
            PhotonNetwork.Player.SetCustomProperties(new Hashtable {
            {
                PhotonPlayerProperty.dead,
                true
            } });
            PhotonNetwork.Player.SetCustomProperties(new Hashtable {
            {
                PhotonPlayerProperty.deaths,
                (int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.deaths] + 1
            } });
            if (viewID != -1)
            {
                PhotonView photonView = PhotonView.Find(viewID);
                if (photonView != null)
                {
                    GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(t1: true, (string)photonView.owner.CustomProperties[PhotonPlayerProperty.name], t2: false, (string)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
                    photonView.owner.SetCustomProperties(new Hashtable {
                    {
                        PhotonPlayerProperty.kills,
                        (int)photonView.owner.CustomProperties[PhotonPlayerProperty.kills] + 1
                    } });
                }
            }
            else
            {
                GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().sendKillInfo(t1: true, titanName, t2: false, (string)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
            }
            GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().photonView.RPC("someOneIsDead", PhotonTargets.MasterClient, (!(titanName == string.Empty)) ? 1 : 0);
        }
        GameObject gameObject = ((IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE) ? ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("hitMeat2"))) : PhotonNetwork.Instantiate("hitMeat2", base.transform.position, Quaternion.Euler(270f, 0f, 0f), 0));
        gameObject.transform.position = base.transform.position;
        if (base.photonView.isMine)
        {
            PhotonNetwork.Destroy(base.photonView);
        }
    }

    [RPC]
    private void netTauntAttack(float tauntTime, float distance = 100f)
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        GameObject[] array2 = array;
        foreach (GameObject gameObject in array2)
        {
            if (Vector3.Distance(gameObject.transform.position, base.transform.position) < distance && (bool)gameObject.GetComponent<TITAN>())
            {
                gameObject.GetComponent<TITAN>().beTauntedBy(base.gameObject, tauntTime);
            }
        }
    }

    [RPC]
    private void netlaughAttack()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("titan");
        GameObject[] array2 = array;
        foreach (GameObject gameObject in array2)
        {
            if (Vector3.Distance(gameObject.transform.position, base.transform.position) < 50f && Vector3.Angle(gameObject.transform.forward, base.transform.position - gameObject.transform.position) < 90f && (bool)gameObject.GetComponent<TITAN>())
            {
                gameObject.GetComponent<TITAN>().beLaughAttacked();
            }
        }
    }

    [RPC]
    private void net3DMGSMOKE(bool ifON)
    {
        if (smoke_3dmg != null)
        {
            smoke_3dmg.enableEmission = ifON;
        }
    }

    [RPC]
    private void showHitDamage()
    {
        GameObject gameObject = GameObject.Find("LabelScore");
        if ((bool)gameObject)
        {
            speed = Mathf.Max(10f, speed);
            gameObject.GetComponent<UILabel>().text = speed.ToString();
            gameObject.transform.localScale = Vector3.zero;
            speed = (int)(speed * 0.1f);
            speed = Mathf.Clamp(speed, 40f, 150f);
            iTween.Stop(gameObject);
            iTween.ScaleTo(gameObject, iTween.Hash("x", speed, "y", speed, "z", speed, "easetype", iTween.EaseType.easeOutElastic, "time", 1f));
            iTween.ScaleTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easetype", iTween.EaseType.easeInBounce, "time", 0.5f, "delay", 2f));
        }
    }

    [RPC]
    public void blowAway(Vector3 force)
    {
        if (IN_GAME_MAIN_CAMERA.gametype == GAMETYPE.SINGLE || base.photonView.isMine)
        {
            base.rigidbody.AddForce(force, ForceMode.Impulse);
            base.transform.LookAt(base.transform.position);
        }
    }

    [RPC]
    private void killObject()
    {
        UnityEngine.Object.Destroy(base.gameObject);
    }
}