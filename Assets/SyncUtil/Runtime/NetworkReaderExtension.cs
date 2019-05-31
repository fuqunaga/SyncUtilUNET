using Mirror;

namespace SyncUtil
{
    public static class NetworkReaderExtension
    {
        public static T ReadMessage<T>(this NetworkReader reader)
            where T : MessageBase, new ()
        {
            var msg = new T();
            msg.Deserialize(reader);
            return msg;
        }

    }
}