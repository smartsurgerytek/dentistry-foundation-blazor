namespace Foundation.Application.Contracts.Dtos
{
    public class FDISegmentationResponseDto
    {
        public int Request_Id { get; set; }
        public YoloResults Yolo_Results { get; set; }
        public string Message { get; set; }
    }
}