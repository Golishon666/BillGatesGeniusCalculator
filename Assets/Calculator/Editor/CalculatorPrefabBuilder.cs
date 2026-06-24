using System.IO;
using BillGatesGeniusCalculator.Calculator.Unity;
using BillGatesGeniusCalculator.Calculator.Unity.Views;
using BillGatesGeniusCalculator.MessageBox;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BillGatesGeniusCalculator.Calculator.Editor
{
    public static class CalculatorPrefabBuilder
    {
        private const string PrefabFolder = "Assets/Calculator/Prefabs";
        private const string ScenePath = "Assets/Scenes/SampleScene.unity";
        private static readonly Color Blue = new Color32(76, 174, 238, 255);
        private static readonly Color LightLine = new Color32(210, 210, 210, 255);
        private static readonly Color TextDark = new Color32(35, 35, 35, 255);
        private static readonly Color Placeholder = new Color32(150, 150, 150, 255);

        [MenuItem("Tools/Calculator/Rebuild Prefabs And Scene")]
        public static void RebuildPrefabsAndScene()
        {
            Directory.CreateDirectory(PrefabFolder);
            var historyPrefab = BuildHistoryItemPrefab();
            var screenPrefab = BuildCalculatorScreenPrefab(historyPrefab);
            var messageBoxPrefab = BuildMessageBoxPrefab();
            BuildCalculatorAppPrefab(screenPrefab, messageBoxPrefab);
            BuildScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static HistoryItemView BuildHistoryItemPrefab()
        {
            var root = CreateRect("HistoryItem", null, Vector2.zero, Vector2.one, new Vector2(1f, 0.5f), Vector2.zero, new Vector2(430f, 42f));
            var label = AddText(root, "Label", "5+5=10", 31, FontStyle.Normal, TextAnchor.MiddleRight, Blue);
            Stretch(label.rectTransform);
            var view = root.AddComponent<HistoryItemView>();
            SetSerialized(view, "label", label);
            return SavePrefab<HistoryItemView>(root, $"{PrefabFolder}/HistoryItem.prefab");
        }

        private static CalculatorScreenView BuildCalculatorScreenPrefab(HistoryItemView historyItemPrefab)
        {
            var root = CreateRect("CalculatorScreen", null, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520f, 300f));
            var panel = root.GetComponent<RectTransform>();
            AddImage(root, Color.white);

            var title = AddText(root, "Title", "CALCULATOR PRO \u00AE", 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Color32(65, 65, 65, 255));
            title.rectTransform.anchorMin = new Vector2(0f, 1f);
            title.rectTransform.anchorMax = new Vector2(1f, 1f);
            title.rectTransform.pivot = new Vector2(0.5f, 1f);
            title.rectTransform.anchoredPosition = new Vector2(0f, -24f);
            title.rectTransform.sizeDelta = new Vector2(-70f, 34f);

            var inputRoot = CreateRect("InputField", root.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(-78f, 54f));
            var inputText = AddText(inputRoot, "Text", string.Empty, 30, FontStyle.Normal, TextAnchor.MiddleLeft, TextDark);
            inputText.supportRichText = false;
            Stretch(inputText.rectTransform, 8f, 6f, 8f, 6f);
            var placeholder = AddText(inputRoot, "Placeholder", "Enter an equation...", 30, FontStyle.Italic, TextAnchor.MiddleLeft, Placeholder);
            Stretch(placeholder.rectTransform, 8f, 6f, 8f, 6f);
            var input = inputRoot.AddComponent<InputField>();
            input.textComponent = inputText;
            input.placeholder = placeholder;
            input.caretColor = Blue;
            input.selectionColor = new Color(Blue.r, Blue.g, Blue.b, 0.35f);
            input.lineType = InputField.LineType.SingleLine;

            var line = CreateRect("InputLine", root.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -145f), new Vector2(-78f, 3f));
            AddImage(line, LightLine);

            var historyScroll = CreateRect("HistoryScroll", root.transform, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -154f), new Vector2(-78f, -246f));
            var scrollRect = historyScroll.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 28f;

            var viewport = CreateRect("Viewport", historyScroll.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            Stretch(viewport.GetComponent<RectTransform>());
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            AddImage(viewport, new Color(1f, 1f, 1f, 0.001f));

            var content = CreateRect("Content", viewport.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(0f, 0f));
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperRight;
            layout.childControlHeight = false;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;
            layout.spacing = 1f;
            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = content.GetComponent<RectTransform>();

            var buttonRoot = CreateRect("ResultButton", root.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(-78f, 72f));
            AddImage(buttonRoot, Blue);
            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = buttonRoot.GetComponent<Image>();
            var buttonLabel = AddText(buttonRoot, "Label", "RESULT", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(buttonLabel.rectTransform);

            var view = root.AddComponent<CalculatorScreenView>();
            SetSerialized(view, "panel", panel);
            SetSerialized(view, "titleText", title);
            SetSerialized(view, "inputField", input);
            SetSerialized(view, "placeholderText", placeholder);
            SetSerialized(view, "resultButton", button);
            SetSerialized(view, "resultButtonLabel", buttonLabel);
            SetSerialized(view, "historyContent", content.GetComponent<RectTransform>());
            SetSerialized(view, "historyScrollRect", scrollRect);
            SetSerialized(view, "historyItemPrefab", historyItemPrefab);

            return SavePrefab<CalculatorScreenView>(root, $"{PrefabFolder}/CalculatorScreen.prefab");
        }

        private static MessageBoxView BuildMessageBoxPrefab()
        {
            var root = CreateRect("MessageBox", null, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var overlay = AddImage(root, new Color(0f, 0f, 0f, 0.001f));
            overlay.raycastTarget = true;

            var panel = CreateRect("Panel", root.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(520f, 280f));
            AddImage(panel, Color.white);

            var message = AddText(panel, "Message", "Please check the expression\nyou just entered", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.black);
            message.rectTransform.anchorMin = new Vector2(0f, 1f);
            message.rectTransform.anchorMax = new Vector2(1f, 1f);
            message.rectTransform.pivot = new Vector2(0.5f, 1f);
            message.rectTransform.anchoredPosition = new Vector2(0f, -48f);
            message.rectTransform.sizeDelta = new Vector2(-60f, 92f);

            var buttonRoot = CreateRect("GotItButton", panel.transform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 42f), new Vector2(-70f, 72f));
            AddImage(buttonRoot, Blue);
            var button = buttonRoot.AddComponent<Button>();
            button.targetGraphic = buttonRoot.GetComponent<Image>();
            var label = AddText(buttonRoot, "Label", "GOT IT", 30, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
            Stretch(label.rectTransform);

            var view = root.AddComponent<MessageBoxView>();
            SetSerialized(view, "root", root);
            SetSerialized(view, "messageText", message);
            SetSerialized(view, "confirmButton", button);
            SetSerialized(view, "confirmButtonLabel", label);

            return SavePrefab<MessageBoxView>(root, $"{PrefabFolder}/MessageBox.prefab");
        }

        private static void BuildCalculatorAppPrefab(CalculatorScreenView screenPrefab, MessageBoxView messageBoxPrefab)
        {
            var root = new GameObject("CalculatorApp");
            var appView = root.AddComponent<CalculatorAppView>();

            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(root.transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 1f;

            var background = CreateRect("BlackBackground", canvasObject.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            Stretch(background.GetComponent<RectTransform>());
            AddImage(background, Color.black);

            var screen = (GameObject)PrefabUtility.InstantiatePrefab(screenPrefab.gameObject, canvasObject.transform);
            screen.name = "CalculatorScreen";
            var screenRect = screen.GetComponent<RectTransform>();
            screenRect.anchorMin = new Vector2(0.5f, 0.5f);
            screenRect.anchorMax = new Vector2(0.5f, 0.5f);
            screenRect.pivot = new Vector2(0.5f, 0.5f);
            screenRect.anchoredPosition = Vector2.zero;

            var messageBox = (GameObject)PrefabUtility.InstantiatePrefab(messageBoxPrefab.gameObject, canvasObject.transform);
            messageBox.name = "MessageBox";
            Stretch(messageBox.GetComponent<RectTransform>());

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.SetParent(root.transform, false);

            SetSerialized(appView, "screenView", screen.GetComponent<CalculatorScreenView>());
            SetSerialized(appView, "messageBoxView", messageBox.GetComponent<MessageBoxView>());
            SavePrefab<CalculatorAppView>(root, $"{PrefabFolder}/CalculatorApp.prefab");
        }

        private static void BuildScene()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "CalculatorApp")
                {
                    Object.DestroyImmediate(root);
                }
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/CalculatorApp.prefab");
            PrefabUtility.InstantiatePrefab(prefab, scene);

            if (Object.FindFirstObjectByType<Camera>() == null)
            {
                var camera = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
                camera.tag = "MainCamera";
                camera.transform.position = new Vector3(0f, 1f, -10f);
            }

            if (Object.FindFirstObjectByType<Light>() == null)
            {
                var light = new GameObject("Directional Light", typeof(Light));
                light.GetComponent<Light>().type = LightType.Directional;
                light.transform.rotation = Quaternion.Euler(50f, 330f, 0f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            var rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            return gameObject;
        }

        private static Image AddImage(GameObject target, Color color)
        {
            var image = target.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static Text AddText(GameObject parent, string name, string value, int size, FontStyle style, TextAnchor alignment, Color color)
        {
            var gameObject = CreateRect(name, parent.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            var text = gameObject.AddComponent<Text>();
            text.text = value;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static void Stretch(RectTransform rectTransform, float left = 0f, float top = 0f, float right = 0f, float bottom = 0f)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        private static T SavePrefab<T>(GameObject root, string path) where T : Component
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            return prefab.GetComponent<T>();
        }

        private static void SetSerialized(Object target, string propertyName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            serializedObject.FindProperty(propertyName).objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

    }
}
