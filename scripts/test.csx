#r "nuget: Geb.Image, 2.0.0"

using System.Runtime.InteropServices;
using Geb.Image;

public class YoloV3Model : IDisposable
{
    public struct DetectResult
    {
        public float X,Y,Width,Height,Score;
        public Int64 ClassIndex;
        public override String ToString(){
            return $"Class:{ClassIndex},Score:{Score},X:{X},Y:{Y},Width:{Width},Height:{Height}";
        }

        public DetectResult Scale(float xScale, float yScale)
        {
            X*=xScale;
            Y*=yScale;
            Width*=xScale;
            Height*=yScale;
            return this;
        }
    }

    [DllImportAttribute("yolov3_detector.dll")]
    public static extern IntPtr load_yolov3_model(String cfg_file_path, String weight_file_path);

    [DllImportAttribute("yolov3_detector.dll")]
    public static extern int release_yolov3_model(IntPtr pModel);

    [DllImportAttribute("yolov3_detector.dll")]
    public unsafe static extern int detect_yolov3(IntPtr pModel, Single* pRgbData, /*YoloV3::*/void* pResults, int maxResults);

    private IntPtr _handle;
    private int MaxResults = 512;

    public bool IsModelLoaded { get { return _handle != IntPtr.Zero; } }

    public YoloV3Model(String cfgFilePath, String weightFilePath)
    {
        _handle = load_yolov3_model(cfgFilePath, weightFilePath);
    }

    public unsafe List<DetectResult> Detect(ImageBgra32 image)
    {
        List<DetectResult> list = new List<DetectResult>();

        ImageBgra32 imageResize = image;
        float scaleX = 1;
        float scaleY= 1;

        if(image.Width != 416 || image.Height != 416)
        {
            imageResize = image.Resize(416,416);
            scaleX = image.Width/416.0f;
            scaleY = image.Height/416.0f;
        }   
        
        ImageFloat imgInput = new ImageFloat(imageResize.Width*3, imageResize.Height);
        Bgra32* pBgr = imageResize.Start;
        Bgra32* pBgrEnd = pBgr + imageResize.Length; 
        float* p = imgInput.Start;
        while(pBgr < pBgrEnd)
        {
            p[0] = pBgr->Red;
            p[1] = pBgr->Green;
            p[2] = pBgr->Blue;
            pBgr++;
            p+=3;
        }

        DetectResult* results = stackalloc DetectResult[MaxResults];
        int count = detect_yolov3(_handle, imgInput.Start, results, MaxResults);

        for(int i = 0; i < count; i++)
        {
            list.Add(results[i].Scale(scaleX,scaleY));
        }

        if(imageResize != image) imageResize.Dispose();
        imgInput.Dispose();

        return list;
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

    String imgPath = "../images/dog.jpg";
    ImageBgra32 img = new Geb.Image.Formats.Jpeg.JpegDecoder().Decode(imgPath);
    var list = model.Detect(img);

    Console.WriteLine($"Detect {list.Count} Objects");
    foreach(var item in list)
        Console.WriteLine(item.ToString());

    model.Dispose();
    Console.WriteLine("Test finished!");
}

RunTest();