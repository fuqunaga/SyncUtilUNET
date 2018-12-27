using UnityEngine.Networking;

#pragma warning disable 0618

namespace SyncUtil
{
    public class CustomMsgType
    {
        public const short Time = MsgType.Highest + 1;
        public const short NetworkTime = MsgType.Highest + 2;
        public const short Latency = MsgType.Highest + 3;
        public const short LockStepConsistency = MsgType.Highest + 4;


        public const short Highest = MsgType.Highest + 4;
    }
}