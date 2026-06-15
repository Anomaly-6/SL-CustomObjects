using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
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
            new(
                "Introductory Information",
                "console.infoicon.sml",
                "These actions allow simple interactions with the game without the need to create a plugin",
                "Actions are called in the order they are placed (top to bottom)",
                "If the action you need is missing, let me know. (If you know how to code, it's better to make a Pull Request in the repository)"
            ),

            new(
                "Special Tags",
                "console.infoicon.sml",
                "These tags can be used when executing the <b>command</b> action",
                "<b>[p_id]</b> or <b>{p_id}</b>: ID of the player who triggered the event"
            ),

            new(
                "Available Actions",
                "console.infoicon.sml",
                "<b>Command:</b> calls a command in the game's admin panel.",
                "<b>Animation:</b> changes animation parameters.",
                "<b>Set Component Property:</b> interaction with other objects",
                "<b>Destroy:</b> deletes the object"),

            new(
                "Trigger Events",
                "d_PlayButton",
                "<b>On Enter:</b> triggers when a player enters the trigger",
                "<b>On Exit:</b> triggers when a player exits the trigger",
                "<b>While Inside:</b> triggers while the player is inside the trigger"),

            new(
                "Interactable Events",
                "d_PlayButton",
                "<b>On Interacted:</b> player clicks on the Interactable (works when <b>InteractionDuration</b> = 0)",
                "<b>On Searching:</b> player starts interacting with the Interactable (works when <b>InteractionDuration</b> > 0)",
                "<b>On Searched:</b> player finished interacting with the Interactable (works when <b>InteractionDuration</b> > 0)",
                "<b>On Search Aborted:</b> player cancelled interaction with the Interactable (works when <b>InteractionDuration</b> > 0)"),
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

}