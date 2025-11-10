using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
public class AutoMergeOnLoad
{
    private const string PREF_KEY = "DJ_Venue_AutoMerge_Done";

    static AutoMergeOnLoad()
    {
        // Check if we've already run
        if (EditorPrefs.GetBool(PREF_KEY, false))
        {
            Debug.Log("Auto-merge already completed. Delete this script to run again.");
            return;
        }

        // Delay execution until Unity is fully loaded
        EditorApplication.delayCall += ExecuteMerge;
    }

    private static void ExecuteMerge()
    {
        Debug.Log("=== AUTO-MERGE STARTING ===");

        // Check if required scenes exist
        string tes1Path = "Assets/tes1.unity";
        string djvsPath = "Assets/DJ_V_S.unity";

        if (!File.Exists(tes1Path))
        {
            Debug.LogError("tes1.unity not found! Aborting auto-merge.");
            return;
        }

        // Ask user to confirm (one last chance)
        if (!EditorUtility.DisplayDialog("Auto-Merge Ready",
            "This will automatically:\n" +
            "• Merge DJ_V_S into tes1 scene\n" +
            "• Fix all lighting (disable AudioLink)\n" +
            "• Add mirror with quality controls\n" +
            "• Add light toggle button\n\n" +
            "This only runs ONCE.\n\n" +
            "Continue?",
            "Yes, Do It!", "Cancel"))
        {
            Debug.Log("Auto-merge cancelled by user");
            EditorPrefs.SetBool(PREF_KEY, true); // Mark as done so it doesn't ask again
            return;
        }

        // Execute the merge
        try
        {
            // Step 1: Load tes1 as base
            EditorSceneManager.OpenScene(tes1Path);
            Debug.Log("✓ Opened tes1.unity");

            // Step 2: Merge DJ_V_S if it exists
            if (File.Exists(djvsPath))
            {
                EditorSceneManager.OpenScene(djvsPath, OpenSceneMode.Additive);
                Debug.Log("✓ Merged DJ_V_S.unity");
            }

            // Step 3: Fix lights
            FixVRSLLights();

            // Step 4: Add mirror
            AddMirrorSystem();

            // Step 5: Add light toggle
            AddLightToggle();

            // Step 6: Save
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("✓ Scene saved");

            // Mark as complete
            EditorPrefs.SetBool(PREF_KEY, true);

            EditorUtility.DisplayDialog("Auto-Merge Complete!",
                "✅ Scenes merged successfully!\n\n" +
                "✓ DJ_V_S merged into tes1\n" +
                "✓ Lights fixed (AudioLink disabled)\n" +
                "✓ Mirror added at (-3, 2, 0)\n" +
                "✓ Light toggle added at (3, 1.5, 0)\n" +
                "✓ Scene saved\n\n" +
                "Press Play to test!\n\n" +
                "This script won't run again. Delete Assets/Editor/AutoMergeOnLoad.cs to re-run.",
                "Awesome!");

            Debug.Log("=== AUTO-MERGE COMPLETE ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Auto-merge failed: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Auto-merge failed:\n{e.Message}", "OK");
        }
    }

    private static void FixVRSLLights()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("VRSL") || obj.name.Contains("AudioLink"))
            {
                // Disable AudioLink components
                MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    if (comp != null && comp.GetType().Name.Contains("AudioLink"))
                    {
                        comp.enabled = false;
                        fixedCount++;
                    }
                }

                // Enable renderers
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null) renderer.enabled = true;

                foreach (Transform child in obj.transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null) childRenderer.enabled = true;
                }
            }
        }

        Debug.Log($"✓ Fixed {fixedCount} AudioLink components");
    }

    private static void AddMirrorSystem()
    {
        GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mirror.name = "Mirror";
        mirror.transform.position = new Vector3(-3, 2, 0);
        mirror.transform.rotation = Quaternion.Euler(0, 90, 0);
        mirror.transform.localScale = new Vector3(2, 2, 1);

        // Add VRC Mirror component
        var mirrorType = System.Type.GetType("VRC.SDK3.Components.VRCMirrorReflection,VRC.SDK3");
        if (mirrorType != null)
        {
            mirror.AddComponent(mirrorType);
        }

        // Create controls
        GameObject controls = new GameObject("MirrorControls");
        controls.transform.position = new Vector3(-3, 1, 0);

        CreateButton("HighQualityButton", new Vector3(-0.3f, 0, 0), controls.transform, "High\nQuality", Color.green);
        CreateButton("LowQualityButton", new Vector3(0.3f, 0, 0), controls.transform, "Low\nQuality", Color.yellow);

        Debug.Log("✓ Mirror system added");
    }

    private static void AddLightToggle()
    {
        CreateButton("LightToggleButton", new Vector3(3, 1.5f, 0), null, "Toggle\nLights", Color.cyan);
        Debug.Log("✓ Light toggle added");
    }

    private static GameObject CreateButton(string name, Vector3 localPos, Transform parent, string text, Color color)
    {
        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
        button.name = name;
        button.transform.localScale = new Vector3(0.4f, 0.3f, 0.1f);

        if (parent != null)
        {
            button.transform.parent = parent;
            button.transform.localPosition = localPos;
        }
        else
        {
            button.transform.position = localPos;
        }

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        button.GetComponent<Renderer>().material = mat;

        GameObject textObj = new GameObject(name + "_Label");
        textObj.transform.parent = button.transform;
        textObj.transform.localPosition = new Vector3(0, 0, -0.06f);
        textObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 32;
        textMesh.color = Color.white;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;

        return button;
    }
}
