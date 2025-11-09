using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class LightToggleController : UdonSharpBehaviour
{
    public GameObject[] lightsToToggle;
    private bool lightsOn = true;

    void Start()
    {
        // Find all lights automatically if not set
        if (lightsToToggle == null || lightsToToggle.Length == 0)
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            System.Collections.Generic.List<GameObject> lights = new System.Collections.Generic.List<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.Contains("VRSL") || obj.name.Contains("Light") || obj.name.Contains("AudioLink"))
                {
                    lights.Add(obj);
                }
            }

            lightsToToggle = lights.ToArray();
        }

        Debug.Log($"Light Toggle: Found {lightsToToggle.Length} lights");
    }

    public void ToggleLights()
    {
        lightsOn = !lightsOn;

        foreach (GameObject lightObj in lightsToToggle)
        {
            if (lightObj != null)
            {
                // Toggle renderers
                Renderer[] renderers = lightObj.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    r.enabled = lightsOn;
                }

                // Toggle actual Unity lights
                Light[] lights = lightObj.GetComponentsInChildren<Light>();
                foreach (Light l in lights)
                {
                    l.enabled = lightsOn;
                }
            }
        }

        Debug.Log($"Lights: {(lightsOn ? "ON" : "OFF")}");
    }

    // Called when player clicks the button
    public override void Interact()
    {
        ToggleLights();
    }
}
