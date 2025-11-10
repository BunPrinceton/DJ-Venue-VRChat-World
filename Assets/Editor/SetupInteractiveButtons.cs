using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using UdonSharp;
using VRC.SDK3.Components;
using VRC.Udon;

public class SetupInteractiveButtons : EditorWindow
{
    [MenuItem("Tools/Setup Button Interactions")]
    public static void SetupButtons()
    {
        if (!EditorUtility.DisplayDialog("Setup Button Interactions",
            "This will add VRChat interaction to all buttons:\n\n" +
            "• Mirror quality buttons (High/Low)\n" +
            "• Light toggle button\n\n" +
            "Make them clickable in VRChat!\n\n" +
            "Continue?",
            "Yes!", "Cancel"))
        {
            return;
        }

        int setupCount = 0;

        // Setup Mirror Buttons
        GameObject mirror = GameObject.Find("Mirror");
        GameObject highQualityButton = GameObject.Find("HighQualityButton");
        GameObject lowQualityButton = GameObject.Find("LowQualityButton");
        GameObject lightToggleButton = GameObject.Find("LightToggleButton");

        if (mirror == null)
        {
            Debug.LogWarning("Mirror not found! Run the merge tool first.");
        }

        // Setup High Quality Button
        if (highQualityButton != null)
        {
            SetupMirrorButton(highQualityButton, mirror, "SetHighQuality");
            setupCount++;
        }

        // Setup Low Quality Button
        if (lowQualityButton != null)
        {
            SetupMirrorButton(lowQualityButton, mirror, "SetLowQuality");
            setupCount++;
        }

        // Setup Light Toggle Button
        if (lightToggleButton != null)
        {
            SetupLightToggleButton(lightToggleButton);
            setupCount++;
        }

        EditorUtility.DisplayDialog("Success!",
            $"✓ Set up {setupCount} interactive buttons\n\n" +
            "Buttons are now clickable in VRChat!\n\n" +
            "In VRChat:\n" +
            "• Look at button\n" +
            "• Press Left Mouse (desktop) or Trigger (VR)\n" +
            "• Button activates!",
            "Awesome!");

        Debug.Log($"Interactive buttons setup complete! ({setupCount} buttons)");
    }

    private static void SetupMirrorButton(GameObject button, GameObject mirror, string methodName)
    {
        // Add box collider if not present
        BoxCollider collider = button.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = button.AddComponent<BoxCollider>();
        }

        // Get or add the MirrorQualityController script
        MirrorQualityController controller = button.GetComponent<MirrorQualityController>();
        if (controller == null)
        {
            controller = button.AddComponent<MirrorQualityController>();
        }

        // Assign the mirror reference
        if (mirror != null)
        {
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("mirror").objectReferenceValue = mirror;
            so.ApplyModifiedProperties();
        }

        // Setup Udon Interact
        SetupUdonInteract(button, methodName);

        Debug.Log($"✓ {button.name} setup with {methodName}");
    }

    private static void SetupLightToggleButton(GameObject button)
    {
        // Add box collider
        BoxCollider collider = button.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = button.AddComponent<BoxCollider>();
        }

        // Get or add LightToggleController
        LightToggleController controller = button.GetComponent<LightToggleController>();
        if (controller == null)
        {
            controller = button.AddComponent<LightToggleController>();
        }

        // Find all lights automatically
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        System.Collections.Generic.List<GameObject> lights = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("VRSL") || obj.name.Contains("Light") ||
                (obj.name.Contains("AudioLink") && obj.GetComponent<Renderer>() != null))
            {
                lights.Add(obj);
            }
        }

        // Assign lights array
        SerializedObject so = new SerializedObject(controller);
        SerializedProperty lightsProp = so.FindProperty("lightsToToggle");
        lightsProp.arraySize = lights.Count;
        for (int i = 0; i < lights.Count; i++)
        {
            lightsProp.GetArrayElementAtIndex(i).objectReferenceValue = lights[i];
        }
        so.ApplyModifiedProperties();

        // Setup Udon Interact
        SetupUdonInteract(button, "ToggleLights");

        Debug.Log($"✓ {button.name} setup with {lights.Count} lights");
    }

    private static void SetupUdonInteract(GameObject obj, string methodName)
    {
        // This creates the interaction that triggers when player clicks
        UdonBehaviour udon = obj.GetComponent<UdonBehaviour>();
        if (udon != null)
        {
            // Set interact text
            SerializedObject so = new SerializedObject(udon);
            so.FindProperty("interactText").stringValue = $"Click to {methodName}";
            so.ApplyModifiedProperties();
        }

        Debug.Log($"  - Udon interact enabled: {methodName}");
    }
}
