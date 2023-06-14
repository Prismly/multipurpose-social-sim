using Anthology.SimulationManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    [SerializeField] private GameObject actorPref;
    [SerializeField] private GameObject locationPref;

    public static UnityEvent actorsUpdated;

    private static Dictionary<int, Actor> actors = new Dictionary<int, Actor>();
    private static HashSet<int> selected = new HashSet<int>();
    private static int focused;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            SimManager.Init("Assets/Scripts/SimManager/Data/Paths.json", typeof(AnthologyRS), typeof(LyraKS));
            actorsUpdated = new UnityEvent();
        }
    }

    private void Start()
    {
        foreach (NPC npc in SimManager.NPCs.Values)
        {
            Actor spawnedActor = Instantiate(actorPref).GetComponent<Actor>();
            if (spawnedActor == null)
            {
                Debug.LogError("Uh oh! Actor prefab doesn't have attached Actor component!");
                break;
            }
            spawnedActor.Init(npc.Name);
        }

        foreach (Location loc in SimManager.Locations.Values)
        {
            GameObject spawnedLocation = Instantiate(locationPref);
            spawnedLocation.name = loc.Name;
            spawnedLocation.transform.position = new Vector3(loc.Coordinates.X, loc.Coordinates.Y, 0);
        }
    }

    /**
     * Attempts to set which Agent is "focused" (is selected/hovered on the frontend).
     * This fails if there is currently a focused Agent.
     * @param targetID is the ID of the Agent to focus.
     * @return whether the currently focused Agent successfully changed.
     */
    public static bool FocusAgent(int targetID)
    {
        // TODO: Should a check occur for attempting to focus the currently focused Agent (i.e. unfocus it)?
        if (focused > -1)
            return false; // Focus already occupied by another Agent

        focused = targetID;
        return true;
    }

    /**
     * Attempts to unfocus the currently "focused" (is selected/hovered on the frontend) Agent.
     * This fails if the currently focused Agent's ID does not match the passed ID.
     * @param targetID is the ID of the Agent to unfocus.
     * @return whether the currently focused Agent was successfully unfocused.
     */
    public static bool UnfocusAgent(int targetID)
    {
        if (focused != targetID)
            return false; // Focus not occupied by Agent with target ID

        focused = -1;
        return true;
    }

    /**
     * Attempts to add a new Agent to the static Dictionary, using its AgentID as the key.
     * This fails if the Dictionary already contains a value with the desired key.
     * @param registree is the Agent to register.
     * @return whether the addition was successful or not.
     */
    public static bool RegisterAgent(Actor registree)
    {
        if (!actors.ContainsKey(registree.AgentID))
        {
            actors.Add(registree.AgentID, registree);
            return true;
        }
        return false;
    }
}