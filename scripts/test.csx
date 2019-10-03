using System.Runtime.InteropServices;

public class YoloV3Model : IDisposable
{
    [DllImportAttribute("yolov3_detector.dll")]
    public static extern IntPtr load_yolov3_model(String cfg_file_path, String weight_file_path);

    [DllImportAttribute("yolov3_detector.dll")]
    public static extern int release_yolov3_model(IntPtr pModel);

    [DllImportAttribute("yolov3_detector.dll")]
    public unsafe static extern int detect_yolov3(IntPtr pModel, Single* pRgbData, /*YoloV3::*/void* pResults, int maxResults);

    private IntPtr _handle;

    public bool IsModelLoaded { get { return _handle != IntPtr.Zero; } }

    public YoloV3Model(String cfgFilePath, String weightFilePath)
    {
        _handle = load_yolov3_model(cfgFilePath, weightFilePath);
    }

    public void Dispose()
    {
        if(_handle != IntPtr.Zero)
        {
            IntPtr p = _handle;
            _handle = IntPtr.Zero;
            release_yolov3_model(p);
        }
    }
}

void RunTest()
{
    Console.WriteLine("Load model ...");

    YoloV3Model model = new YoloV3Model("../models/yolov3.cfg","../models/yolov3.weights");

    if(model.IsModelLoaded == false)
    {
        Console.WriteLine("Load model failed."); 
        return;
    }

    model.Dispose();
    Console.WriteLine("Test finished!");
}

RunTest();