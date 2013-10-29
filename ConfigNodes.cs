using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//A great many of these are glorified enums.

public class CMMResourceCollection : IConfigNode
{
    public List<string> Resources { get; private set; }

    public CMMResourceCollection()
    {
        Resources = new List<string>();
    }

    public void Load(ConfigNode node)
    {
        Debug.Log("CMM: Loading Resource Collection");
        if (!node.HasValue("resource"))
        {
            Debug.LogError("No resource values in collection " + node.name);
            return;
        }
        foreach (string resource in node.GetValues("resource"))
        {
            Resources.Add(resource);
        }
    }

    public void Save(ConfigNode node)
    {
        Debug.Log("CMM: Saving Resource Collection");
        foreach (string resource in Resources)
        {
            node.AddValue("resource", resource);
        }
    }
}

//Can refer to both areas and collection device categories.
public class CMMResourceLocation : IConfigNode
{
    public bool Land { get; protected set; }
    public bool Sea { get; protected set; }
    public bool Atmos { get; protected set; }
    public bool Space { get; protected set; }

    public CMMResourceLocation() { }
    public CMMResourceLocation(ConfigNode loadFrom)
    {
        if (loadFrom != null)
        {
            Load(loadFrom);
        }
    }

    public bool Contains(CMMResourceLocation other)
    {
        return (Land && other.Land) || (Sea && other.Sea) || (Space && other.Space) || (Atmos && other.Atmos);
    }

    public override bool Equals(object obj)
    {
        if (!(obj is CMMResourceLocation)) return false;
        CMMResourceLocation other = (CMMResourceLocation)obj;
        return Land == other.Land && Sea == other.Sea && Space == other.Space && Atmos == other.Atmos;
    }
    public override int GetHashCode()
    {
        int hash = 0;
        if (Land) hash += 1;
        if (Sea) hash += 2;
        if (Atmos) hash += 4;
        if (Space) hash += 8;
        return hash;
    }
    public override string ToString()
    {
        StringBuilder b = new StringBuilder();
        if (Land) b.Append("Land");
        if (Sea) b.Append("Sea");
        if (Atmos) b.Append("Atmos");
        if (Space) b.Append("Space");
        return b.ToString();
    }

    public void Load(ConfigNode node)
    {
        Debug.Log("CMM: Loading Resource Location");
        if (!node.HasValue("area"))
        {
            Debug.LogError("No area values in location " + node.name);
            return;
        }
        foreach (string Loc in node.GetValues("area"))
        {
            string loc = Loc.ToLower();
            if (loc == "land" || loc == "ground") Land = true;
            if (loc == "sea" || loc == "ocean" || loc == "water") Sea = true;
            if (loc.StartsWith("atmo") || loc == "air") Atmos = true;
            if (loc == "space" || loc == "orbit") Space = true;
        }
    }

    public void Save(ConfigNode node)
    {
        Debug.Log("CMM: Saving Resource Location");
        if (Land) node.AddValue("area", "Land");
        if (Sea) node.AddValue("area", "Sea");
        if (Atmos) node.AddValue("area", "Atmos");
        if (Space) node.AddValue("area", "Space");
    }
}