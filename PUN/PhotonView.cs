using System.Reflection;
using Photon;
using UnityEngine;

[AddComponentMenu("Miscellaneous/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public int subId;

	public int ownerId;

	public int group;

	protected internal bool mixedModeIsReliable;

	public int prefixBackup = -1;

	private object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public Component observed;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public int instantiationId;

	protected internal bool didAwake;

	protected internal bool destroyedByPhotonNetworkOrQuit;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;

	public int prefix
	{
		get
		{
			if (prefixBackup == -1 && PhotonNetwork.NetworkingPeer != null)
			{
				prefixBackup = PhotonNetwork.NetworkingPeer.currentLevelPrefix;
			}
			return prefixBackup;
		}
		set
		{
			prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!didAwake)
			{
				instantiationDataField = PhotonNetwork.NetworkingPeer.FetchInstantiationData(instantiationId);
			}
			return instantiationDataField;
		}
		set
		{
			instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return ownerId * PhotonNetwork.MAX_VIEW_IDS + subId;
		}
		set
		{
			bool flag = didAwake && subId == 0;
			ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			subId = value % PhotonNetwork.MAX_VIEW_IDS;
			if (flag)
			{
				PhotonNetwork.NetworkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView => ownerId == 0;

	public PhotonPlayer owner => PhotonPlayer.Find(ownerId);

	public int OwnerActorNr => ownerId;

	public bool IsMine => ownerId == PhotonNetwork.Player.ID || (isSceneView && PhotonNetwork.IsMasterClient);

	protected internal void Awake()
	{
		PhotonNetwork.NetworkingPeer.RegisterPhotonView(this);
		instantiationDataField = PhotonNetwork.NetworkingPeer.FetchInstantiationData(instantiationId);
		didAwake = true;
	}

	protected internal void OnApplicationQuit()
	{
		destroyedByPhotonNetworkOrQuit = true;
	}

	protected internal void OnDestroy()
	{
		if (!destroyedByPhotonNetworkOrQuit)
		{
			PhotonNetwork.NetworkingPeer.LocalCleanPhotonView(this);
		}
		if (!destroyedByPhotonNetworkOrQuit && !Application.isLoadingLevel)
		{
			if (instantiationId > 0)
			{
				Debug.LogError(string.Concat("OnDestroy() seems to be called without PhotonNetwork.Destroy()?! GameObject: ", base.gameObject, " Application.isLoadingLevel: ", Application.isLoadingLevel));
			}
			else if (viewID <= 0)
			{
				Debug.LogWarning($"OnDestroy manually allocated PhotonView {this}. The viewID is 0. Was it ever (manually) set?");
			}
			else if (IsMine && !PhotonNetwork.ManuallyAllocatedViewIds.Contains(viewID))
			{
				Debug.LogWarning($"OnDestroy manually allocated PhotonView {this}. The viewID is local (isMine) but not in manuallyAllocatedViewIds list. Use UnAllocateViewID() after you destroyed the PV.");
			}
		}
		if (PhotonNetwork.NetworkingPeer.instantiatedObjects.ContainsKey(instantiationId))
		{
			GameObject gameObject = PhotonNetwork.NetworkingPeer.instantiatedObjects[instantiationId];
			bool flag = gameObject == base.gameObject;
			if (flag)
			{
				Debug.LogWarning(string.Format("OnDestroy for PhotonView {0} but GO is still in instantiatedObjects. instantiationId: {1}. Use PhotonNetwork.Destroy(). {2} Identical with this: {3} PN.Destroyed called for this PV: {4}", this, instantiationId, (!Application.isLoadingLevel) ? string.Empty : "Loading new scene caused this.", flag, destroyedByPhotonNetworkOrQuit));
			}
		}
	}

	protected internal void ExecuteOnSerialize(PhotonStream pStream, PhotonMessageInfo info)
	{
		if (!failedToFindOnSerialize)
		{
			if (OnSerializeMethodInfo == null && !NetworkingPeer.GetMethod(observed as UnityEngine.MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out OnSerializeMethodInfo))
			{
				Debug.LogError("The observed monobehaviour (" + observed.name + ") of this PhotonView does not implement OnPhotonSerializeView()!");
				failedToFindOnSerialize = true;
			}
			else
			{
				OnSerializeMethodInfo.Invoke(observed, new object[2] { pStream, info });
			}
		}
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		if (PhotonNetwork.NetworkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
		{
			PhotonNetwork.RPC(this, methodName, PhotonNetwork.MasterClient, parameters);
		}
		else
		{
			PhotonNetwork.RPC(this, methodName, target, parameters);
		}
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.NetworkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", viewID, (!(base.gameObject != null)) ? "GO==null" : base.gameObject.name, (!isSceneView) ? string.Empty : "(scene)", prefix);
	}
}
