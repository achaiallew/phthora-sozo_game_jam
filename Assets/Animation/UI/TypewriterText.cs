using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using TextElement = UnityEngine.UIElements.TextElement;

[RequireComponent(typeof(UIDocument))]
public class TypewriterText : MonoBehaviour
{
    
    [SerializeField] private string labelName = "DialogueLabel"; // optional, matches a UXML Label
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private FontAsset font;
    [SerializeField] private int fontSize = 48;
    [SerializeField] private Vector2 anchoredPosition = new Vector2(50, 50); // left, top in px

    private Label label;
    private float elapsed;
    private float startRealtime;
    private IVisualElementScheduledItem animationJob;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        label = root.Q<Label>(labelName);
        if (label == null)
        {
            label = new Label();
            root.Add(label);
        }

        // Positioning — absolute, pinned relative to the panel
        label.style.position = Position.Absolute;
        label.style.left = anchoredPosition.x;
        label.style.top = anchoredPosition.y;

        // Font (SDF)
        if (font != null)
            label.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromSDFFont(font));

        label.style.fontSize = fontSize;
        label.style.unityTextAlign = TextAnchor.UpperLeft;
        label.style.color = new StyleColor(Color.white);

        label.PostProcessTextVertices += OnPostProcessTextVertices;

        const int targetHz = 60;
        animationJob = label.schedule.Execute(UpdateTime).Every(1000 / targetHz);
        animationJob.Pause();
    }

    private void OnDisable()
    {
        if (label != null)
            label.PostProcessTextVertices -= OnPostProcessTextVertices;
        animationJob?.Pause();
    }

    /// <summary>Call this to reveal new text on screen.</summary>
    public void PlayText(string text)
    {
        label.text = text;
        elapsed = 0f;
        startRealtime = Time.realtimeSinceStartup;
        label.MarkDirtyRepaint(); // pass at elapsed=0 so it starts fully hidden
        animationJob.Resume();
    }

    private void UpdateTime()
    {
        elapsed = Mathf.Min(Time.realtimeSinceStartup - startRealtime, animationDuration);
        if (elapsed >= animationDuration)
        {
            elapsed = animationDuration;
            animationJob.Pause();
        }
        label.MarkDirtyRepaint();
    }

    private void OnPostProcessTextVertices(TextElement.GlyphsEnumerable glyphs)
    {
        int glyphsToShow = (int)(elapsed * glyphs.Count / animationDuration);
        int index = 0;
        foreach (TextElement.Glyph glyph in glyphs)
        {
            bool visible = index < glyphsToShow;
            var verts = glyph.vertices;
            for (int i = 0; i < verts.Length; i++)
            {
                var v = verts[i];
                var tint = v.tint;
                tint.a = visible ? (byte)255 : (byte)0;
                v.tint = tint;
                verts[i] = v;
            }
            index++;
        }
    }
}