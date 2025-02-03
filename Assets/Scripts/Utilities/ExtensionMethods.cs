using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class ExtensionMethods
{
    public static int Mod(this int a, int b)
    {
        return (a % b + b) % b;
    }

    public static Vector3 NewX(this Vector3 vec, float newVal) => new(newVal, vec.y, vec.z);
    public static Vector3 NewY(this Vector3 vec, float newVal) => new(vec.x, newVal, vec.z);
    public static Vector3 NewZ(this Vector3 vec, float newVal) => new(vec.x, vec.y, newVal);


    public static T RandomItem<T>(this List<T> itemList) => itemList[Random.Range(0, itemList.Count())];

    public static T RandomItem<T>(this T[] itemList) => itemList[Random.Range(0, itemList.Count())];

    //public static IEnumerable<string> SplitCamelCase(this string source)
    //{
    //    const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
    //    MatchCollection matches = Regex.Matches(source, pattern);
    //    foreach (Match match in matches)
    //    {
    //        yield return match.Value;
    //    }
    //}

    //static IEnumerable<string> SplitCamelCase(this string source)
    //{
    //    const string pattern = @"[A-Z][a-z]*|[a-z]+|\d+";
    //    MatchCollection matches = Regex.Matches(source, pattern);
    //    foreach (Match match in matches)
    //    {
    //        yield return match.Value;
    //    }
    //}


    //public static string GetUINameFormat(this string source)
    //{
    //    string retVal = "";

    //    string[] splitName = source.SplitCamelCase().ToArray();
    //    Debug.Log(splitName[0]);

    //    for(int i = 0; i < splitName.Length-1; i++)
    //    {
    //        retVal += splitName[i].ToLower();
    //        retVal += '-';
    //    }

    //    retVal += splitName[^1].ToLower();

    //    return retVal;
    //}

    //public static string GetXMLNameFormat(this string source)
    //{
    //    string retVal = "";

    //    string[] splitName = source.SplitCamelCase().ToArray();


    //    for (int i = 0; i < splitName.Length; i++)
    //    {
    //        string str = splitName[i].ToLower();
    //        str = char.ToUpper(str[0]) + str.Substring(1);

    //        retVal += str;
    //    }
    //    return retVal;
    //}

}
