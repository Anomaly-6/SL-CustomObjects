using UnityEditor;
using UnityEngine;

namespace DONT_TOUCH.Scripts.Editors
{
    public class ActionInfoWindow : EditorWindow
    {
        private sealed class WikiSection
        {
            public readonly GUIContent TitleContent;
            public readonly GUIContent[] Lines;

            public WikiSection(string title, string iconName, params string[] lines)
            {
                Texture icon = EditorGUIUtility.IconContent(iconName).image;
                TitleContent = new GUIContent(title, icon);

                Lines = new GUIContent[lines.Length];

                for (int i = 0; i < lines.Length; i++)
                    Lines[i] = new GUIContent("- " + lines[i]);
            }
        }

        private readonly struct LanguagePack
        {
            public readonly string ReadOnlyTitle;
            public readonly string ReadOnlyMessage;
            public readonly WikiSection[] Sections;

            public LanguagePack(
                string readOnlyTitle,
                string readOnlyMessage,
                WikiSection[] sections)
            {
                ReadOnlyTitle = readOnlyTitle;
                ReadOnlyMessage = readOnlyMessage;
                Sections = sections;
            }
        }

        private static readonly string[] LanguageTabs =
        {
            "English",
            "Русский",
        };

        private static readonly LanguagePack[] Languages =
        {
            new LanguagePack(
                "Read Only",
                "This window is informational only. To update the wiki, edit ActionInfoWindow.cs in code.",
                new[]
                {
                    new WikiSection(
                        "Introduction",
                        "console.infoicon.sml",
                        "These actions allow you to create simple interactions with the game without needing to create a plugin.",
                        "Actions are executed in the order they are listed, from top to bottom.",
                        "If the action you need is missing, contact the developer. If you know how to code, consider creating a pull request in the repository."),

                    new WikiSection(
                        "Special Tags",
                        "console.infoicon.sml",
                        "These tags can be used when executing the <b>Command</b> action.",
                        "<b>[p_id]</b> or <b>{p_id}</b>: The ID of the player who triggered the event."),

                    new WikiSection(
                        "Available Actions",
                        "console.infoicon.sml",
                        "<b>Command:</b> Executes a command through the game's Remote Admin panel.",
                        "<b>Animation:</b> Changes animation parameters.",
                        "<b>Set Component Property:</b> Changes a property on another object's component.",
                        "<b>Destroy:</b> Deletes an object."),

                    new WikiSection(
                        "Trigger Events",
                        "d_PlayButton",
                        "<b>On Enter:</b> Runs when a player enters the trigger.",
                        "<b>On Exit:</b> Runs when a player exits the trigger.",
                        "<b>While Inside:</b> Runs while a player remains inside the trigger."),

                    new WikiSection(
                        "Interactable Events",
                        "d_PlayButton",
                        "<b>On Interacted:</b> Runs when the player interacts with the Interactable. Used when <b>InteractionDuration</b> = 0.",
                        "<b>On Searching:</b> Runs when the player begins interacting. Used when <b>InteractionDuration</b> > 0.",
                        "<b>On Searched:</b> Runs when the player finishes interacting. Used when <b>InteractionDuration</b> > 0.",
                        "<b>On Search Aborted:</b> Runs when the player cancels the interaction. Used when <b>InteractionDuration</b> > 0."),
                }),

            new LanguagePack(
                "Только для чтения",
                "Это окно предназначено только для информации. Чтобы обновить справку, измените ActionInfoWindow.cs в коде.",
                new[]
                {
                    new WikiSection(
                        "Вводная информация",
                        "console.infoicon.sml",
                        "Данные действия позволяют делать простые взаимодействия с игрой без необходимости создания плагина.",
                        "Действия вызываются в том порядке, в котором они расположены, сверху вниз.",
                        "Если нужного вам действия нет, напишите разработчику. Если вы умеете писать код, лучше создайте Pull Request в репозитории."),

                    new WikiSection(
                        "Спец. тэги",
                        "console.infoicon.sml",
                        "Данные тэги можно использовать при выполнении действия <b>Command</b>.",
                        "<b>[p_id]</b> или <b>{p_id}</b>: ID игрока, запустившего событие."),

                    new WikiSection(
                        "Доступные действия",
                        "console.infoicon.sml",
                        "<b>Command:</b> вызывает команду в админ-панели игры.",
                        "<b>Animation:</b> изменяет параметры анимации.",
                        "<b>Set Component Property:</b> изменяет свойство компонента другого объекта.",
                        "<b>Destroy:</b> удаляет объект."),

                    new WikiSection(
                        "Trigger события",
                        "d_PlayButton",
                        "<b>On Enter:</b> срабатывает, когда игрок входит в триггер.",
                        "<b>On Exit:</b> срабатывает, когда игрок выходит из триггера.",
                        "<b>While Inside:</b> срабатывает, пока игрок находится внутри триггера."),

                    new WikiSection(
                        "Interactable события",
                        "d_PlayButton",
                        "<b>On Interacted:</b> игрок нажимает на Interactable. Работает при <b>InteractionDuration</b> = 0.",
                        "<b>On Searching:</b> игрок начинает взаимодействие с Interactable. Работает при <b>InteractionDuration</b> > 0.",
                        "<b>On Searched:</b> игрок заканчивает взаимодействие с Interactable. Работает при <b>InteractionDuration</b> > 0.",
                        "<b>On Search Aborted:</b> игрок отменяет взаимодействие с Interactable. Работает при <b>InteractionDuration</b> > 0."),
                }),
        };

