public class PhotonMessageInfo
{
    private int TimeInt;

    public PhotonPlayer Sender;

    public PhotonView PhotonView;


    public double Timestamp => (uint)TimeInt / 1000.0;

    public PhotonMessageInfo()
    {
        Sender = PhotonNetwork.Player;
        TimeInt = (int)(PhotonNetwork.Time * 1000.0);
        PhotonView = null;
    }

    public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
    {
        Sender = player;
        TimeInt = timestamp;
        PhotonView = view;
    }

    public override string ToString()
    {
        return string.Format("[PhotonMessageInfo: player='{1}' timestamp={0}]", Timestamp, Sender);
    }
}