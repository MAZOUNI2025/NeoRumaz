// Cairo Night Runner style reminder: functional English HUD, angular transit instrumentation, cyan action hierarchy and amber rewards.
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NeoRumaz.Runtime
{
    public sealed class NeoRumazHud : MonoBehaviour
    {
        private NightRunnerGame game;
        private Text scoreText;
        private Text creditsText;
        private Text contractText;
        private Text boostText;
        private Text toastText;
        private GameObject toastPanel;
        private GameObject gameOverPanel;
        private float toastSeconds;

        public void Configure(NightRunnerGame runner)
        {
            game = runner;
            if (FindObjectOfType<EventSystem>() == null) new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            var canvasRoot = new GameObject("NeoRumaz HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scoreText = Label(Panel(canvasRoot.transform, "Score Panel", new Vector2(0.03f, 0.83f), new Vector2(0.22f, 0.14f), new Color(0.02f, 0.06f, 0.12f, 0.88f)), "SCORE\n0", 42, TextAnchor.MiddleCenter, Color.white);
            creditsText = Label(Panel(canvasRoot.transform, "Credits Panel", new Vector2(0.82f, 0.85f), new Vector2(0.15f, 0.12f), new Color(0.02f, 0.06f, 0.12f, 0.88f)), "N  248", 42, TextAnchor.MiddleCenter, new Color(0.72f, 0.96f, 1f));
            contractText = Label(Panel(canvasRoot.transform, "Contract Panel", new Vector2(0.03f, 0.75f), new Vector2(0.22f, 0.06f), new Color(0.03f, 0.07f, 0.13f, 0.78f)), "CAIRO CONTRACT  0 / 6", 19, TextAnchor.MiddleCenter, new Color(1f, 0.78f, 0.34f));
            boostText = Label(Panel(canvasRoot.transform, "Nile Rush Panel", new Vector2(0.40f, 0.89f), new Vector2(0.2f, 0.065f), new Color(0.02f, 0.22f, 0.34f, 0.86f)), "", 20, TextAnchor.MiddleCenter, new Color(0.43f, 0.94f, 1f));
            Button jump = Button(Panel(canvasRoot.transform, "Jump Button", new Vector2(0.84f, 0.08f), new Vector2(0.12f, 0.15f), new Color(0.05f, 0.1f, 0.18f, 0.92f)), "JUMP", 31, new Color(0.7f, 0.96f, 1f));
            jump.onClick.AddListener(game.RequestJump);
            Button left = Button(Panel(canvasRoot.transform, "Left Lane", new Vector2(0.07f, 0.11f), new Vector2(0.08f, 0.10f), new Color(0.04f, 0.1f, 0.17f, 0.88f)), "<", 30, Color.white);
            left.onClick.AddListener(delegate { game.RequestLane(-1); });
            Button right = Button(Panel(canvasRoot.transform, "Right Lane", new Vector2(0.16f, 0.11f), new Vector2(0.08f, 0.10f), new Color(0.04f, 0.1f, 0.17f, 0.88f)), ">", 30, Color.white);
            right.onClick.AddListener(delegate { game.RequestLane(1); });
            toastPanel = Panel(canvasRoot.transform, "Toast", new Vector2(0.34f, 0.66f), new Vector2(0.32f, 0.055f), new Color(0.02f, 0.10f, 0.16f, 0.93f));
            toastText = Label(toastPanel.transform, "", 20, TextAnchor.MiddleCenter, new Color(0.78f, 0.97f, 1f));
            toastPanel.SetActive(false);
            BuildGameOver(canvasRoot.transform);
        }

        private void Update()
        {
            if (toastSeconds <= 0f) return;
            toastSeconds -= Time.deltaTime;
            if (toastSeconds <= 0f) toastPanel.SetActive(false);
        }

        public void Refresh()
        {
            scoreText.text = "SCORE\n" + game.Score.ToString("N0");
            creditsText.text = "N  " + game.Credits.ToString("N0");
            contractText.text = "CAIRO CONTRACT  " + game.ContractCredits + " / " + game.ContractGoal;
            boostText.text = game.NileRushSeconds > 0f ? "NILE RUSH  " + game.NileRushSeconds.ToString("0.0") + "s" : game.ShieldSeconds > 0f ? "SCARAB SHIELD  " + game.ShieldSeconds.ToString("0.0") + "s" : "";
        }

        public void ShowToast(string message)
        {
            toastText.text = message;
            toastSeconds = 2.1f;
            toastPanel.SetActive(true);
        }

        public void ShowGameOver(int score, int credits)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.Find("Summary").GetComponent<Text>().text = "RUN INTERRUPTED\n\nSCORE  " + score.ToString("N0") + "\nCREDITS  " + credits.ToString("N0");
        }

        public void HideGameOver()
        {
            gameOverPanel.SetActive(false);
        }

        private void BuildGameOver(Transform parent)
        {
            gameOverPanel = Panel(parent, "Game Over", new Vector2(0.32f, 0.25f), new Vector2(0.36f, 0.48f), new Color(0.01f, 0.035f, 0.08f, 0.96f));
            Text summary = Label(gameOverPanel.transform, "Summary", "RUN INTERRUPTED", 37, TextAnchor.MiddleCenter, Color.white);
            Stretch(summary.rectTransform, new Vector2(0.12f, 0.38f), new Vector2(0.88f, 0.84f));
            Button retry = Button(gameOverPanel, "RUN AGAIN", 26, new Color(0.02f, 0.58f, 0.72f));
            Stretch(retry.GetComponent<RectTransform>(), new Vector2(0.15f, 0.12f), new Vector2(0.85f, 0.28f));
            retry.onClick.AddListener(game.RestartRun);
            gameOverPanel.SetActive(false);
        }

        private GameObject Panel(Transform parent, string title, Vector2 anchor, Vector2 size, Color color)
        {
            GameObject panel = new GameObject(title, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor + size;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = panel.GetComponent<Image>();
            image.color = color;
            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = new Color(0.2f, 0.92f, 1f, 0.78f);
            outline.effectDistance = new Vector2(1.2f, -1.2f);
            return panel;
        }

        private Text Label(GameObject parent, string content, int size, TextAnchor alignment, Color color)
        {
            return Label(parent.transform, "Label", content, size, alignment, color);
        }

        private Text Label(Transform parent, string title, string content, int size, TextAnchor alignment, Color color)
        {
            GameObject label = new GameObject(title, typeof(RectTransform), typeof(Text));
            label.transform.SetParent(parent, false);
            Text text = label.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.text = content;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            Stretch(label.GetComponent<RectTransform>(), new Vector2(0.08f, 0.1f), new Vector2(0.92f, 0.9f));
            return text;
        }

        private Button Button(GameObject panel, string title, int size, Color color)
        {
            Button button = panel.AddComponent<Button>();
            Text label = Label(panel.transform, "Button Label", title, size, TextAnchor.MiddleCenter, color);
            ColorBlock block = button.colors;
            block.normalColor = Color.white;
            block.highlightedColor = new Color(0.5f, 0.95f, 1f);
            block.pressedColor = new Color(0.2f, 0.7f, 0.8f);
            button.colors = block;
            return button;
        }

        private void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
