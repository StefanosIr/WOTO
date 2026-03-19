using UnityEngine;
using UnityEngine.UI;

public static class StageHudBuilder
{
    private const string HudRootName = "_StageHud";

    public static void Build(FighterGameplay fighterA, FighterGameplay fighterB, MatchController match)
    {
        GameObject existing = GameObject.Find(HudRootName);
        if (existing != null)
        {
            Object.Destroy(existing);
        }

        GameObject canvasObject = new GameObject(HudRootName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(StageHudRuntime));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        HudWidgets leftHud = BuildFighterHud(canvasObject.transform, fighterA, new Vector2(0.18f, 0.885f), TextAnchor.MiddleLeft, true);
        HudWidgets rightHud = BuildFighterHud(canvasObject.transform, fighterB, new Vector2(0.82f, 0.885f), TextAnchor.MiddleRight, false);
        Text titleText = BuildHeader(canvasObject.transform, "Wrath of the Olympians");
        Text timerText = BuildCenterText(canvasObject.transform, new Vector2(0.5f, 0.855f), new Vector2(220f, 56f), 34, "60");
        Text roundText = BuildCenterText(canvasObject.transform, new Vector2(0.5f, 0.815f), new Vector2(260f, 40f), 24, "Round 1");
        Text overlayText = BuildCenterText(canvasObject.transform, new Vector2(0.5f, 0.56f), new Vector2(900f, 96f), 56, string.Empty);
        overlayText.color = new Color(0.98f, 0.94f, 0.82f, 1f);
        BuildCenterPlate(canvasObject.transform, new Vector2(0.5f, 0.885f), new Vector2(540f, 110f));
        Text controlsText = BuildCenterText(canvasObject.transform, new Vector2(0.5f, 0.055f), new Vector2(1200f, 30f), 18, "P1 A/D Move  Space Jump  Z Punch  X Kick  C Ultra       P2 J/L Move  O Jump  U Punch  P Kick  M Ultra");
        controlsText.color = new Color(0.88f, 0.89f, 0.94f, 0.95f);
        BuildBottomPlate(canvasObject.transform, new Vector2(0.5f, 0.055f), new Vector2(1320f, 42f));

        StageHudRuntime runtime = canvasObject.GetComponent<StageHudRuntime>();
        runtime.Initialize(fighterA, fighterB, match, leftHud, rightHud, titleText, timerText, roundText, overlayText, controlsText);
    }

    private static HudWidgets BuildFighterHud(Transform parent, FighterGameplay fighter, Vector2 anchor, TextAnchor alignment, bool leftAligned)
    {
        GameObject root = new GameObject(fighter.FighterName + "_HUD", typeof(RectTransform));
        root.transform.SetParent(parent, false);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(540f, 120f);

        Text nameText = CreateText(root.transform, fighter.FighterName, 36, alignment);
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.5f, 1f);
        nameRect.anchorMax = new Vector2(0.5f, 1f);
        nameRect.sizeDelta = new Vector2(520f, 40f);
        nameRect.anchoredPosition = Vector2.zero;

        Image frame = CreateImage(root.transform, "HealthFrame", new Color(0.08f, 0.08f, 0.12f, 0.88f));
        RectTransform frameRect = frame.rectTransform;
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.sizeDelta = new Vector2(520f, 38f);
        frameRect.anchoredPosition = new Vector2(0f, -20f);

        Image fill = CreateImage(frame.transform, "HealthFill", leftAligned ? new Color(0.92f, 0.78f, 0.24f, 1f) : new Color(0.9f, 0.34f, 0.2f, 1f));
        RectTransform fillRect = fill.rectTransform;
        fillRect.anchorMin = leftAligned ? new Vector2(0f, 0f) : new Vector2(1f, 0f);
        fillRect.anchorMax = leftAligned ? new Vector2(0f, 1f) : new Vector2(1f, 1f);
        fillRect.pivot = leftAligned ? new Vector2(0f, 0.5f) : new Vector2(1f, 0.5f);
        fillRect.sizeDelta = new Vector2(512f, -8f);
        fillRect.anchoredPosition = leftAligned ? new Vector2(4f, 0f) : new Vector2(-4f, 0f);

        Text hpText = CreateText(root.transform, "100 / 100", 20, alignment);
        RectTransform hpRect = hpText.GetComponent<RectTransform>();
        hpRect.anchorMin = new Vector2(0.5f, 0.5f);
        hpRect.anchorMax = new Vector2(0.5f, 0.5f);
        hpRect.sizeDelta = new Vector2(520f, 24f);
        hpRect.anchoredPosition = new Vector2(0f, -50f);

        Text ultraText = CreateText(root.transform, "Ultra Ready", 18, alignment);
        ultraText.color = new Color(0.74f, 0.9f, 1f, 1f);
        RectTransform ultraRect = ultraText.GetComponent<RectTransform>();
        ultraRect.anchorMin = new Vector2(0.5f, 0.5f);
        ultraRect.anchorMax = new Vector2(0.5f, 0.5f);
        ultraRect.sizeDelta = new Vector2(520f, 22f);
        ultraRect.anchoredPosition = new Vector2(0f, -72f);

