using ImagineClass.Library;
using FairyGUI;

public class Mediator
{
    private Downloader downloader;
    private DownloaderUI downloaderUI;
    private Model<DownloaderState> model;
    private GComponent rootUI;
    public Mediator(GComponent rootUI = null)
    {
        model = new Model<DownloaderState>();
        downloaderUI = new DownloaderUI(model, rootUI);
        downloader = new Downloader(model);
        PopUp.Init();
    }
}