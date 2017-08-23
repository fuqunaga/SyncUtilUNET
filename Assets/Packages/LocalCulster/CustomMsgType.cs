using UnityEngine.Networking;

public class CustomMsgType
{
    //public const short ChangeMode = MsgType.Highest + 1;
    public const short Time = MsgType.Highest + 2;
    public const short NetworkTime = MsgType.Highest + 3;
    public const short Latency = MsgType.Highest + 4;

    public const short Highest = MsgType.Highest + 5;
}
