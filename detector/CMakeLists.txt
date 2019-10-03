project(yolov3_detector)

set(DARK_INC  ${CMAKE_CURRENT_SOURCE_DIR}/inc)
aux_source_directory(src DETECTOR_SRCS)
include_directories(${DARK_INC})

link_directories(${OpenCV_LIBS} "${TORCH_LIBRARIES}" ) 

add_library(yolov3_detector SHARED ${DETECTOR_SRCS})
include(GenerateExportHeader)
GENERATE_EXPORT_HEADER(yolov3_detector EXPORT_FILE_NAME ${CMAKE_CURRENT_SOURCE_DIR}/inc/Detetion_export.h)

target_include_directories(yolov3_detector PUBLIC ${DARK_INC} )
target_link_libraries(yolov3_detector PUBLIC ${OpenCV_LIBS} "${TORCH_LIBRARIES}")
