using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ModuleCMMProspector : ModuleCMM
{
    [KSPField]
    public CMMResourceLocation areasToProspect;
    [KSPField]
    public CMMResourceCollection additionalResources;

    [KSPField(guiActive=true)]
    public double powerUsage = 0f;

    [KSPField(guiName="Found", guiActive=true, isPersistant=false)]
    public String found = "Nothing";

    [KSPField(guiActive=true, isPersistant=false)]
    public String planet = "BROKEN";

    [KSPEvent(guiName = "Prospect", guiActive = true)]
    public void CMProspect()
    {
        //Check that the device works in this environment.
        if (!WorksHere())
        {
            found = "Can't Prospect Here!";
            return;
        }

        double request = vessel.rootPart.RequestResource("ElectricCharge", powerUsage);
        print("Draining " + request + " Power.");
        if (request < powerUsage)
        {
            found = "Not Enough Power!";
            print("Not enough power. (Requires "+powerUsage+")");
            return;
        }

        List<string> foundList = new List<string>();
        List<CelMatMgmt.Resource> allResources = CelMatMgmt.Instance.GetCollectionResources(planet);
        foreach (CelMatMgmt.Resource resource in allResources)
        {
            if (additionalResources.Resources.Contains(resource.name)) { foundList.Add(resource.name); }
            else if (areasToProspect.Contains(resource.location)) { foundList.Add(resource.name); }
        }
        if(foundList.Count > 0)
        {
            found = CMMUtils.CommaSeparatedList(foundList);
        }
        else
        {
            found = "Nothing";
        }
    }

    public override void OnUpdate()
    {
        if (planet != vessel.mainBody.name)
        {
            planet = vessel.mainBody.name;
            found = "Nothing";
        }
    }
}