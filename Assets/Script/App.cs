using FairyGUI;
using UnityEngine;

public class App : MonoBehaviour
{
    private void Start()
    {
        ICSharpCode.SharpZipLib.Zip.ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
        var com = GetComponent<UIPanel>().ui;
        var mediator = new Mediator(com);
    }
}