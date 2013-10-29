using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ModuleCMM : PartModule
{
    [KSPField]
    public CMMResourceLocation worksIn;

    public override void OnLoad(ConfigNode node)
    {
        base.OnLoad(node);
        foreach (ConfigNode sub in node.nodes)
        {
            Debug.LogWarning(" - " + sub.name);
        }
    }

    public bool WorksHere()
    {
        bool works = false;
        if (worksIn == null)
        {
            Debug.LogWarning("No worksIn value for "+this.ClassName, part);
            return false;
        }
        if (worksIn.Land && vessel.Landed) works = true;
        if (worksIn.Sea && vessel.Splashed) works = true;
        if (worksIn.Atmos && vessel.atmDensity > 0) works = true;
        if (worksIn.Space && vessel.atmDensity <= 0) works = true;
        return works;
    }
}
