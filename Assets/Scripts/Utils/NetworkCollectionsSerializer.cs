using System;
using System.Collections.Generic;
using Unity.Netcode;

public static class NetworkCollectionsSerializer
{
    public static void SerializeList<T, TSerializer>(ref List<T> list, BufferSerializer<TSerializer> serializer) where T : INetworkSerializable, new() where TSerializer : IReaderWriter
    {
        int count = list != null ? list.Count : 0;
        serializer.SerializeValue(ref count);
        
        if (serializer.IsWriter)
        {
            for (int i = 0; i < count; i++)
            {
                T item = list[i];
                serializer.SerializeValue(ref item);
            }
        }
        else
        {
            list = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                T item = new T();
                serializer.SerializeValue(ref item);
                list.Add(item);
            }
        }
    }

    public static void SerializeEnumerable<T, TSerializer>(ref IEnumerable<T> enumerable, BufferSerializer<TSerializer> serializer) where T : INetworkSerializable, new() where TSerializer : IReaderWriter
    {
        var list = enumerable != null ? new List<T>(enumerable) : new List<T>();
        SerializeList(ref list, serializer);
        enumerable = list;
    }
}