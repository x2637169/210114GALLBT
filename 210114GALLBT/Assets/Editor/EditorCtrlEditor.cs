using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorCtrl))]
public class EditorCtrlEditor : Editor
{
    EditorCtrl m_target;

    private bool[,] _slotpos = new bool[5, 3];
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        m_target = (EditorCtrl)target;

        EditorGUILayout.BeginHorizontal();
        _slotpos[0, 2] = EditorGUILayout.ToggleLeft("0,2", m_target.slotpos[0, 2], GUILayout.MaxWidth(50));
        m_target.slotpos[0, 2] = _slotpos[0, 2];
        _slotpos[1, 2] = EditorGUILayout.ToggleLeft("1,2", m_target.slotpos[1, 2], GUILayout.MaxWidth(50));
        m_target.slotpos[1, 2] = _slotpos[1, 2];
        _slotpos[2, 2] = EditorGUILayout.ToggleLeft("2,2", m_target.slotpos[2, 2], GUILayout.MaxWidth(50));
        m_target.slotpos[2, 2] = _slotpos[2, 2];
        _slotpos[3, 2] = EditorGUILayout.ToggleLeft("3,2", m_target.slotpos[3, 2], GUILayout.MaxWidth(50));
        m_target.slotpos[3, 2] = _slotpos[3, 2];
        _slotpos[4, 2] = EditorGUILayout.ToggleLeft("4,2", m_target.slotpos[4, 2], GUILayout.MaxWidth(50));
        m_target.slotpos[4, 2] = _slotpos[4, 2];
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _slotpos[0, 1] = EditorGUILayout.ToggleLeft("0,1", m_target.slotpos[0, 1], GUILayout.MaxWidth(50));
        m_target.slotpos[0, 1] = _slotpos[0, 1];
        _slotpos[1, 1] = EditorGUILayout.ToggleLeft("1,1", m_target.slotpos[1, 1], GUILayout.MaxWidth(50));
        m_target.slotpos[1, 1] = _slotpos[1, 1];
        _slotpos[2, 1] = EditorGUILayout.ToggleLeft("2,1", m_target.slotpos[2, 1], GUILayout.MaxWidth(50));
        m_target.slotpos[2, 1] = _slotpos[2, 1];
        _slotpos[3, 1] = EditorGUILayout.ToggleLeft("3,1", m_target.slotpos[3, 1], GUILayout.MaxWidth(50));
        m_target.slotpos[3, 1] = _slotpos[3, 1];
        _slotpos[4, 1] = EditorGUILayout.ToggleLeft("4,1", m_target.slotpos[4, 1], GUILayout.MaxWidth(50));
        m_target.slotpos[4, 1] = _slotpos[4, 1];
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _slotpos[0, 0] = EditorGUILayout.ToggleLeft("0,0", m_target.slotpos[0, 0], GUILayout.MaxWidth(50));
        m_target.slotpos[0, 0] = _slotpos[0, 0];
        _slotpos[1, 0] = EditorGUILayout.ToggleLeft("1,0", m_target.slotpos[1, 0], GUILayout.MaxWidth(50));
        m_target.slotpos[1, 0] = _slotpos[1, 0];
        _slotpos[2, 0] = EditorGUILayout.ToggleLeft("2,0", m_target.slotpos[2, 0], GUILayout.MaxWidth(50));
        m_target.slotpos[2, 0] = _slotpos[2, 0];
        _slotpos[3, 0] = EditorGUILayout.ToggleLeft("3,0", m_target.slotpos[3, 0], GUILayout.MaxWidth(50));
        m_target.slotpos[3, 0] = _slotpos[3, 0];
        _slotpos[4, 0] = EditorGUILayout.ToggleLeft("4,0", m_target.slotpos[4, 0], GUILayout.MaxWidth(50));
        m_target.slotpos[4, 0] = _slotpos[4, 0];
        EditorGUILayout.EndHorizontal();
    }
}
