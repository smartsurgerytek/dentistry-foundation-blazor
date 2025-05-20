using System;
using System.Drawing.Text;
using System.IO;
using FellowOakDicom;
using FellowOakDicom.IO.Buffer;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using FellowOakDicom.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;

public class ImageOperationsHelper
{    
    // Private tag configuration (odd group number, creator UID)
    private const string PrivateCreatorUID = "SmartSurgeryTekTag";

    public static byte[] ConvertToDicom(IFormFile imageFile, string fdiData = "")
    {
        using var image = Image.Load<Rgb24>(imageFile.OpenReadStream());

        var dataset = new DicomDataset();
        AddBasicMetadata(dataset, "AnonymousPatient");
        AddImageParameters(dataset, image);
        AddPrivateTags(dataset, imageFile, fdiData);

        var pixelData = CreatePixelData(dataset, image);
        AddPixelFrame(pixelData, image);

        var dicomFile = new DicomFile(dataset);
        dicomFile.FileMetaInfo.TransferSyntax = DicomTransferSyntax.ExplicitVRLittleEndian;

        // read the json back
        var jsonString = ReadPrivateJsonTag(dicomFile);

        using var outputStream = new MemoryStream();
        dicomFile.Save(outputStream);
        return outputStream.ToArray();
    }

    private static void AddBasicMetadata(DicomDataset dataset, string patientName)
    {
        dataset.Add(DicomTag.PatientName, patientName);
        dataset.Add(DicomTag.PatientID, Guid.NewGuid().ToString());
        dataset.Add(DicomTag.StudyInstanceUID, DicomUID.Generate());
        dataset.Add(DicomTag.SeriesInstanceUID, DicomUID.Generate());
        dataset.Add(DicomTag.SOPInstanceUID, DicomUID.Generate());
        dataset.Add(DicomTag.SOPClassUID, DicomUID.SecondaryCaptureImageStorage);
    }

    private static void AddImageParameters(DicomDataset dataset, Image<Rgb24> image)
    {
        dataset.Add(DicomTag.Rows, (ushort)image.Height);
        dataset.Add(DicomTag.Columns, (ushort)image.Width);
        dataset.Add(DicomTag.BitsAllocated, (ushort)8);
        dataset.Add(DicomTag.BitsStored, (ushort)8);
        dataset.Add(DicomTag.HighBit, (ushort)7);

        // Corrected: Use string value instead of enum
        dataset.Add(DicomTag.PhotometricInterpretation, PhotometricInterpretation.Rgb.Value);

        dataset.Add(DicomTag.SamplesPerPixel, (ushort)3);
        dataset.Add(DicomTag.PixelRepresentation, (ushort)PixelRepresentation.Unsigned);
        dataset.Add(DicomTag.PlanarConfiguration, (ushort)PlanarConfiguration.Interleaved);
    }

    private static DicomPixelData CreatePixelData(DicomDataset dataset, Image<Rgb24> image)
    {
        var pixelData = DicomPixelData.Create(dataset, true);
        pixelData.BitsStored = 8;
        pixelData.SamplesPerPixel = 3;
        pixelData.PlanarConfiguration = PlanarConfiguration.Interleaved;
        return pixelData;
    }

    private static void AddPixelFrame(DicomPixelData pixelData, Image<Rgb24> image)
    {
        byte[] pixelBytes = new byte[image.Width * image.Height * 3];
        image.CopyPixelDataTo(pixelBytes);
        pixelData.AddFrame(new MemoryByteBuffer(pixelBytes));
    }

    private static void AddPrivateTags(DicomDataset dataset, IFormFile imageFile, string fdiData = "")
    {
        // 1. Add private creator identification (required for private tags)
        var privateCreatorTag = new DicomTag(0x1001, 0x0010);
        dataset.Add(new DicomLongString(privateCreatorTag, PrivateCreatorUID));
        // dataset.Add(new DicomUniqueIdentifier(DicomTag.PrivateCreator, PrivateCreatorUID));

        // 2. Create JSON metadata
        var jsonData = new
        {
            Conversion = new
            {
                Timestamp = DateTime.UtcNow.ToString("o"),
                OriginalFilename = imageFile.FileName,
                OriginalSize = imageFile.Length,
                Software = "DICOM Converter v1.0",
                Checksum = ComputeFileHash(imageFile)
            },
            ImageCharacteristics = new
            {
                Width = dataset.GetSingleValue<int>(DicomTag.Columns),
                Height = dataset.GetSingleValue<int>(DicomTag.Rows),
                ColorSpace = "RGB",
                BitsPerPixel = 24,
                OriginalFormat = imageFile.ContentType
            },
            FdiData = fdiData
        };

        // 3. Serialize and compress JSON
        byte[] jsonBytes = CompressJson(jsonData);

        // 4. Add to private tag with OB VR type
        var privateTag = new DicomTag(0x1001, 0x0011, PrivateCreatorUID);
        dataset.Add(new DicomOtherByte(privateTag, jsonBytes));

        // 5. Add compression marker tag
        var compressionTag = new DicomTag(0x1001, 0x0012, PrivateCreatorUID);
        dataset.Add(new DicomCodeString(compressionTag, "ZLIB"));
    }

    private static byte[] CompressJson(object jsonData)
    {
        string jsonString = JsonSerializer.Serialize(jsonData);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

        using var output = new MemoryStream();
        using (var compressor = new System.IO.Compression.DeflateStream(output, CompressionLevel.Optimal))
        {
            compressor.Write(jsonBytes, 0, jsonBytes.Length);
        }
        return output.ToArray();
    }

    private static string ComputeFileHash(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public static string ReadPrivateJsonTag(DicomFile dicomFile)
    {
        const string privateCreator = "SmartSurgeryTekTag";
        var jsonTag = new DicomTag(0x1001, 0x0011, privateCreator);
        var compressionTag = new DicomTag(0x1001, 0x0012, privateCreator);

        if (dicomFile.Dataset.TryGetValues<byte>(jsonTag, out byte[] jsonBytes))
        {
            string compression = dicomFile.Dataset.GetString(compressionTag) ?? "NONE";

            using var input = new MemoryStream(jsonBytes);
            using var output = new MemoryStream();

            if (compression == "ZLIB")
            {
                using var decompressor = new System.IO.Compression.DeflateStream(input, CompressionMode.Decompress);
                decompressor.CopyTo(output);
            }
            else
            {
                input.CopyTo(output);
            }

            return Encoding.UTF8.GetString(output.ToArray());
        }
        return null;
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