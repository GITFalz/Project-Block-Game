using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Vector3CustomUI : MonoBehaviour
{
    public TMP_InputField xInput;
    public TMP_InputField yInput;
    public TMP_InputField zInput;
    
    public bool valueChanged = false;
    
    public void UpdateUI(Vector3 vector)
    {
        xInput.text = vector.x.ToString(CultureInfo.InvariantCulture);
        yInput.text = vector.y.ToString(CultureInfo.InvariantCulture);
        zInput.text = vector.z.ToString(CultureInfo.InvariantCulture);
    }
    
    public Vector3 GetVector3()
    {
        return new Vector3(float.Parse(xInput.text, CultureInfo.InvariantCulture), float.Parse(yInput.text, CultureInfo.InvariantCulture), float.Parse(zInput.text, CultureInfo.InvariantCulture));
    }

    public void OnValueChanged()
    {
        valueChanged = true;
    }
}