        private static readonly GUIContent HeroTitle =
            new GUIContent("Action Info");

        private static readonly GUIContent HeroSubtitle =
            new GUIContent("Read-only wiki for actions");

        private int _languageTab;
        private Vector2 _scroll;

        private bool _styleSkin;

        private GUIStyle _heroTitleStyle;
        private GUIStyle _heroSubtitleStyle;
        private GUIStyle _cardStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _lineStyle;

        private LanguagePack CurrentLanguage
        {
            get
            {
                int index = Mathf.Clamp(_languageTab, 0, Languages.Length - 1);
                return Languages[index];
            }
        }

        [MenuItem("SchematicManager/Action Info")]
        public static void OpenWindow()
        {
            ActionInfoWindow window =
                GetWindow<ActionInfoWindow>("Action Info");

            window.minSize = new Vector2(560f, 420f);
            window.Show();
        }

        public static bool DrawOpenButton(
            string buttonText = "Open Action Info",
            params GUILayoutOption[] options)
        {
            if (!GUILayout.Button(buttonText, options))
                return false;

            OpenWindow();
            return true;
        }

        private void OnEnable()
        {
            RebuildStyles();
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawWindowBackground();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawHero();

            _languageTab = GUILayout.Toolbar(
                _languageTab,
                LanguageTabs);

            GUILayout.Space(6f);

            LanguagePack language = CurrentLanguage;

            DrawReadOnlyNotice(language);

            WikiSection[] sections = language.Sections;

            for (int i = 0; i < sections.Length; i++)
                DrawSection(sections[i]);

            EditorGUILayout.EndScrollView();
        }

        private void EnsureStyles()
        {
            if (_heroTitleStyle == null ||
                _styleSkin != EditorGUIUtility.isProSkin)
            {
                RebuildStyles();
            }
        }

        private void RebuildStyles()
        {
            _styleSkin = EditorGUIUtility.isProSkin;

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

            _heroSubtitleStyle.normal.textColor =
                new Color(0.92f, 0.95f, 1f, 1f);

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
            Color backgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0.1f, 0.12f, 0.17f, 1f)
                : new Color(0.93f, 0.95f, 1f, 1f);

            EditorGUI.DrawRect(
                new Rect(0f, 0f, position.width, position.height),
                backgroundColor);
        }

        private void DrawHero()
        {
            Rect heroRect = GUILayoutUtility.GetRect(
                10f,
                50f,
                GUILayout.ExpandWidth(true));

            Color heroColor = EditorGUIUtility.isProSkin
                ? new Color(0.2f, 0.27f, 0.42f, 0.95f)
                : new Color(0.38f, 0.54f, 0.86f, 0.9f);

            EditorGUI.DrawRect(heroRect, heroColor);

            Rect titleRect = new Rect(
                heroRect.x + 10f,
                heroRect.y + 8f,
                heroRect.width - 20f,
                24f);

            Rect subtitleRect = new Rect(
                heroRect.x + 10f,
                heroRect.y + 23f,
                heroRect.width - 20f,
                30f);

            EditorGUI.LabelField(
                titleRect,
                HeroTitle,
                _heroTitleStyle);

            EditorGUI.LabelField(
                subtitleRect,
                HeroSubtitle,
                _heroSubtitleStyle);

            GUILayout.Space(6f);
        }

        private void DrawReadOnlyNotice(LanguagePack language)
        {
            GUILayout.BeginVertical(_cardStyle);

            EditorGUILayout.LabelField(
                language.ReadOnlyTitle,
                _sectionTitleStyle);

            EditorGUILayout.LabelField(
                language.ReadOnlyMessage,
                _lineStyle);

            GUILayout.EndVertical();
        }

        private void DrawSection(WikiSection section)
        {
            GUILayout.BeginVertical(_cardStyle);

            EditorGUILayout.LabelField(
                section.TitleContent,
                _sectionTitleStyle);

            GUIContent[] lines = section.Lines;

            for (int i = 0; i < lines.Length; i++)
                DrawWrappedLine(lines[i]);

            GUILayout.EndVertical();
        }

        private void DrawWrappedLine(GUIContent content)
        {
            float availableWidth =
                Mathf.Max(140f, position.width - 72f);

            float requiredHeight =
                _lineStyle.CalcHeight(content, availableWidth);

            Rect lineRect = EditorGUILayout.GetControlRect(
                false,
                requiredHeight);

            EditorGUI.LabelField(
                lineRect,
                content,
                _lineStyle);
        }
    }
}
