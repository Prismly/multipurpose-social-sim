using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [Header(" --- Generic Panel ---")]
    [Tooltip("A list of Panels that should Hide when this object Hides.\n" +
        "(These objects do not necessarily show when this object shows, hence the distinction from in-scene parenting!)")]
    [SerializeField] private List<Panel> connectedPanels;
    [Tooltip("Local variable tracking whether this panel is currently Shown or Hidden.")]
    [SerializeField] protected bool isShown = false;
    public bool IsShown { get { return isShown; } }

    /**
     * Looks at the given index of the Connected Panels list, and calls Toggle for that Panel.
     * Logs an error and does nothing if the index is invalid.
     * @param idx is the target Panel's index in the Connected Panels list.
     */
    public void ToggleConnected(int idx)
    {
        if (idx < 0 || idx >= connectedPanels.Count)
        {
            Debug.LogError("Panel tried to toggle Connected Panel #" + idx + ", but no such Panel exists!");
            return;
        }
        connectedPanels[idx].Toggle();
    }

    /**
     * Delegates to either the Show or Hide functions to change the state of IsShown.
     * For instance, if IsShown is true, Hide will be called, and vice versa.
     */
    public void Toggle()
    {
        if (!IsShown)
            Show();
        else
            Hide();
    }

    /**
     * Makes this Panel's GameObject visible and records that it is Shown.
     */
    public virtual void Show()
    {
        isShown = true;
        gameObject.SetActive(true);
    }

    /**
     * Makes this Panel's GameObject invisible and records that it is Hidden.
     * Additionally, broadcasts the call to Hide to all Connected Panels.
     */
    public virtual void Hide()
    {
        isShown = false;
        gameObject.SetActive(false);
        foreach (Panel p in connectedPanels)
        {
            p.Hide();
        }
    }
}
