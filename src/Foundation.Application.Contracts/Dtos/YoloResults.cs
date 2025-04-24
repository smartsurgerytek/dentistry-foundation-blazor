using System.Collections.Generic;

namespace Foundation.Application.Contracts.Dtos
{
    public class YoloResults
    {
        public Dictionary<string, string> Class_Names { get; set; }
        public Yolov8Contents[] Yolov8_Contents { get; set; }
    }
}