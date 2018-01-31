using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncUtil
{
    /// <summary>
    /// LockStepOffline
    /// A class that provides a lockstep interface when offline
    /// </summary>
    public class LockStepOfflineStub : MonoBehaviour, ILockStep
    {
        public static LockStep.ConsistencyData consistencyData = new LockStep.ConsistencyData();

        public int stepCount;

        protected Func<MessageBase> _getDataFunc;
        protected Func<int, NetworkReader, bool> _stepFunc;




        #region Unity

        private void Update()
        {
            var msg = _getDataFunc();

            var writer = new NetworkWriter();
            writer.Write(msg);
            if (_stepFunc(stepCount, new NetworkReader(writer)))
            {
                stepCount++;
            }
        }

        #endregion


        #region  Override

        public Func<MessageBase> getDataFunc { set { _getDataFunc = value; } }
        public Func<int, NetworkReader, bool> stepFunc { set { _stepFunc = value; } }
        public Func<bool> onMissingCatchUpServer { set { } }
        public Action onMissingCatchUpClient { set { } }
        public Func<string> getHashFunc { set { } }

        public LockStep.ConsistencyData GetLastConsistencyData()
        {
            return consistencyData;
        }

        public void StartCheckConsistency()
        {
        }


        public int stepCountServer => stepCount;
        public int stepCountClient => stepCount;
        #endregion
    }
}