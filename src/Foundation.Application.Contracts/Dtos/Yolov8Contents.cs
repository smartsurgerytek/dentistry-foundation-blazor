namespace Foundation.Application.Contracts.Dtos
{
    public class Yolov8Contents
    {
        public float Confidence { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public int[] Points { get; set; }
    }
}