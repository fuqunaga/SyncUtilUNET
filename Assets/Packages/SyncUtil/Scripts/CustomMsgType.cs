using UnityEngine.Networking;

namespace SyncUtil
{
    public class CustomMsgType
    {
        public const short Time = MsgType.Highest + 1;
        public const short NetworkTime = MsgType.Highest + 2;
        public const short Latency = MsgType.Highest + 3;

        public const short Highest = MsgType.Highest + 3;
    }
}