using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extention 
{
    public static string ToFormattedString(this int num)
    {
        return num.ToString("N0");
    }
}
