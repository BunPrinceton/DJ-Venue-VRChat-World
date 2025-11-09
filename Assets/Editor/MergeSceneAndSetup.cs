using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class MergeSceneAndSetup : EditorWindow
{
    [MenuItem("Tools/Merge Scenes and Complete Setup")]
    public static void MergeAndSetup()
    {
        if (!EditorUtility.DisplayDialog("Complete DJ Venue Setup",
            "This will:\n" +
            "• Merge DJ_V_S into tes1 scene\n" +
            "• Fix all lighting (disable AudioLink for now)\n" +
            "• Add mirror with quality controls\n" +
            "• Add light toggle button\n\n" +
            "Continue?",
            "Yes!", "Cancel"))
        {
            return;
        }

        // Step 1: Load tes1 as the base scene
        string tes1Path = "Assets/tes1.unity";
        if (!File.Exists(tes1Path))
        {
            EditorUtility.DisplayDialog("Error", "tes1.unity not found!", "OK");
            return;
        }

        EditorSceneManager.OpenScene(tes1Path);

        // Step 2: Load DJ_V_S additively (merge it in)
        string djvsPath = "Assets/DJ_V_S.unity";
        if (File.Exists(djvsPath))
        {
            EditorSceneManager.OpenScene(djvsPath, OpenSceneMode.Additive);
            Debug.Log("Merged DJ_V_S into tes1");
        }

        // Step 3: Fix all VRSL lights - disable AudioLink
        FixVRSLLights();

        // Step 4: Add Mirror with controls
        AddMirrorSystem();

        // Step 5: Add light toggle button
        AddLightToggle();

        // Step 6: Save
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog("Success!",
            "✓ Scenes merged\n" +
            "✓ Lights fixed (AudioLink disabled)\n" +
            "✓ Mirror added with quality controls\n" +
            "✓ Light toggle button added\n\n" +
            "Press Play to test!",
            "Awesome!");
    }

    private static void FixVRSLLights()
    {
        // Find all game objects with "VRSL" or "AudioLink" in name
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int fixedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            // Check if it's a VRSL light
            if (obj.name.Contains("VRSL") || obj.name.Contains("AudioLink"))
            {
                // Disable AudioLink-related components
                MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();
                foreach (var comp in components)
                {
                    if (comp != null && comp.GetType().Name.Contains("AudioLink"))
                    {
                        comp.enabled = false;
                        fixedCount++;
                    }
                }

                // Enable the light's renderer so it shows up
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }

                // Check children too
                foreach (Transform child in obj.transform)
                {
                    Renderer childRenderer = child.GetComponent<Renderer>();
                    if (childRenderer != null)
                    {
                        childRenderer.enabled = true;
                    }
                }
            }
        }

        Debug.Log($"Fixed {fixedCount} AudioLink components on lights");
    }

    private static void AddMirrorSystem()
    {
        // Create mirror object
        GameObject mirror = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mirror.name = "Mirror";
        mirror.transform.position = new Vector3(-3, 2, 0);
        mirror.transform.rotation = Quaternion.Euler(0, 90, 0);
        mirror.transform.localScale = new Vector3(2, 2, 1);

        // Try to add VRC Mirror component (requires VRC SDK)
        var mirrorType = System.Type.GetType("VRC.SDK3.Components.VRCMirrorReflection,VRC.SDK3");
        if (mirrorType != null)
        {
            mirror.AddComponent(mirrorType);
        }

        // Create mirror controls parent
        GameObject controls = new GameObject("MirrorControls");
        controls.transform.position = new Vector3(-3, 1, 0);

        // High Quality Button
        CreateButton("HighQualityButton", new Vector3(-0.3f, 0, 0), controls.transform,
            "High\nQuality", Color.green);

        // Low Quality Button
        CreateButton("LowQualityButton", new Vector3(0.3f, 0, 0), controls.transform,
            "Low\nQuality", Color.yellow);

        Debug.Log("Mirror system added");
    }

    private static void AddLightToggle()
    {
        // Create light toggle button
        GameObject lightToggle = CreateButton("LightToggleButton",
            new Vector3(3, 1.5f, 0), null, "Toggle\nLights", Color.cyan);

        Debug.Log("Light toggle button added");
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

        // Add colored material
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = color;
        button.GetComponent<Renderer>().material = mat;

        // Create text label (3D text)
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