        Image[] scorePips = new Image[2];
        for (int i = 0; i < scorePips.Length; i++)
        {
            Image pip = CreateImage(root.transform, "RoundPip_" + i, new Color(0.28f, 0.28f, 0.34f, 0.9f));
            RectTransform pipRect = pip.rectTransform;
            pipRect.anchorMin = new Vector2(0.5f, 1f);
            pipRect.anchorMax = new Vector2(0.5f, 1f);
            float x = leftAligned ? 180f + i * 30f : -180f - i * 30f;
            pipRect.sizeDelta = new Vector2(18f, 18f);
            pipRect.anchoredPosition = new Vector2(x, -8f);
            scorePips[i] = pip;
        }

        return new HudWidgets
        {
            HealthFill = fillRect,
            HealthText = hpText,
            UltraText = ultraText,
            ScorePips = scorePips
        };
    }

    private static void BuildCenterPlate(Transform parent, Vector2 anchor, Vector2 size)
    {
        Image panel = CreateImage(parent, "CenterPlate", new Color(0.06f, 0.06f, 0.09f, 0.58f));
        RectTransform rect = panel.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        panel.transform.SetAsFirstSibling();
    }

    private static void BuildBottomPlate(Transform parent, Vector2 anchor, Vector2 size)
    {
        Image panel = CreateImage(parent, "BottomPlate", new Color(0.04f, 0.04f, 0.06f, 0.52f));
        RectTransform rect = panel.rectTransform;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        panel.transform.SetAsFirstSibling();
    }

    private static Text BuildHeader(Transform parent, string content)
    {
        Text text = CreateText(parent, content, 30, TextAnchor.MiddleCenter);
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.94f);
        rect.anchorMax = new Vector2(0.5f, 0.94f);
        rect.sizeDelta = new Vector2(700f, 40f);
        return text;
    }

    private static Text BuildCenterText(Transform parent, Vector2 anchor, Vector2 size, int fontSize, string content)
    {
        Text text = CreateText(parent, content, fontSize, TextAnchor.MiddleCenter);
        RectTransform rect = text.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = size;
        return text;
    }

    private static Image CreateImage(Transform parent, string name, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(Image));
        imageObject.transform.SetParent(parent, false);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static Text CreateText(Transform parent, string content, int fontSize, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(content, typeof(Text));
        textObject.transform.SetParent(parent, false);
        Text text = textObject.GetComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = new Color(0.95f, 0.95f, 0.98f, 1f);
        text.supportRichText = false;
        return text;
    }

}

internal sealed class HudWidgets
{
    public RectTransform HealthFill;
    public Text HealthText;
    public Text UltraText;
    public Image[] ScorePips;
}

public sealed class StageHudRuntime : MonoBehaviour
{
    private FighterGameplay fighterA;
    private FighterGameplay fighterB;
    private MatchController match;
    private HudWidgets leftHud;
    private HudWidgets rightHud;
        private Text titleText;
        private Text timerText;
        private Text roundText;
        private Text overlayText;
        private Text controlsText;

    internal void Initialize(
        FighterGameplay leftFighter,
        FighterGameplay rightFighter,
        MatchController matchController,
        HudWidgets leftWidgets,
        HudWidgets rightWidgets,
        Text headerText,
        Text timerLabel,
        Text roundLabel,
        Text overlayLabel,
        Text controlsLabel)
    {
        fighterA = leftFighter;
        fighterB = rightFighter;
        match = matchController;
        leftHud = leftWidgets;
        rightHud = rightWidgets;
        titleText = headerText;
        timerText = timerLabel;
        roundText = roundLabel;
        overlayText = overlayLabel;
        controlsText = controlsLabel;
    }

    private void Update()
    {
        if (fighterA == null || fighterB == null)
        {
            return;
        }

        UpdateFighterHud(fighterA, leftHud, true, match != null ? match.ScoreA : 0);
        UpdateFighterHud(fighterB, rightHud, false, match != null ? match.ScoreB : 0);

        if (match == null)
        {
            titleText.text = "Olympus Duel";
            timerText.text = "Free Play";
            roundText.text = string.Empty;
            overlayText.text = string.Empty;
            controlsText.enabled = true;
            return;
        }

        titleText.text = "Wrath of the Olympians";
        timerText.text = Mathf.CeilToInt(match.TimerRemaining).ToString("00");
        roundText.text = "Round " + match.CurrentRound;
        overlayText.text = match.OverlayMessage;
        overlayText.enabled = !string.IsNullOrEmpty(match.OverlayMessage);
        controlsText.enabled = true;
    }

    private static void UpdateFighterHud(FighterGameplay fighter, HudWidgets hud, bool leftAligned, int score)
    {
        hud.HealthFill.sizeDelta = new Vector2(512f * fighter.HealthNormalized, -8f);
        hud.HealthText.text = Mathf.CeilToInt(fighter.CurrentHealth) + " / 100";
        hud.UltraText.text = fighter.UltraCooldownNormalized <= 0f ? "Ultra Ready" : "Ultra " + Mathf.CeilToInt(fighter.UltraCooldownNormalized * 100f) + "%";
        hud.UltraText.color = fighter.UltraCooldownNormalized <= 0f
            ? new Color(0.8f, 0.94f, 1f, 1f)
            : new Color(0.72f, 0.74f, 0.82f, 1f);

        for (int i = 0; i < hud.ScorePips.Length; i++)
        {
            hud.ScorePips[i].color = i < score
                ? (leftAligned ? new Color(0.92f, 0.78f, 0.24f, 1f) : new Color(0.9f, 0.34f, 0.2f, 1f))
                : new Color(0.28f, 0.28f, 0.34f, 0.9f);
        }
    }
}
