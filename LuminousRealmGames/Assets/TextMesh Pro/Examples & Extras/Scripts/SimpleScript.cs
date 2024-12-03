using UnityEngine;
using TMPro;

public class GlowOnMouseClick : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Material textMaterial;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textMaterial = textComponent.fontMaterial;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Linker muisknop
        {
            ToggleGlow(true);
        }

        if (Input.GetMouseButtonUp(0)) // Loslaten muisknop
        {
            ToggleGlow(false);
        }
    }

    void ToggleGlow(bool enable)
    {
        if (textMaterial != null)
        {
            textMaterial.SetFloat(ShaderUtilities.ID_GlowPower, enable ? 1.0f : 0.0f);
        }
    }
}
