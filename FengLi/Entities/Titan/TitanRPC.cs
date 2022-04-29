using UnityEngine;

public partial class TITAN : TitanBase
{
    [RPC]
    public void grabbedTargetEscape()
    {
        grabbedTarget = null;
    }

    [RPC]
    public void grabToLeft()
    {
        Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_L/upper_arm_L/forearm_L/hand_L/hand_L_001");
        grabTF.transform.parent = transform;
        grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
        grabTF.transform.localPosition -= Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
        grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
        grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z + 180f);
    }

    [RPC]
    public void grabToRight()
    {
        Transform transform = base.transform.Find("Amarture/Core/Controller_Body/hip/spine/chest/shoulder_R/upper_arm_R/forearm_R/hand_R/hand_R_001");
        grabTF.transform.parent = transform;
        grabTF.transform.position = transform.GetComponent<SphereCollider>().transform.position;
        grabTF.transform.rotation = transform.GetComponent<SphereCollider>().transform.rotation;
        grabTF.transform.localPosition -= Vector3.right * transform.GetComponent<SphereCollider>().radius * 0.3f;
        grabTF.transform.localPosition += Vector3.up * transform.GetComponent<SphereCollider>().radius * 0.51f;
        grabTF.transform.localPosition -= Vector3.forward * transform.GetComponent<SphereCollider>().radius * 0.3f;
        grabTF.transform.localRotation = Quaternion.Euler(grabTF.transform.localRotation.eulerAngles.x, grabTF.transform.localRotation.eulerAngles.y + 180f, grabTF.transform.localRotation.eulerAngles.z);
    }

    [RPC]
    public void hitAnkleRPC(int viewID)
    {
        if (hasDie || state == TitanState.down)
        {
            return;
        }
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView == null)
        {
            return;
        }
        float magnitude = (photonView.gameObject.transform.position - base.transform.position).magnitude;
        if (magnitude < 20f)
        {
            if (base.photonView.IsMine && grabbedTarget != null)
            {
                grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
            }
            getDown();
        }
    }

    [RPC]
    public void hitEyeRPC(int viewID)
    {
        if (hasDie)
        {
            return;
        }
        float magnitude = (PhotonView.Find(viewID).gameObject.transform.position - neck.position).magnitude;
        if (magnitude < 20f)
        {
            if (base.photonView.IsMine && grabbedTarget != null)
            {
                grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
            }
            if (!hasDie)
            {
                justHitEye();
            }
        }
    }

    [RPC]
    public void titanGetHit(int viewID, int speed)
    {
        PhotonView photonView = PhotonView.Find(viewID);
        if (photonView == null)
        {
            return;
        }
        float magnitude = (photonView.gameObject.transform.position - neck.position).magnitude;
        if (magnitude < 30f && !hasDie)
        {
            base.photonView.RPC("netDie", PhotonTargets.OthersBuffered);
            if (grabbedTarget != null)
            {
                grabbedTarget.GetPhotonView().RPC("netUngrabbed", PhotonTargets.All);
            }
            netDie();
            if (nonAI)
            {
                GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, (string)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.name]);
            }
            else
            {
                GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>().titanGetKill(photonView.owner, speed, base.name);
            }
        }
    }

    [RPC]
    private void dieBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.photonView.IsMine)
        {
            float magnitude = (attacker - base.transform.position).magnitude;
            if (magnitude < 80f)
            {
                dieBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void dieHeadBlowRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.photonView.IsMine)
        {
            float magnitude = (attacker - base.transform.position).magnitude;
            if (magnitude < 80f)
            {
                dieHeadBlowFunc(attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void hitLRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.photonView.IsMine)
        {
            float magnitude = (attacker - base.transform.position).magnitude;
            if (magnitude < 80f)
            {
                hit("hit_eren_L", attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void hitRRPC(Vector3 attacker, float hitPauseTime)
    {
        if (base.photonView.IsMine && !hasDie)
        {
            float magnitude = (attacker - base.transform.position).magnitude;
            if (magnitude < 80f)
            {
                hit("hit_eren_R", attacker, hitPauseTime);
            }
        }
    }

    [RPC]
    private void laugh(float sbtime = 0f)
    {
        if (state == TitanState.idle || state == TitanState.turn || state == TitanState.chase)
        {
            this.sbtime = sbtime;
            state = TitanState.laugh;
            crossFade("laugh", 0.2f);
        }
    }

    [RPC]
    private void netCrossFade(string aniName, float time)
    {
        base.animation.CrossFade(aniName, time);
    }

    [RPC]
    private void netDie()
    {
        asClientLookTarget = false;
        if (!hasDie)
        {
            hasDie = true;
            if (nonAI)
            {
                currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setMainObject(null);
                currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().setSpectorMode(val: true);
                currentCamera.GetComponent<IN_GAME_MAIN_CAMERA>().gameOver = true;
                PhotonNetwork.Player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
                {
                    PhotonPlayerProperty.dead,
                    true
                } });
                PhotonNetwork.Player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable {
                {
                    PhotonPlayerProperty.deaths,
                    (int)PhotonNetwork.Player.CustomProperties[PhotonPlayerProperty.deaths] + 1
                } });
            }
            dieAnimation();
        }
    }

    [RPC]
    private void netPlayAnimation(string aniName)
    {
        base.animation.Play(aniName);
    }

    [RPC]
    private void netPlayAnimationAt(string aniName, float normalizedTime)
    {
        base.animation.Play(aniName);
        base.animation[aniName].normalizedTime = normalizedTime;
    }

    [RPC]
    private void netSetAbnormalType(int type)
    {
        switch (type)
        {
            case 0:
                abnormalType = AbnormalType.NORMAL;
                base.name = "Titan";
                runAnimation = "run_walk";
                GetComponent<TITAN_SETUP>().setHair();
                break;

            case 1:
                abnormalType = AbnormalType.TYPE_I;
                base.name = "Aberrant";
                runAnimation = "run_abnormal";
                GetComponent<TITAN_SETUP>().setHair();
                break;

            case 2:
                abnormalType = AbnormalType.TYPE_JUMPER;
                base.name = "Jumper";
                runAnimation = "run_abnormal";
                GetComponent<TITAN_SETUP>().setHair();
                break;

            case 3:
                abnormalType = AbnormalType.TYPE_CRAWLER;
                base.name = "Crawler";
                runAnimation = "crawler_run";
                GetComponent<TITAN_SETUP>().setHair();
                break;

            case 4:
                abnormalType = AbnormalType.TYPE_PUNK;
                base.name = "Punk";
                runAnimation = "run_abnormal_1";
                GetComponent<TITAN_SETUP>().setPunkHair();
                break;
        }
        if (abnormalType == AbnormalType.TYPE_I || abnormalType == AbnormalType.TYPE_JUMPER || abnormalType == AbnormalType.TYPE_PUNK)
        {
            speed = 18f;
            if (myLevel > 1f)
            {
                speed *= Mathf.Sqrt(myLevel);
            }
            if (myDifficulty == 1)
            {
                speed *= 1.4f;
            }
            if (myDifficulty == 2)
            {
                speed *= 1.6f;
            }
            base.animation["turnaround1"].speed = 2f;
            base.animation["turnaround2"].speed = 2f;
        }
        if (abnormalType == AbnormalType.TYPE_CRAWLER)
        {
            chaseDistance += 50f;
            speed = 25f;
            if (myLevel > 1f)
            {
                speed *= Mathf.Sqrt(myLevel);
            }
            if (myDifficulty == 1)
            {
                speed *= 2f;
            }
            if (myDifficulty == 2)
            {
                speed *= 2.2f;
            }
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().height = 10f;
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().radius = 5f;
            base.transform.Find("AABB").gameObject.GetComponent<CapsuleCollider>().center = new Vector3(0f, 5.05f, 0f);
        }
        if (nonAI)
        {
            if (abnormalType == AbnormalType.TYPE_CRAWLER)
            {
                speed = Mathf.Min(70f, speed);
            }
            else
            {
                speed = Mathf.Min(60f, speed);
            }
            base.animation["attack_jumper_0"].speed = 7f;
            base.animation["attack_crawler_jump_0"].speed = 4f;
        }
        base.animation["attack_combo_1"].speed = 1f;
        base.animation["attack_combo_2"].speed = 1f;
        base.animation["attack_combo_3"].speed = 1f;
        base.animation["attack_quick_turn_l"].speed = 1f;
        base.animation["attack_quick_turn_r"].speed = 1f;
        base.animation["attack_anti_AE_l"].speed = 1.1f;
        base.animation["attack_anti_AE_low_l"].speed = 1.1f;
        base.animation["attack_anti_AE_r"].speed = 1.1f;
        base.animation["attack_anti_AE_low_r"].speed = 1.1f;
        idle();
    }

    [RPC]
    private void netSetLevel(float level, int AI, int skinColor)
    {
        setLevel(level, AI, skinColor);
    }

    [RPC]
    private void playsoundRPC(string sndname)
    {
        Transform transform = base.transform.Find(sndname);
        transform.GetComponent<AudioSource>().Play();
    }

    [RPC]
    private void setIfLookTarget(bool bo)
    {
        asClientLookTarget = bo;
    }

    [RPC]
    private void setMyTarget(int ID)
    {
        if (ID == -1)
        {
            myHero = null;
        }
        PhotonView photonView = PhotonView.Find(ID);
        if (photonView != null)
        {
            myHero = photonView.gameObject;
        }
    }
}