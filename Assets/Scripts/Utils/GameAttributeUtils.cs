using System;
using System.Collections.Generic;
using Unity.Netcode;



public class GameAttributeUtils
{
    public static float GetValuesFrom(IEnumerable<GameAttribute> list, GameAttributeType type)
    {
        foreach (var attribute in list)
        {
            if (attribute.Type == type)
            {
                return attribute.Value;
            }
        }
        return 0;
    }

    public static float GetValuesFrom(NetworkList<GameAttribute> list, GameAttributeType type)
    {
        foreach (var attribute in list)
        {
            if (attribute.Type == type)
            {
                return attribute.Value;
            }
        }
        return 0;
    }


}
