using System;
using System.IO;
using System.Runtime.InteropServices;
using FairyGUI;
using ImagineClass.Library;
using UnityEngine;

public class DownloaderUI : View<DownloaderState>
{
    protected override string[] ComponentNames { get; } = {
        "confirm", "input_url", "input_url/input_value", "progress", "tip", "btn_file"
    };

    protected override string[] ControllerNames { get; } = {
        "page"
    };

    private string filePath;

    public DownloaderUI(Model<DownloaderState> model, GComponent rootUI) : base(model, rootUI)
    {
        components["confirm"].asButton.onClick.Set(() =>
        {
            var state = model.GetState();
            if (File.Exists(state.filePath))
            {
                state.clickConfirm.Invoke();
                state.page = "downloading";
                model.SetState(state);
            }
            else
            {
                PopUp.Toast(ToastEmoji.Info, "选择本地资源包压缩文件");
            }
        });

        components["input_url"].asButton.onClick.Set(() =>
        {
            var s = model.GetState();
            var filePath = OpenFileDialog();
            s.filePath = filePath;
            long fileSize = 0;
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                fileSize = fileInfo.Length;
            }
            var extInfo = fileSize > 0 ? HumanReadableFilesize(fileSize) : "";
            s.tip = $"选中的文件:{filePath}[{extInfo}]";
            model.SetState(s);
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

    public string OpenFileDialog()
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "all files";// (*.zip)|*.zip
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;
        pth.title = "选择本地资源包压缩文件(Textbooks.zip)";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (global::OpenFileDialog.GetOpenFileName(pth))
        {
            return pth.file;
        }
        return "";
    }

    public string HumanReadableFilesize(long size)
    {
        var num = 1024.00;

        if (size < num)
            return size + "B";
        if (size < Math.Pow(num, 2))
            return (size / num).ToString("f2") + "KB";
        if (size < Math.Pow(num, 3))
            return (size / Math.Pow(num, 2)).ToString("f2") + "M";
        if (size < Math.Pow(num, 4))
            return (size / Math.Pow(num, 3)).ToString("f2") + "G";

        return (size / Math.Pow(num, 4)).ToString("f2") + "T";
    }
}