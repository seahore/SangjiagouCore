using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(StackedBar))]
public class StackedBarEditor : Editor
{
    SerializedProperty _texture;
    SerializedProperty _animatorController;
    //SerializedProperty _enter;
    //SerializedProperty _hover;
    //SerializedProperty _exit;
    //SerializedProperty _notHover;
    SerializedProperty _onEnterBar;
    SerializedProperty _items;
    ReorderableList _stackedBarItemArray;

    void OnEnable()
    {
        _texture = serializedObject.FindProperty("Texture");
        _animatorController = serializedObject.FindProperty("AnimatorController");
        //_enter = serializedObject.FindProperty("Enter");
        //_hover = serializedObject.FindProperty("Hover");
        //_exit = serializedObject.FindProperty("Exit");
        //_notHover = serializedObject.FindProperty("NotHover");
        _onEnterBar = serializedObject.FindProperty("OnEnterBar");
        _items = serializedObject.FindProperty("Items");

        _stackedBarItemArray = new ReorderableList(serializedObject, _items, true, true, true, true);
        _stackedBarItemArray.drawHeaderCallback = rect => GUI.Label(rect, "Stack Bar Items");
        _stackedBarItemArray.elementHeight = 46;
        _stackedBarItemArray.drawElementCallback = (rect, index, selected, focused) => {
            SerializedProperty item = _stackedBarItemArray.serializedProperty.GetArrayElementAtIndex(index);
            rect.height -= 4;
            rect.y += 2;
            EditorGUI.PropertyField(rect, item, new GUIContent("Index" + index));
        };
        _stackedBarItemArray.onAddCallback = list => {
            _items.InsertArrayElementAtIndex(_items.arraySize);
            var item = list.serializedProperty.GetArrayElementAtIndex(_items.arraySize - 1);
            item.FindPropertyRelative("_name").stringValue = "New item";
            item.FindPropertyRelative("_color").colorValue = Color.white;
            item.FindPropertyRelative("_value").floatValue = 0;
            list.Select(_items.arraySize - 1);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_texture);
        EditorGUILayout.PropertyField(_animatorController);
        //EditorGUILayout.PropertyField(_enter);
        //EditorGUILayout.PropertyField(_hover);
        //EditorGUILayout.PropertyField(_exit);
        //EditorGUILayout.PropertyField(_notHover);
        EditorGUILayout.PropertyField(_onEnterBar);
        _stackedBarItemArray.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}


[CustomPropertyDrawer(typeof(StackedBar.StackedBarItem))]
public class StackBarItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property)) {
            EditorGUIUtility.labelWidth = 40;
            position.height = EditorGUIUtility.singleLineHeight;

            var nameRect = new Rect(position);
            var colorRect = new Rect(nameRect) { width = position.width / 2 - 5, y = nameRect.y + EditorGUIUtility.singleLineHeight + 5 };
            var valueRect = new Rect(colorRect) { x = colorRect.x + colorRect.width + 5 };

            var nameProp = property.FindPropertyRelative("_name");
            var colorProp = property.FindPropertyRelative("_color");
            var valueProp = property.FindPropertyRelative("_value");

            nameProp.stringValue = EditorGUI.TextField(nameRect, nameProp.displayName, nameProp.stringValue);
            colorProp.colorValue = EditorGUI.ColorField(colorRect, colorProp.displayName, colorProp.colorValue);
            valueProp.floatValue = EditorGUI.FloatField(valueRect, valueProp.displayName, valueProp.floatValue);
        }
    }
}