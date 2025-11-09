using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class DJVenueAutoSetup : EditorWindow
{
    [MenuItem("Tools/Setup DJ Venue World")]
    public static void SetupDJVenue()
    {
        if (!EditorUtility.DisplayDialog("Setup DJ Venue",
            "This will open the VRChat default scene and automatically place all your DJ equipment. Continue?",
            "Yes, Set It Up!", "Cancel"))
        {
            return;
        }

        // Open the default VRChat scene
        string defaultScenePath = "Assets/Scenes/VRCDefaultWorldScene.unity";
        if (!File.Exists(defaultScenePath))
        {
            EditorUtility.DisplayDialog("Error", "VRCDefaultWorldScene not found!", "OK");
            return;
        }

        EditorSceneManager.OpenScene(defaultScenePath);

        // Find all DJ equipment models
        string modelsPath = "Assets/DJ_Equipment/Models";

        // DJ Equipment positions (adjust these as needed)
        AddModel("Pioneer_DJM_A9", new Vector3(0, 1.0f, 2), Quaternion.Euler(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f));
        AddModel("Pioneer_CDJ_3000_NXS_1", new Vector3(-0.5f, 1.0f, 2), Quaternion.Euler(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f));
        AddModel("Pioneer_CDJ_3000_NXS_2", new Vector3(0.5f, 1.0f, 2), Quaternion.Euler(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f));
        AddModel("Cables", new Vector3(0, 1.0f, 1.8f), Quaternion.Euler(0, 0, 0), new Vector3(0.01f, 0.01f, 0.01f));

        // Venue structure
        AddModel("Plane", new Vector3(0, 0, 0), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f)); // Floor
        AddModel("Plane_002", new Vector3(0, 3, 5), Quaternion.Euler(90, 0, 0), new Vector3(0.01f, 0.01f, 0.01f)); // Back wall
        AddModel("Cube_001", new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f)); // Side walls
        AddModel("Cylinder", new Vector3(0, 0, 3), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f));

        // Props
        AddModel("tabletop_tabletop_0", new Vector3(1.5f, 0.8f, 1.5f), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f));
        AddModel("frame_frame_0", new Vector3(1.5f, 0, 1.5f), Quaternion.identity, new Vector3(0.01f, 0.01f, 0.01f));

        // Save the scene
        EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog("Success!",
            "DJ Venue setup complete!\n\n" +
            "✓ All equipment placed\n" +
            "✓ Positions configured\n" +
            "✓ Scene saved\n\n" +
            "Press Play to test!",
            "Awesome!");
    }

    private static void AddModel(string modelName, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        string modelPath = $"Assets/DJ_Equipment/Models/{modelName}.fbx";

        GameObject modelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (modelPrefab == null)
        {
            Debug.LogWarning($"Model not found: {modelName}");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(modelPrefab);
        instance.name = modelName;
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        instance.transform.localScale = scale;

        // Add mesh collider to everything for collision
        MeshCollider collider = instance.GetComponent<MeshCollider>();
        if (collider == null)
        {
            // Check if it has child meshes
            MeshFilter[] meshFilters = instance.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.gameObject.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider mc = meshFilter.gameObject.AddComponent<MeshCollider>();
                    mc.convex = false;
                }
            }
        }

        Debug.Log($"Added {modelName} at {position}");
    }
}
