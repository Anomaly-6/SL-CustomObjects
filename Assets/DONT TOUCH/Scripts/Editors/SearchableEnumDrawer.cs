using System;
using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    [CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
    public sealed class SearchableEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "SearchableEnumAttribute works only with enum fields.");
                EditorGUI.EndProperty();
                return;
            }

            Rect fieldRect = EditorGUI.PrefixLabel(position, label);
            string displayedValue = property.hasMultipleDifferentValues ? "Mixed" : GetDisplayedValue(property);

            if (EditorGUI.DropdownButton(fieldRect, new GUIContent(displayedValue), FocusType.Keyboard,
                    EditorStyles.popup))
            {
                SearchableEnumPopup.Show(GUIUtility.GUIToScreenRect(fieldRect), property.serializedObject,
                    property.propertyPath);
            }

            EditorGUI.EndProperty();
        }

        private static string GetDisplayedValue(SerializedProperty property)
        {
            if (property.enumValueIndex < 0 || property.enumValueIndex >= property.enumDisplayNames.Length)
                return "None";

            return property.enumDisplayNames[property.enumValueIndex];
        }
    }

    internal sealed class SearchableEnumPopup : EditorWindow
    {
        private const float WindowWidth = 260f;
        private const float WindowHeight = 320f;
        private const float ItemHeight = 20f;
        private const float HeaderHeight = 24f;

        private SerializedObject _serializedObject;
        private string _propertyPath;
        private string _searchText = string.Empty;
        private Vector2 _scrollPosition;
        private string[] _displayNames;
        private string[] _searchTokens;
        [SerializeField] private bool _stylesInitialized;
        [SerializeField] private bool _isProSkin;
        [SerializeField] private Color _windowBackground;
        [SerializeField] private Color _headerBackground;
        [SerializeField] private Color _rowAltBackground;
        [SerializeField] private Color _rowHoverBackground;
        [SerializeField] private Color _rowSelectedBackground;
        [SerializeField] private GUIStyle _headerLabelStyle;
        [SerializeField] private GUIStyle _searchContainerStyle;
        [SerializeField] private GUIStyle _rowLabelStyle;
        [SerializeField] private GUIStyle _rowLabelSelectedStyle;
        [SerializeField] private GUIStyle _emptyLabelStyle;
        [SerializeField] private GUIStyle _searchPlaceholderStyle;

        public static void Show(Rect buttonRect, SerializedObject serializedObject, string propertyPath)
        {
            SearchableEnumPopup window = CreateInstance<SearchableEnumPopup>();
            window.hideFlags = HideFlags.HideAndDontSave;
            window._serializedObject = serializedObject;
            window._propertyPath = propertyPath;
            window.Initialize();
            float popupWidth = Mathf.Clamp(buttonRect.width, 160f, WindowWidth);
            window.ShowAsDropDown(buttonRect, new Vector2(popupWidth, WindowHeight));
            window.Focus();
        }

        private void Initialize()
        {
            SerializedProperty property = _serializedObject?.FindProperty(_propertyPath);
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Close();
                return;
            }

            int optionCount = property.enumDisplayNames.Length;
            _displayNames = new string[optionCount];
            _searchTokens = new string[optionCount];

            for (int i = 0; i < optionCount; i++)
            {
                _displayNames[i] = property.enumDisplayNames[i];
                _searchTokens[i] = property.enumNames[i];
            }

            _searchText = string.Empty;
            minSize = new Vector2(Mathf.Max(WindowWidth, 220f), 160f);
            maxSize = new Vector2(800f, WindowHeight);
        }

        private void OnGUI()
        {
            EnsureStyles();

            if (_serializedObject == null)
            {
                Close();
                return;
            }

            SerializedProperty property = _serializedObject.FindProperty(_propertyPath);
            if (property == null || property.propertyType != SerializedPropertyType.Enum)
            {
                Close();
                return;
            }

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            DrawBackground();
            DrawHeader(property);
            DrawSearchField();

            EditorGUILayout.Space(4f);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawOptions(property);
            EditorGUILayout.EndScrollView();

            HandleKeyboard();
        }

        private void EnsureStyles()
        {
            bool isProSkin = EditorGUIUtility.isProSkin;
            if (_stylesInitialized && _isProSkin == isProSkin)
                return;

            _stylesInitialized = true;
            _isProSkin = isProSkin;

            _windowBackground = isProSkin ? new Color(0.17f, 0.17f, 0.17f, 1f) : new Color(0.93f, 0.93f, 0.93f, 1f);
            _headerBackground = isProSkin ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.82f, 0.82f, 0.82f, 1f);
            _rowAltBackground = isProSkin ? new Color(0.20f, 0.20f, 0.20f, 1f) : new Color(0.97f, 0.97f, 0.97f, 1f);
            _rowHoverBackground = new Color(0.24f, 0.49f, 0.90f, 0.20f);
            _rowSelectedBackground = new Color(0.24f, 0.49f, 0.90f, 0.32f);

            _headerLabelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };

            _searchContainerStyle = new GUIStyle("HelpBox")
            {
                padding = new RectOffset(6, 6, 4, 4),
                margin = new RectOffset(4, 4, 0, 0)
            };

            _rowLabelStyle = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(8, 4, 2, 2)
            };
            _rowLabelSelectedStyle = new GUIStyle(_rowLabelStyle)
            {
                fontStyle = FontStyle.Bold
            };

            _emptyLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            _searchPlaceholderStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(15, 0, 0, 0),
                normal =
                {
                    textColor = isProSkin ? new Color(0.75f, 0.75f, 0.75f, 0.7f) : new Color(0.3f, 0.3f, 0.3f, 0.7f)
                }
            };
        }

        private void DrawBackground()
        {
            Rect backgroundRect = new Rect(0f, 0f, position.width, position.height);
            EditorGUI.DrawRect(backgroundRect, _windowBackground);
        }

        private void DrawHeader(SerializedProperty property)
        {
            Rect headerRect =
                GUILayoutUtility.GetRect(GUIContent.none, _headerLabelStyle, GUILayout.Height(HeaderHeight));
            EditorGUI.DrawRect(headerRect, _headerBackground);
            string headerText = $"{property.displayName} ({_displayNames.Length})";
            GUI.Label(headerRect, headerText, _headerLabelStyle);
        }

        private void DrawSearchField()
        {
            EditorGUILayout.BeginVertical(_searchContainerStyle);
            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName("SearchableEnumSearchField");
            Rect searchRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            _searchText = EditorGUI.TextField(searchRect, _searchText, EditorStyles.toolbarSearchField);
            if (EditorGUI.EndChangeCheck())
                Repaint();

            if (Event.current.type == EventType.Repaint)
                EditorGUI.FocusTextInControl("SearchableEnumSearchField");

            if (string.IsNullOrEmpty(_searchText) && Event.current.type == EventType.Repaint)
                GUI.Label(searchRect, "Search...", _searchPlaceholderStyle);
            EditorGUILayout.EndVertical();
        }

        private void DrawOptions(SerializedProperty property)
        {
            int visibleIndex = 0;

            for (int i = 0; i < _displayNames.Length; i++)
            {
                if (!MatchesSearch(i))
                    continue;

                bool isSelected = i == property.enumValueIndex;
                Rect optionRect =
                    GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, GUILayout.Height(ItemHeight));
                bool isHovered = optionRect.Contains(Event.current.mousePosition);

                if ((visibleIndex & 1) == 1)
                    EditorGUI.DrawRect(optionRect, _rowAltBackground);

                if (isHovered)
                    EditorGUI.DrawRect(optionRect, _rowHoverBackground);

                if (isSelected)
                    EditorGUI.DrawRect(optionRect, _rowSelectedBackground);

                GUI.Label(optionRect, _displayNames[i], isSelected ? _rowLabelSelectedStyle : _rowLabelStyle);

                if (GUI.Button(optionRect, GUIContent.none, GUIStyle.none))
                {
                    SelectIndex(property, i);
                    return;
                }

                visibleIndex++;
            }

            if (visibleIndex == 0)
            {
                Rect emptyRect =
                    GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label, GUILayout.Height(ItemHeight));
                EditorGUI.LabelField(emptyRect, "No matches", _emptyLabelStyle);
            }
        }

        private bool MatchesSearch(int index)
        {
            if (string.IsNullOrWhiteSpace(_searchText))
                return true;

            string search = _searchText.Trim();
            return _displayNames[index].IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   _searchTokens[index].IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SelectIndex(SerializedProperty property, int index)
        {
            _serializedObject.Update();
            Undo.RecordObjects(_serializedObject.targetObjects, $"Change {property.displayName}");
            property.enumValueIndex = index;
            _serializedObject.ApplyModifiedProperties();
            Close();
        }

        private void HandleKeyboard()
        {
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Escape)
            {
                currentEvent.Use();
                Close();
            }
        }
    }
}