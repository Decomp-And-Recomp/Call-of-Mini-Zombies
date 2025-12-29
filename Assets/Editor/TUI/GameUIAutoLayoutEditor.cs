using UnityEditor;

namespace TUIEditor
{
    [CustomEditor(typeof(GameUIAutoLayout))]
    public class GameUIAutoLayoutEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var target = this.target as GameUIAutoLayout;

            if (target.bottom && target.top) 
                EditorGUILayout.HelpBox("It is recommended you DONT use 'top' and 'bottom' at the same time", MessageType.Warning);

            if (target.left && target.right) 
                EditorGUILayout.HelpBox("It is recommended you DONT use 'left' and 'right' at the same time", MessageType.Warning);

            int count = 0;

            if (target.left) count++;
            if (target.right) count++;
            if (target.top) count++;
            if (target.bottom) count++;

            if (count > 2)
                EditorGUILayout.HelpBox("It is recommended you have only 2 anchors selected at the same time.", MessageType.Warning);

            base.OnInspectorGUI();
        }
    }
}