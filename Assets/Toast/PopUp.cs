using FairyGUI;
using ImagineClass.Library;
using ImagineClass.Toast;

public class PopUp
{
    private static PopUp Instance = null;
    private GComponent root;
    private Model<ToastModel> model;
    private ToastView view;

    static PopUp()
    {
        Init();
    }

    public static void Init()
    {
        UIPackage.AddPackage("Toast");
        Instance = null;
        Instance = new PopUp
        {
            root = UIPackage.CreateObject("Toast", "main").asCom,
            model = new Model<ToastModel>()
        };
        Instance.view = new ToastView(Instance.model, Instance.root);
    }

    public static void Toast(ToastEmoji emoji, string contentText)
    {
        GRoot.inst.AddChild(Instance.root);
        Instance.root.Center();
        var toast = new ToastModel()
        {
            content = contentText,
            emoji = emoji
        };

        Instance.InitilzeModel(toast);
    }

    public static void Toast(string contentText)
    {
        Toast(ToastEmoji.None, contentText);
    }

    private void InitilzeModel(ToastModel toast)
    {
        var state = Instance.model.GetState();
        state = toast;
        Instance.model.SetState(state);
    }
}