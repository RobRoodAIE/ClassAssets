using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

class SOWindow : EditorWindow
{
    public class AssetType
    {
        public string typeName = "";
        public string displayName = "";
        public bool showObjs = false;
        public List<ScriptableObject> assets = new List<ScriptableObject>();
    }

    Vector2 _typeScrollViewPosition;
    List<AssetType> assetTypes = new List<AssetType>();
    public bool showFullName = true;

    void OnEnable()
    {
        GetTypes();
    }

    void OnFocus()
    {
        GetTypes();
    }

    [MenuItem("Tools/Scriptable Object Browser")]
    static void ShowWindow()
    {
        GetWindow<SOWindow>("Scriptable Objects");
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Scriptable Object Types", EditorStyles.largeLabel);
        showFullName = EditorGUILayout.Toggle("Full Name", showFullName);
        if (GUI.changed)
            GetTypes();

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh List"))
            GetTypes();

        _typeScrollViewPosition = GUILayout.BeginScrollView(_typeScrollViewPosition);

        for (int i = 0; i < assetTypes.Count; i++)
        {
            assetTypes[i].showObjs = EditorGUILayout.BeginFoldoutHeaderGroup(assetTypes[i].showObjs, assetTypes[i].displayName);

            if (assetTypes[i].showObjs)
            {
                for (int j = 0; j < assetTypes[i].assets.Count; j++)
                {
                    if (GUILayout.Button("\t" + GetNiceName(assetTypes[i].assets[j].name), EditorStyles.linkLabel))
                        Selection.activeObject = assetTypes[i].assets[j];
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        GUILayout.EndScrollView();
    }

    AssetType HaveType(List<AssetType> atypes, string tname)
    {
        for (int i = 0; i < atypes.Count; i++)
        {
            if (atypes[i].typeName == tname)
                return atypes[i];
        }

        return null;
    }

    static int SortTypes(AssetType t1, AssetType t2)
    {
        return System.String.Compare(t1.displayName, t2.displayName);
    }

    static int SortAssets(ScriptableObject t1, ScriptableObject t2)
    {
        return System.String.Compare(t1.name, t2.name);
    }

    void GetTypes()
    {
        string[] GUIDs = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { "Assets" });
        ScriptableObject[] SOs = new ScriptableObject[GUIDs.Length];

        for (int i = 0; i < GUIDs.Length; i++)
            SOs[i] = (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(GUIDs[i]), typeof(ScriptableObject));

        assetTypes.Clear();

        for (int i = 0; i < SOs.Length; i++)
        {
            AssetType atype = HaveType(assetTypes, SOs[i].GetType().FullName);

            if (atype == null)
            {
                atype = new AssetType();
                atype.typeName = SOs[i].GetType().FullName;
                if (showFullName)
                    atype.displayName = GetNiceName(SOs[i].GetType().FullName);
                else
                    atype.displayName = GetNiceName(SOs[i].GetType().Name);

                assetTypes.Add(atype);
            }

            if (!atype.assets.Contains(SOs[i]))
                atype.assets.Add(SOs[i]);
        }

        assetTypes.Sort(SortTypes);

        for (int i = 0; i < assetTypes.Count; i++)
            assetTypes[i].assets.Sort(SortAssets);
    }

    string GetNiceName(string text)
    {
        string niceText = ObjectNames.NicifyVariableName(text);
        niceText = niceText.Replace(" SO", "");
        niceText = niceText.Replace("-", " ");
        niceText = niceText.Replace("_", " ");
        return niceText;
    }
}

