#ifndef DETECTOR_H
#define DETECTOR_H

#include <memory>
#include <array>
#include <torch/torch.h>
#include <opencv2/opencv.hpp>
#include "Detetion_export.h"

enum class YOLOType {
    YOLOv3,
    YOLOv3_TINY
};

//class Rect2DF
//{
//public:
//	typedef float value_type;
//
//	//! default constructor
//	Rect2DF();
//	Rect2DF(float _x, float _y, float _width, float _height);
//	Rect2DF(const Rect2DF& r);
//
//	//Rect_& operator = (const Rect_& r);
//	//Rect_& operator = (Rect_&& r) CV_NOEXCEPT;
//	////! the top-left corner
//	//Point_<_Tp> tl() const;
//	////! the bottom-right corner
//	//Point_<_Tp> br() const;
//
//	////! size (width, height) of the rectangle
//	//Size_<_Tp> size() const;
//	//! area (width*height) of the rectangle
//	float area() const;
//	//! true if empty
//	bool empty() const;
//
//	////! checks whether the rectangle contains the point
//	//bool contains(const Point_<_Tp>& pt) const;
//
//	float x; //!< x coordinate of the top-left corner
//	float y; //!< y coordinate of the top-left corner
//	float width; //!< width of the rectangle
//	float height; //!< height of the rectangle
//};

struct YOLOV3_DETECTOR_EXPORT Detection {
	cv::Rect2f bbox;  // struct: (x, y, width, height)
	float scr; // maxProbabilityOfClass
	int64 cls; // class index
};

class YOLOV3_DETECTOR_EXPORT Detector {
public:
    explicit Detector(const std::array<int64_t, 2> &_inp_dim, 
		              const char* config_file,
					  const char* weight_file, 
		              YOLOType type = YOLOType::YOLOv3);

    ~Detector();

	std::vector<Detection> predict(cv::Mat image);
	std::vector<Detection> predict(float* pImageData, int width, int height);

private:
	torch::DeviceType device_type;

    class Darknet;
    std::unique_ptr<Darknet> net;
	
    std::array<int64_t, 2> inp_dim;
    static const float NMS_threshold;
    static const float confidence_threshold;
};

extern "C" {

	/**
	 * 加载 YoloV3 Model
	 */
	YOLOV3_DETECTOR_EXPORT void* load_yolov3_model(const char* cfg_file_path, const char* weight_file_path);

	/**
	 * 采用 YoloV3 Model 进行目标检测。pModel 为加载的 YoloV3 Model。pRgbData 为每个 Channel 的类型为 float RGB 格式的图像的地址
	 * pResults 为检测结果所放置的地方。maxResults 为所返回的最大目标数量。
	 * 返回值：所得到的目标检测结果数量
	 */
	YOLOV3_DETECTOR_EXPORT int detect_yolov3(const void* pModel, float* pRgbData, /*YoloV3::*/Detection* pResults, int maxResults);

	/**
	 * 释放创建的 YoloV3 Model
	 */
	YOLOV3_DETECTOR_EXPORT int release_yolov3_model(const void* pModel);

}

#endif //DETECTOR_H
