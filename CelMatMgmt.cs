using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
public partial class CelMatMgmt : MonoBehaviour
{
    /// <summary>
    /// An instance of this plugin. Used by PartModules, generally.
    /// </summary>
    public static CelMatMgmt Instance { get { return instance; } }
    static CelMatMgmt instance;

    //The directory where savefiles are stored.
    const String pluginRoot = "GameData/CMM";

    /// <summary>
    /// The title of the loaded game with non-alphanumeric characters removed.
    /// </summary>
    String canonicalGameName;
    /// <summary>
    /// A dictionary containing lists of resources you can collect on a planet.
    /// </summary>
    /// <param name="key">The name of the planet</param>
    /// <param name="value">A list of objects that describe what resources can be collected on the planet.</param>
    Dictionary<String, List<Resource>> planetResources;

    /// <summary>
    /// A dictionary linking resource names to the quantity stored on Kerbin at the KSC.
    /// </summary>
    Dictionary<String, double> resourcesAtKSC;

    public void Awake()
    {
        instance = this;
    }
    
    public void Start()
    {
        //Store the game's name, load up the existing save if any.
        canonicalGameName = CMMUtils.CanonicalGameName(HighLogic.CurrentGame.Title);
        print("Game Name: " + canonicalGameName);
        ConfigNode save = ReadSavefile();

        //Populate the resource storage table (resources the player already has) with either saved data or zeroes.
        resourcesAtKSC = new Dictionary<string, double>();
        print("Resources At KSC: ");
        foreach (ConfigNode def in GameDatabase.Instance.GetConfigNodes("RESOURCE_DEFINITION"))
        {
            String resource = def.GetValue("name");
            double amount = 0.0;
            if (save != null)
            {
                double.TryParse(save.GetValue(resource), out amount);
            }
            resourcesAtKSC.Add(resource, amount);
            print(" - " + resource + ": " + amount);
        }

        //Populate the collectable resource location table (resources the player can gather).
        planetResources = new Dictionary<string, List<Resource>>();
        foreach (ConfigNode body in GameDatabase.Instance.GetConfigNodes("CMM_BODY"))
        {
            String bodyName = body.GetValue("name");
            print("For Planet: " + bodyName);
            List<Resource> resources = new List<Resource>();
            foreach (ConfigNode resource in body.GetNodes("RESOURCE"))
            {
                String resourceName = resource.GetValue("name");
                if (!resourcesAtKSC.ContainsKey(resourceName))
                {
                    Debug.LogError(bodyName + ": " + resourceName + " matches no defined resources. Discarding.", this);
                    continue;
                }
                ConfigNode loc = resource.GetNode("location");
                if (loc == null)
                {
                    Debug.LogError(bodyName + ": " + resourceName + ": No location node. Remember not to assign a value.", this);
                    continue;
                }
                double rate = 0.0;
                if (!double.TryParse(resource.GetValue("collectionRate"), out rate))
                {
                    continue;
                }
                print(" - " + resourceName + " (" + tag + ") : " + rate + "/s");
                resources.Add(new Resource(resourceName, new CMMResourceLocation(loc), rate));
            }
            if (resources.Count > 0)
            {
                planetResources.Add(bodyName, resources);
            }
        }
    }

    /// <summary>
    /// Searches the database for saved data regarding this game.
    /// </summary>
    /// <returns>A ConfigNode with data about stored resources.</returns>

    ConfigNode ReadSavefile()
    {
        foreach (ConfigNode persistence in GameDatabase.Instance.GetConfigNodes("CMM_SAVEFILE"))
        {
            Debug.LogWarning("Found Save Data: "+persistence.GetValue("name"), this);
            if (persistence.GetValue("name") == canonicalGameName)
            {
                return persistence;
            }
        }
        Debug.LogWarning("No saved data found for " + canonicalGameName, this);
        return null;
    }

    /// <summary>
    /// Writes a config file named after the current game containing the saved data.
    /// </summary>
    void WriteSavefile()
    {
        ConfigNode saveRoot = new ConfigNode();
        ConfigNode saveResources = new ConfigNode("CMM_SAVEFILE");
        saveRoot.AddNode(saveResources);
        saveResources.AddValue("name", canonicalGameName);
        foreach (KeyValuePair<String, double> kvp in resourcesAtKSC)
        {
            saveResources.AddValue(kvp.Key, kvp.Value.ToString());
        }
        saveRoot.Save(pluginRoot + "/" + canonicalGameName + ".cfg", "HERPDERP");
    }

    /// <summary>
    /// Gets a list of all resources available on the specified body.
    /// </summary>
    /// <param name="bodyName">The name of a planet or moon.</param>
    /// <returns>A list of CelMatMgmt.Resource objects detailing what can be collected.</returns>
    public List<Resource> GetCollectionResources(String bodyName)
    {
        try
        {
            return planetResources[bodyName];
        }
        catch (ArgumentOutOfRangeException)
        {
            return new List<Resource>();
        }
    }

    /// <summary>
    /// Gets a list of collected and stored resources available at the specified location.
    /// </summary>
    /// <param name="bodyName">The name of a planet or moon.</param>
    /// <returns>A dictionary mapping resource names to quantities.</returns>
    public Dictionary<String, double> GetReserves(String bodyName)
    {
        //Ignore body name for now, just get KSC's resources.
        return resourcesAtKSC;
    }

    /// <summary>
    /// Contains data about what can be collected on a planet or moon.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// The name of a defined resource.
        /// </summary>
        public String name { get; protected set; }
        /// <summary>
        /// A tag representing what general tools can collect this resource.
        /// </summary>
        public CMMResourceLocation location { get; protected set; }
        /// <summary>
        /// The rate at which each tool collects resources.
        /// </summary>
        public double collectionRate { get; protected set; } //per second

        /// <summary>
        /// Makes a new resource entry.
        /// </summary>
        /// <param name="name">The name of a defined resource.</param>
        /// <param name="tag">Tag representing tools that can collect this resource.</param>
        /// <param name="rate">Rate of collection per second.</param>
        public Resource(String name, CMMResourceLocation loc, double rate)
        {
            name = name;
            location = loc;
            collectionRate = rate;
        }
    }

    public override string ToString()
    {
        return "CMM-"+canonicalGameName;
    }
}
