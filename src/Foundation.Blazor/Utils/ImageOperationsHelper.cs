using System;
using System.Drawing.Text;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.IO.Buffer;
using SkiaSharp;

public class ImageOperationsHelper
{
    public static byte[] ConvertImageToDicom(byte[] imgBytes, string fdiData)
    {
        // Load the PNG image using SkiaSharp
        using var skBitmap = SKBitmap.Decode(imgBytes) ?? throw new ArgumentException("Invalid image stream. Could not decode the image.");

        // Convert the bitmap to a byte array
        var pixelData = GetPixelData(skBitmap);

        // Create a new DICOM dataset
        var dicomDataset = new DicomDataset
        {
            { DicomTag.PatientName, "John Doe" },
            { DicomTag.PatientID, "12345" },
            { DicomTag.StudyInstanceUID, DicomUID.Generate() },
            { DicomTag.SeriesInstanceUID, DicomUID.Generate() },
            { DicomTag.SOPInstanceUID, DicomUID.Generate() },
            { DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage },
            { DicomTag.PhotometricInterpretation, "RGB" },
            { DicomTag.Rows, (ushort)skBitmap.Height },
            { DicomTag.Columns, (ushort)skBitmap.Width },
            { DicomTag.BitsAllocated, (ushort)8 },
            { DicomTag.BitsStored, (ushort)8 },
            { DicomTag.HighBit, (ushort)7 },
            { DicomTag.PixelRepresentation, (ushort)0 },
            { DicomTag.PixelData, pixelData }
        };

        // Add FDI data as a private tag
        var privateTag = new DicomTag(0x9999, 0x0010, "SmartSurgeryExtension");
        // dicomDataset.Add(privateTag, DicomVR.LO, fdiData);

        // Create a DICOM file and save it
        var dicomFile = new DicomFile(dicomDataset);
        // Save the DICOM file to a MemoryStream and return as byte[]
        using var memoryStream = new MemoryStream();
        dicomFile.Save(memoryStream);

        return memoryStream.ToArray();
    }

    private static byte[] GetPixelData(SKBitmap skBitmap)
    {
        // Convert the SKBitmap pixel data to a byte array
        var pixelData = new byte[skBitmap.ByteCount];
        System.Runtime.InteropServices.Marshal.Copy(skBitmap.GetPixels(), pixelData, 0, pixelData.Length);
        return pixelData;
    }

    public static Stream CombineTwoImages(byte[] firstImageStream, byte[] secondImageStream)
    {
        if (firstImageStream == null)
        {
            throw new ArgumentNullException(nameof(firstImageStream));
        }

        if (secondImageStream == null)
        {
            throw new ArgumentNullException(nameof(secondImageStream));
        }

        // Decode the streams into SKBitmap objects
        using var firstImage = SKBitmap.Decode(firstImageStream);
        using var secondImage = SKBitmap.Decode(secondImageStream);

        if (firstImage == null || secondImage == null)
        {
            throw new ArgumentException("One or both streams could not be decoded into images.");
        }

        // Resize the second image to match the height of the first image
        int resizedSecondImageWidth = (int)((double)secondImage.Width / secondImage.Height * firstImage.Height);
        using var resizedSecondImage = secondImage.Resize(new SKImageInfo(resizedSecondImageWidth, firstImage.Height), SKFilterQuality.High);

        // Calculate the dimensions of the output image
        int outputImageWidth = firstImage.Width + resizedSecondImage.Width;
        int outputImageHeight = firstImage.Height;

        // Create the output image
        var outputImage = new SKBitmap(outputImageWidth, outputImageHeight);

        using (var canvas = new SKCanvas(outputImage))
        {
            // Draw the first image
            canvas.DrawBitmap(firstImage, new SKRect(0, 0, firstImage.Width, firstImage.Height));

            // Draw the resized second image next to the first image
            canvas.DrawBitmap(resizedSecondImage, new SKRect(firstImage.Width, 0, firstImage.Width + resizedSecondImage.Width, firstImage.Height));
        }

        // Encode the output image into a memory stream
        var memoryStream = new MemoryStream();
        using (var image = SKImage.FromBitmap(outputImage))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            data.SaveTo(memoryStream);
        }

        // Reset the stream position to the beginning
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}