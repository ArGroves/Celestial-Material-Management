using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CMMUtils
{
    public static String CanonicalGameName(String gameName)
    {
        StringBuilder canonicalName = new StringBuilder();
        foreach (char c in gameName)
        {
            if (char.IsLetterOrDigit(c))
            {
                canonicalName.Append(c);
            }
        }
        return canonicalName.ToString();
    }

    public static String CommaSeparatedList(List<string> list)
    {
        StringBuilder build = new StringBuilder();
        string last = list[list.Count - 1];
        foreach(string s in list)
        {
            build.Append(s);
            if (s != last)
            {
                build.Append(", ");
            }
        }
        return build.ToString();
    }

    public static void ManualLoad(string nodeName, IConfigNode obj, ConfigNode parent)
    {
        if (parent.HasNode(nodeName)) obj.Load(parent.GetNode(nodeName));
        else Debug.LogError("["+obj+"]: Node " + nodeName + " does not exist.");
    }
    public static void ManualSave(string nodeName, IConfigNode obj, ConfigNode parent)
    {
        ConfigNode save = new ConfigNode(nodeName);
        obj.Save(save);
        parent.AddNode(save);
    }
}