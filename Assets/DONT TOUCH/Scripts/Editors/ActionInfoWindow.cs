using UnityEditor;
using UnityEngine;

public class ActionInfoWindow : EditorWindow
{
    private readonly struct WikiSection
    {
        public readonly string Title;
        public readonly string IconName;
        public readonly string[] Lines;

        public WikiSection(string title, string iconName, params string[] lines)
        {
            Title = title;
            IconName = iconName;
            Lines = lines;
        }
    }

    private static readonly WikiSection[] Sections =
    {
        new (
            "Вводная информация",
            "console.infoicon.sml",
            "Данные действия позволяют делать простые взаимодействия с игрой без обходимости создания плагина",
            "Действия вызывается в том порядке в котором они расположены (сверух вниз)",
            "Если нужного вам действия нету, то напишите мне об этом. (Если вы умеете писать код, то лучше сделайте Pull request в репозитории)"
            ),
        new(
            "Доступные действия",
            "console.infoicon.sml",
            "<b>Command:</b> вызывает команду в админ панели игры.",
            "<b>Animation:</b> изменяет параметры анимации.",
            "<b>Audio:</b> включает указанный звук. (нужно указать имя файла)"),

        new(
            "Trigger события",
            "d_PlayButton",
            "<b>On Enter:</b> срабатывает когда игрок входит в триггер",
            "<b>On Exit:</b> срабатывает когда игрок выходит из триггера",
            "<b>While Inside:</b> срабатывает пока игрок внутри триггера"),

        new(
            "Animation Rules",
            "Animation Icon",
            "Target must have Animator (or child Animator).",
            "Param list is pulled from animator parameters.",
            "Trigger param type: Value is ignored.",
            "Bool param type: Value is False or True.",
            "Int param type: Value is integer.",
            "Float param type: Value is float.")
    };

    private Vector2 _scroll;
    private GUIStyle _heroTitleStyle;
    private GUIStyle _heroSubtitleStyle;
    private GUIStyle _cardStyle;
    private GUIStyle _sectionTitleStyle;
    private GUIStyle _lineStyle;
    
    [MenuItem("SchematicManager/Action Info")]
    public static void OpenWindow()
    {
        ActionInfoWindow window = GetWindow<ActionInfoWindow>("Action Info");
        window.minSize = new Vector2(560f, 420f);
        window.Show();
    }

    public static bool DrawOpenButton(string buttonText = "Open Action Info", params GUILayoutOption[] options)
    {
        if (!GUILayout.Button(buttonText, options))
            return false;

        OpenWindow();
        return true;
    }

    private void OnEnable()
    {
        EnsureStyles();
    }

    private void OnGUI()
    {
        EnsureStyles();
        DrawWindowBackground();

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        DrawHero();
        DrawReadOnlyNotice();

        foreach (WikiSection section in Sections)
            DrawSection(section);
        
        EditorGUILayout.EndScrollView();
    }

    private void EnsureStyles()
    {
        if (_heroTitleStyle != null)
            return;

        _heroTitleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 19,
            richText = true,
        };
        _heroTitleStyle.normal.textColor = Color.white;

        _heroSubtitleStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 11,
            wordWrap = true,
            richText = true,
        };
        _heroSubtitleStyle.normal.textColor = new Color(0.92f, 0.95f, 1f, 1f);

        _cardStyle = new GUIStyle("HelpBox")
        {
            margin = new RectOffset(8, 8, 6, 6),
            padding = new RectOffset(12, 12, 10, 10),
        };

        _sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            richText = true,
        };

        _lineStyle = new GUIStyle(EditorStyles.label)
        {
            wordWrap = true,
            richText = true,
        };
    }

    private void DrawWindowBackground()
    {
        Color bottom = EditorGUIUtility.isProSkin
            ? new Color(0.1f, 0.12f, 0.17f, 1f)
            : new Color(0.93f, 0.95f, 1f, 1f);

        Rect full = new Rect(0f, 0f, position.width, position.height);

        EditorGUI.DrawRect(full, bottom);
    }

    private void DrawHero()
    {
        Rect hero = GUILayoutUtility.GetRect(10f, 50f, GUILayout.ExpandWidth(true));

        Color heroColor = EditorGUIUtility.isProSkin
            ? new Color(0.2f, 0.27f, 0.42f, 0.95f)
            : new Color(0.38f, 0.54f, 0.86f, 0.9f);
        EditorGUI.DrawRect(hero, heroColor);

        Rect titleRect = new Rect(hero.x + 10f, hero.y + 8f, hero.width - 20f, 24f);
        Rect subtitleRect = new Rect(hero.x + 10f, hero.y + 23f, hero.width - 20f, 30f);

        EditorGUI.LabelField(titleRect, "Action Info", _heroTitleStyle);
        EditorGUI.LabelField(subtitleRect, "Read-only wiki for actions", _heroSubtitleStyle);

        GUILayout.Space(6f);
    }

    private void DrawReadOnlyNotice()
    {
        GUILayout.BeginVertical(_cardStyle);
        EditorGUILayout.LabelField("Read Only", _sectionTitleStyle);
        EditorGUILayout.LabelField(
            "This window is informational only. To update the wiki, edit ActionInfoWindow.cs in code.",
            _lineStyle);
        GUILayout.EndVertical();
    }

    private void DrawSection(WikiSection section)
    {
        GUILayout.BeginVertical(_cardStyle);

        Texture icon = EditorGUIUtility.IconContent(section.IconName)?.image;
        GUIContent title = new GUIContent(section.Title, icon);
        EditorGUILayout.LabelField(title, _sectionTitleStyle);

        for (int i = 0; i < section.Lines.Length; i++)
            DrawWrappedBullet(section.Lines[i]);

        GUILayout.EndVertical();
    }

    private void DrawWrappedBullet(string text)
    {
        string bullet = "- " + text;
        float width = Mathf.Max(140f, position.width - 72f);
        float height = _lineStyle.CalcHeight(new GUIContent(bullet), width);
        Rect lineRect = EditorGUILayout.GetControlRect(false, height);
        EditorGUI.LabelField(lineRect, bullet, _lineStyle);
    }
}
