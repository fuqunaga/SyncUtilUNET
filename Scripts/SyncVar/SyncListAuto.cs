using Mirror;

namespace SyncUtil
{
    /// <summary>
    /// SyncList which does not need implementation of SerializeItem/DeserializeItem
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SyncListAuto<T> : SyncList<T>
    {
        protected override void SerializeItem(NetworkWriter writer, T item)
        {
            ReflectionSerializer.Serialize(writer, item);
        }

        protected override T DeserializeItem(NetworkReader reader)
        {
            return ReflectionSerializer.Deserialize<T>(reader);
        }
    }
}