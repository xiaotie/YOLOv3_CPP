# 脚本使用说明

这里是对 yolov3_detector api 的调用演示。

## 前提

要运行这些脚本，需要将 libtorch 的 dll 和 opencv4.* 的 dll 复制到本目录下。

## 运行

test.csx 是 [dotnet-script](https://github.com/filipw/dotnet-script) 脚本。运行 dotnet script test.csx 即可。

## 说明

将 yolov3_detector 的 api 封装成 csharp class 方便使用。

```csharp
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
```