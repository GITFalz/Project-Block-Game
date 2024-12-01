using UnityEngine;

public class CustomUiUpdater : MonoBehaviour
{
    public int count;
    public GameObject go;

    private void Update()
    {
        if (count > 0)
        {
            count--;
            go.SetActive(false);
            go.SetActive(true);
        }
    }
}