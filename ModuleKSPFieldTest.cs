using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ModuleKSPFieldTest : PartModule
{
    [KSPField(guiActive = true)]
    float powerConsumption = 0f;
    [KSPField(guiActive = true)]
    string status = "Broken";

    public override void OnStart(StartState state)
    {
        status = "Started";
    }

    public override void OnLoad(ConfigNode node)
    {
        base.OnLoad(node);
        print("Power Consumption: " + powerConsumption);
        print("Status: " + status);
    }
}
