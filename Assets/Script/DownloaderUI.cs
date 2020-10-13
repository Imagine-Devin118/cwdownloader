using FairyGUI;
using ImagineClass.Library;
using UnityEngine;

public class DownloaderUI : View<DownloaderState>
{
    protected override string[] ComponentNames { get; } = {
        "confirm", "input_url/input_value", "progress", "tip", "btn_file"
    };

    protected override string[] ControllerNames { get; } = {
        "page"
    };

    public DownloaderUI(Model<DownloaderState> model, GComponent rootUI) : base(model, rootUI)
    {
        components["confirm"].asButton.onClick.Set(() =>
        {
            var url = components["input_url/input_value"].asTextField.text;
            var availableUrl = CheckUri(url);
            var state = model.GetState();
            state.url = url;
            model.SetState(state);
            if (availableUrl)
            {
                state.clickConfirm.Invoke();
                state.page = "downloading";
                model.SetState(state);
            }
            else
            {
                PopUp.Toast(ToastEmoji.Info, "地址不合法");
            }
        });

        components["btn_file"].asButton.onClick.Set(() =>
        {
            Application.OpenURL(Application.persistentDataPath);
        });
    }

    public override void Render(DownloaderState data)
    {
        components["progress"].asProgress.value = data.progress;
        components["tip"].asTextField.text = data.tip;
        controllers["page"].selectedPage = data.page;
    }

    private bool CheckUri(string strUri)
    {
        try
        {
            System.Net.HttpWebRequest.Create(strUri).GetResponse();
            return true;
        }
        catch
        {
            return false;
        }
    }
}