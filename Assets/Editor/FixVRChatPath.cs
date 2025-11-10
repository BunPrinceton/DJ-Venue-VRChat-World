using UnityEngine;
using UnityEditor;
using System.IO;

public class FixVRChatPath : EditorWindow
{
    [MenuItem("Tools/Fix VRChat Build & Test Path")]
    public static void FixPath()
    {
        string vrchatPath = @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\VRChat.exe";

        if (!File.Exists(vrchatPath))
        {
            // Try alternate locations
            string[] possiblePaths = new string[]
            {
                @"C:\Program Files\Steam\steamapps\common\VRChat\VRChat.exe",
                @"D:\Steam\steamapps\common\VRChat\VRChat.exe",
                @"E:\Steam\steamapps\common\VRChat\VRChat.exe"
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    vrchatPath = path;
                    break;
                }
            }
        }

        if (File.Exists(vrchatPath))
        {
            // Set the VRChat client path in Unity preferences
            EditorPrefs.SetString("VRC_installedClientPath", vrchatPath);

            EditorUtility.DisplayDialog("VRChat Path Fixed!",
                $"✅ VRChat found and configured!\n\n" +
                $"Path: {vrchatPath}\n\n" +
                "Now try Build & Test again:\n\n" +
                "1. VRChat SDK → Show Control Panel\n" +
                "2. Builder tab\n" +
                "3. Build & Test\n" +
                "4. VRChat should launch automatically!",
                "Got It!");

            Debug.Log($"VRChat path set to: {vrchatPath}");
        }
        else
        {
            EditorUtility.DisplayDialog("VRChat Not Found",
                "VRChat.exe not found in common locations.\n\n" +
                "Please install VRChat from Steam:\n" +
                "1. Open Steam\n" +
                "2. Search 'VRChat'\n" +
                "3. Install (FREE)\n" +
                "4. Run this tool again",
                "OK");
        }
    }
}
