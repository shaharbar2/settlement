using UnityEditor;

[InitializeOnLoadAttribute]
public static class PlayRefreshEditor {
    static PlayRefreshEditor() {
        EditorApplication.playModeStateChanged += PlayRefresh;
    }

    private static void PlayRefresh(PlayModeStateChange state) {
        if (state == PlayModeStateChange.ExitingEditMode) {
            AssetDatabase.Refresh();
        }
    }
}