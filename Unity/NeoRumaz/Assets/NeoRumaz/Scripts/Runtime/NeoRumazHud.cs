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
        private GameObject runHudRoot;
        private GameObject mainMenuPanel;
        private GameObject garagePanel;
        private Text menuCreditsText;
        private Text garageStatusText;
        private Button audioButton;
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
            runHudRoot = new GameObject("Run HUD", typeof(RectTransform));
            runHudRoot.transform.SetParent(canvasRoot.transform, false);
            scoreText = Label(Panel(runHudRoot.transform, "Score Panel", new Vector2(0.03f, 0.83f), new Vector2(0.22f, 0.14f), new Color(0.02f, 0.06f, 0.12f, 0.88f)), "SCORE\n0", 42, TextAnchor.MiddleCenter, Color.white);
            creditsText = Label(Panel(runHudRoot.transform, "Credits Panel", new Vector2(0.82f, 0.85f), new Vector2(0.15f, 0.12f), new Color(0.02f, 0.06f, 0.12f, 0.88f)), "N  248", 42, TextAnchor.MiddleCenter, new Color(0.72f, 0.96f, 1f));
            contractText = Label(Panel(runHudRoot.transform, "Contract Panel", new Vector2(0.03f, 0.75f), new Vector2(0.22f, 0.06f), new Color(0.03f, 0.07f, 0.13f, 0.78f)), "CAIRO CONTRACT  0 / 6", 19, TextAnchor.MiddleCenter, new Color(1f, 0.78f, 0.34f));
            boostText = Label(Panel(runHudRoot.transform, "Nile Rush Panel", new Vector2(0.40f, 0.89f), new Vector2(0.2f, 0.065f), new Color(0.02f, 0.22f, 0.34f, 0.86f)), "", 20, TextAnchor.MiddleCenter, new Color(0.43f, 0.94f, 1f));
            Button jump = Button(Panel(runHudRoot.transform, "Jump Button", new Vector2(0.84f, 0.08f), new Vector2(0.12f, 0.15f), new Color(0.05f, 0.1f, 0.18f, 0.92f)), "JUMP", 31, new Color(0.7f, 0.96f, 1f));
            jump.onClick.AddListener(game.RequestJump);
            Button left = Button(Panel(runHudRoot.transform, "Left Lane", new Vector2(0.07f, 0.11f), new Vector2(0.08f, 0.10f), new Color(0.04f, 0.1f, 0.17f, 0.88f)), "<", 30, Color.white);
            left.onClick.AddListener(delegate { game.RequestLane(-1); });
            Button right = Button(Panel(runHudRoot.transform, "Right Lane", new Vector2(0.16f, 0.11f), new Vector2(0.08f, 0.10f), new Color(0.04f, 0.1f, 0.17f, 0.88f)), ">", 30, Color.white);
            right.onClick.AddListener(delegate { game.RequestLane(1); });
            toastPanel = Panel(canvasRoot.transform, "Toast", new Vector2(0.34f, 0.66f), new Vector2(0.32f, 0.055f), new Color(0.02f, 0.10f, 0.16f, 0.93f));
            toastText = Label(toastPanel.transform, "", 20, TextAnchor.MiddleCenter, new Color(0.78f, 0.97f, 1f));
            toastPanel.SetActive(false);
            BuildGameOver(canvasRoot.transform);
            BuildMainMenu(canvasRoot.transform);
            BuildGarage(canvasRoot.transform);
            runHudRoot.SetActive(false);
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
            if (menuCreditsText != null) menuCreditsText.text = "N  " + game.Credits.ToString("N0") + "   |   HIGH SCORE  " + game.Profile.HighScore.ToString("N0");
            if (audioButton != null)
            {
                Text label = audioButton.transform.Find("Button Label").GetComponent<Text>();
                label.text = game.Profile.AudioEnabled ? "AUDIO  //  ON" : "AUDIO  //  OFF";
            }
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

        public void ShowRunHud()
        {
            mainMenuPanel.SetActive(false);
            garagePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            runHudRoot.SetActive(true);
        }

        public void ShowMainMenu()
        {
            runHudRoot.SetActive(false);
            garagePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            Refresh();
        }

        private void ShowGarage()
        {
            mainMenuPanel.SetActive(false);
            garagePanel.SetActive(true);
            RefreshGarage();
        }

        private void RefreshGarage()
        {
            garageStatusText.text = "CREDITS  N " + game.Credits.ToString("N0") + "\nSELECTED  " + game.SelectedCharacter.DisplayName;
            for (int index = 0; index < CharacterCatalog.Count; index++)
            {
                Transform node = garagePanel.transform.Find("Character " + index);
                if (node == null) continue;
                CharacterDefinition character = CharacterCatalog.Get(index);
                bool unlocked = game.Profile.UnlockedCharacterIndices.Contains(index);
                Text label = node.Find("Button Label").GetComponent<Text>();
                label.text = unlocked ? (game.Profile.SelectedCharacterIndex == index ? character.DisplayName + "  //  ACTIVE" : "SELECT  " + character.DisplayName) : "UNLOCK " + character.DisplayName + "  N " + character.UnlockCost;
            }
        }

        private void BuildMainMenu(Transform parent)
        {
            mainMenuPanel = Panel(parent, "Main Menu", new Vector2(0.56f, 0.14f), new Vector2(0.36f, 0.68f), new Color(0.01f, 0.035f, 0.08f, 0.9f));
            Text title = Label(mainMenuPanel.transform, "Title", "NEORUMAZ\nNILE CIRCUIT", 48, TextAnchor.MiddleCenter, new Color(0.72f, 0.96f, 1f));
            Stretch(title.rectTransform, new Vector2(0.08f, 0.70f), new Vector2(0.92f, 0.94f));
            Text tag = Label(mainMenuPanel.transform, "Tag", "THE CITY DOESN'T WAIT.", 20, TextAnchor.MiddleCenter, new Color(1f, 0.78f, 0.34f));
            Stretch(tag.rectTransform, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.7f));
            menuCreditsText = Label(mainMenuPanel.transform, "Profile", "", 20, TextAnchor.MiddleCenter, Color.white);
            Stretch(menuCreditsText.rectTransform, new Vector2(0.1f, 0.51f), new Vector2(0.9f, 0.6f));
            Button run = MakeActionButton(mainMenuPanel.transform, "Run", "START RUN", new Vector2(0.16f, 0.35f), new Vector2(0.84f, 0.45f), new Color(0.18f, 0.92f, 1f));
            run.onClick.AddListener(game.StartNewRun);
            Button garage = MakeActionButton(mainMenuPanel.transform, "Garage", "RUNNER GARAGE", new Vector2(0.16f, 0.23f), new Vector2(0.84f, 0.33f), new Color(1f, 0.3f, 0.85f));
            garage.onClick.AddListener(ShowGarage);
            audioButton = MakeActionButton(mainMenuPanel.transform, "Audio", "AUDIO", new Vector2(0.16f, 0.11f), new Vector2(0.84f, 0.21f), new Color(0.65f, 0.9f, 1f));
            audioButton.onClick.AddListener(ToggleAudio);
            Button reward = MakeActionButton(mainMenuPanel.transform, "Reward", "REWARDED AD  //  NOT CONFIGURED", new Vector2(0.16f, 0.01f), new Vector2(0.84f, 0.09f), new Color(1f, 0.78f, 0.34f));
            reward.onClick.AddListener(game.RequestDailyReward);
        }

        private void ToggleAudio()
        {
            game.SetAudio(!game.Profile.AudioEnabled);
            ShowToast(game.Profile.AudioEnabled ? "AUDIO ENABLED" : "AUDIO DISABLED");
            Refresh();
        }

        private void BuildGarage(Transform parent)
        {
            garagePanel = Panel(parent, "Runner Garage", new Vector2(0.14f, 0.12f), new Vector2(0.72f, 0.76f), new Color(0.01f, 0.035f, 0.08f, 0.94f));
            Text title = Label(garagePanel.transform, "Title", "RUNNER GARAGE", 42, TextAnchor.MiddleCenter, new Color(0.72f, 0.96f, 1f));
            Stretch(title.rectTransform, new Vector2(0.08f, 0.85f), new Vector2(0.92f, 0.96f));
            garageStatusText = Label(garagePanel.transform, "Status", "", 22, TextAnchor.MiddleCenter, Color.white);
            Stretch(garageStatusText.rectTransform, new Vector2(0.1f, 0.71f), new Vector2(0.9f, 0.84f));
            for (int index = 0; index < CharacterCatalog.Count; index++)
            {
                int capture = index;
                Button character = MakeActionButton(garagePanel.transform, "Character " + index, "", new Vector2(0.12f, 0.54f - index * 0.16f), new Vector2(0.88f, 0.66f - index * 0.16f), CharacterCatalog.Get(index).Accent);
                character.onClick.AddListener(delegate { SelectCharacter(capture); });
            }
            Button back = MakeActionButton(garagePanel.transform, "Back", "BACK TO ROUTE", new Vector2(0.3f, 0.04f), new Vector2(0.7f, 0.13f), new Color(0.65f, 0.9f, 1f));
            back.onClick.AddListener(ShowMainMenu);
            garagePanel.SetActive(false);
        }

        private void SelectCharacter(int index)
        {
            ShopResult result = game.TrySelectCharacter(index);
            if (result == ShopResult.UnlockedAndSelected) ShowToast("RUNNER UNLOCKED");
            else if (result == ShopResult.Selected) ShowToast("RUNNER SELECTED");
            else if (result == ShopResult.AlreadySelected) ShowToast("RUNNER ALREADY ACTIVE");
            else if (result == ShopResult.InsufficientCredits) ShowToast("NOT ENOUGH CREDITS");
            RefreshGarage();
            Refresh();
        }

        private void BuildGameOver(Transform parent)
        {
            gameOverPanel = Panel(parent, "Game Over", new Vector2(0.32f, 0.25f), new Vector2(0.36f, 0.48f), new Color(0.01f, 0.035f, 0.08f, 0.96f));
            Text summary = Label(gameOverPanel.transform, "Summary", "RUN INTERRUPTED", 37, TextAnchor.MiddleCenter, Color.white);
            Stretch(summary.rectTransform, new Vector2(0.12f, 0.38f), new Vector2(0.88f, 0.84f));
            Button retry = MakeActionButton(gameOverPanel.transform, "Retry", "RUN AGAIN", new Vector2(0.15f, 0.14f), new Vector2(0.85f, 0.28f), new Color(0.02f, 0.58f, 0.72f));
            retry.onClick.AddListener(game.RestartRun);
            Button menu = MakeActionButton(gameOverPanel.transform, "Menu", "RETURN TO MENU", new Vector2(0.15f, 0.04f), new Vector2(0.85f, 0.10f), new Color(0.65f, 0.9f, 1f));
            menu.onClick.AddListener(ShowMainMenu);
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

        private Button MakeActionButton(Transform parent, string title, string content, Vector2 min, Vector2 max, Color color)
        {
            GameObject panel = new GameObject(title, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            Image image = panel.GetComponent<Image>();
            image.color = new Color(color.r * 0.18f, color.g * 0.18f, color.b * 0.18f, 0.95f);
            Outline outline = panel.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(1.2f, -1.2f);
            Stretch(panel.GetComponent<RectTransform>(), min, max);
            return Button(panel, content, 22, color);
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
