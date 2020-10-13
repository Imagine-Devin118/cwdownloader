using FairyGUI;
using UnityEngine;

public class App : MonoBehaviour
{
    private void Start()
    {
        var com = GetComponent<UIPanel>().ui;
        var mediator = new Mediator(com);
    }
}