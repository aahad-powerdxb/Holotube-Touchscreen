using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text;

public class HierarchyDumper : MonoBehaviour
{
    [MenuItem("Tools/Copy Hierarchy to Clipboard")]
    static void CopyHierarchy()
    {
        StringBuilder sb = new StringBuilder();
        Scene scene = SceneManager.GetActiveScene();

        // Get all root objects
        GameObject[] roots = scene.GetRootGameObjects();

        foreach (GameObject root in roots)
        {
            Traverse(root, sb, "");
        }

        // Copy to clipboard
        GUIUtility.systemCopyBuffer = sb.ToString();
        Debug.Log("Hierarchy copied to clipboard! You can paste it now.");
    }

    static void Traverse(GameObject obj, StringBuilder sb, string indentation)
    {
        // Add current object to list
        sb.AppendLine(indentation + "- " + obj.name);

        // Recursive call for children
        foreach (Transform child in obj.transform)
        {
            Traverse(child.gameObject, sb, indentation + "  ");
        }
    }
}