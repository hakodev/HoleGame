using Alteruna;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class RegisterUID : MonoBehaviour
{
    [MenuItem("Window/Alteruna/Reregister UID in current scene")]
    private static void ReregisterUIDsForAllObjects()
    {
        var synchronizables = FindObjectsByType<Synchronizable>(FindObjectsSortMode.None).ToList();
        synchronizables.ForEach(x =>
        {
            x.OverrideUID(System.Guid.NewGuid());

            EditorUtility.SetDirty(x);
        });

        var attributeSyncs = FindObjectsByType<AttributesSync>(FindObjectsSortMode.None).ToList();
        attributeSyncs.ForEach(x =>
        {
            x.OverrideUID(System.Guid.NewGuid());

            EditorUtility.SetDirty(x);
        });

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
