using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Extention 
{
    public static string ToFormattedString(this int num)
    {
        return num.ToString("N0");
    }

    public static T DeepClone<T>(this T obj)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Position = 0;

            return (T)formatter.Deserialize(stream);
        }
    }

    public static void SetActive(this Component component, bool active)
    {
        if(component == null)
        {
            Debug.LogError("Component is null");
            return;
        }

        GameObject gameObject = component.gameObject;
        if (gameObject == null)
        {
            Debug.LogError("GameObject is null");
            return;
        }

        if(gameObject.activeInHierarchy != active)
            gameObject.SetActive(active);
    }
}
