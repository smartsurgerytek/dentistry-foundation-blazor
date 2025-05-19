using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
//using System.Drawing;
using Amazon.S3.Model;
using Amazon.S3;
using DocumentFormat.OpenXml.Packaging;
using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

using static System.Net.WebRequestMethods;
using Amazon;
using DeviceDetectorNET.Class.Client;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Drawing;
//using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.Json;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using SixLabors.Fonts;
using PointF = SixLabors.ImageSharp.PointF;
using FontStyle = SixLabors.Fonts.FontStyle;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Office2010.Word;

namespace Foundation.Services
{
    [Audited]
    public class ExaminationAppService : ApplicationService, IExaminationAppService, ITransientDependency
    {
        private readonly IRepository<ExaminationReport, Guid> _examinationRepository;
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;


        public ExaminationAppService(IRepository<ExaminationReport, Guid> examinationRepository,
            IRepository<AuditLog, Guid> auditLogRepository)
        {
            _examinationRepository = examinationRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<string> CreateExaminationAsync(PatientExaminationRecordDto input)
        {
            try
            {
                await LogAudit("DocEditor - Create Examination", string.Empty);

                //using HttpClient httpClient = new HttpClient();
                var fileBytes = input.DefaultFileBytes;
                //var fileBytes = await httpClient.GetByteArrayAsync("/FileData/DefaultFile.docx");
                //var fileBytes = await httpClient.GetByteArrayAsync(input.FileBaseAddress + "/FileData/DefaultFile.docx");
                //var fileBytes = await httpClient.GetByteArrayAsync("http://smartsurgerytek.foundation.s3.amazonaws.com/foundation/FileData/DefaultFile.docx");
                await LogAudit("DocEditor - Read DefaultFile", string.Empty);
                using var stream = new MemoryStream();
                stream.Write(fileBytes, 0, fileBytes.Length);
                stream.Position = 0;

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, true))
                {
                    foreach (var headerPart in wordDocument.MainDocumentPart.HeaderParts)
                    {
                        var header = headerPart.Header;

                        var tablesHeader = header.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                        if (tablesHeader.Count > 0)
                        {
                            var headerTable = tablesHeader[0];

                            var firstRow = headerTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAtOrDefault(0);
                            if (firstRow != null)
                            {
                                var firstCell = firstRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ElementAtOrDefault(0);
                                if (firstCell != null)
                                {
                                    firstCell.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                                    firstCell.Append(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                        new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Date: " + DateTime.Now.ToShortDateString()))
                                    ));
                                }
                            }
                        }
                        header.Save();
                    }


                    #region Header And Body

                    var body = wordDocument.MainDocumentPart.Document.Body;
                    var tables = body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();
                    var firstTable = tables[0];

                    DocumentFormat.OpenXml.Wordprocessing.TableRow tbFirstRow = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAt(0);
                    DocumentFormat.OpenXml.Wordprocessing.TableRow tbSecondRow = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAt(1);
                    DocumentFormat.OpenXml.Wordprocessing.TableRow tbFourRow = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAt(3);
                    DocumentFormat.OpenXml.Wordprocessing.TableRow tblSixteenRow = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ElementAt(16);

                    DocumentFormat.OpenXml.Wordprocessing.TableCell cellName = tbFirstRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ElementAt(0);
                    DocumentFormat.OpenXml.Wordprocessing.TableCell cellDoctorName = tbFirstRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ElementAt(1);
                    DocumentFormat.OpenXml.Wordprocessing.TableCell cellDob = tbSecondRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ElementAt(0);

                    cellName.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                    cellDoctorName.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                    cellDob.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();


                    var paraName = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text("Name: " + input.PatientName)
                        )
                    );
                    var paraDentist = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text("Dentist: " + input.DoctorName)
                        )
                    );
                    var paraDob = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.Run(
                            new DocumentFormat.OpenXml.Wordprocessing.Text("Date Of Birth: " + input.PatientDob)
                        )
                    );

                    cellName.Append(paraName);
                    cellDoctorName.Append(paraDentist);
                    cellDob.Append(paraDob);

                    await LogAudit("DocEditor - Header Create", string.Empty);

                    #endregion

                    #region Image Creation using ImageSharp

                    var cellPic = tbFourRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ElementAt(0);

                    //var imageUrl = "http://smartsurgerytek.foundation.s3.amazonaws.com/foundation/FileData/withnum.jpg";
                    //var imageUrl = input.DefaultJawImageBytes;
                    //using HttpClient httpClientImg = new();
                    //using var responseData = await httpClientImg.GetAsync(imageUrl);
                    //responseData.EnsureSuccessStatusCode();


                    await using var remoteStream = new MemoryStream(input.DefaultJawImageBytes);

                    var issueTeethList =
                        input.UpperLeft.Where(t => t.CariesYes == true).Select(t => t.ToothNumber)
                        .Concat(input.UpperRight.Where(t => t.CariesYes == true).Select(t => t.ToothNumber))
                        .Concat(input.LowerLeft.Where(t => t.CariesYes == true).Select(t => t.ToothNumber))
                        .Concat(input.LowerRight.Where(t => t.CariesYes == true).Select(t => t.ToothNumber))
                        .ToArray();

                    Dictionary<int, Rectangle> toothMap = new()
        {
            { 18, new Rectangle(55, 307, 25, 25) },{ 17, new Rectangle(68, 263, 25, 25) },{ 16, new Rectangle(82, 220, 25, 25) },
            { 15, new Rectangle(106, 178, 25, 25) },{ 14, new Rectangle(118, 149, 25, 25) },{ 13, new Rectangle(139, 119, 25, 25) },
            { 12, new Rectangle(171, 102, 22, 22) },{ 11, new Rectangle(206, 83, 25, 25) },{ 21, new Rectangle(249, 86, 25, 25) },
            { 22, new Rectangle(287, 98, 25, 25) },{ 23, new Rectangle(316, 120, 25, 25) },{ 24, new Rectangle(336, 150, 25, 25) },
            { 25, new Rectangle(353, 177, 25, 25) },{ 26, new Rectangle(366, 220, 25, 25) },{ 27, new Rectangle(383, 261, 22, 22) },
            { 28, new Rectangle(390, 303, 25, 25) },{ 48, new Rectangle(482, 95, 25, 25) },{ 47, new Rectangle(495, 136, 25, 25) },
            { 46, new Rectangle(510, 178, 25, 25) },{ 45, new Rectangle(533, 219, 25, 25) },{ 44, new Rectangle(554, 249, 25, 25) },
            { 43, new Rectangle(576, 277, 25, 25) },{ 42, new Rectangle(608, 294, 22, 22) },{ 41, new Rectangle(637, 303, 25, 25) },
            { 31, new Rectangle(665, 305, 25, 25) },{ 32, new Rectangle(697, 296, 25, 25) },{ 33, new Rectangle(729, 277, 25, 25) },
            { 34, new Rectangle(748, 248, 25, 25) },{ 35, new Rectangle(771, 216, 25, 25) },{ 36, new Rectangle(791, 179, 25, 25) },
            { 37, new Rectangle(807, 135, 25, 25) },{ 38, new Rectangle(820, 93, 25, 25) }
        };

                    using var image = await SixLabors.ImageSharp.Image.LoadAsync(remoteStream);
                    var green = SixLabors.ImageSharp.Color.Green;
                    var white = SixLabors.ImageSharp.Color.White;

                    // Load font
                    //var fontFamily = SixLabors.Fonts.SystemFonts.Families.FirstOrDefault();                  
                    //var font = fontFamily.CreateFont(12, FontStyle.Bold);


                    // Mutate image with highlights
                    foreach (int tooth in issueTeethList)
                    {
                        if (toothMap.TryGetValue(tooth, out var rect))
                        {
                            image.Mutate(ctx =>
                            {
                                ctx.Fill(green, new EllipsePolygon(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2));
                                //ctx.DrawText(tooth.ToString(), font, white, new PointF(rect.X + rect.Width / 2 - 5, rect.Y + rect.Height / 2 - 5));
                            });
                        }
                    }

                    // Save final image to memory stream
                    using var finalImageStream = new MemoryStream();
                    await image.SaveAsJpegAsync(finalImageStream);
                    finalImageStream.Position = 0;

                    // Insert image into Word using OpenXML
                    ImagePart imagePart = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                    imagePart.FeedData(finalImageStream);
                    string imagePartId = wordDocument.MainDocumentPart.GetIdOfPart(imagePart);

                    double scale = 0.5;

                    long widthEmus = (long)(image.Width * 9525 * scale);
                    long heightEmus = (long)(image.Height * 9525 * scale);

                    var drawing = new DocumentFormat.OpenXml.Wordprocessing.Drawing(
                        new DW.Inline(
                            new DW.Extent() { Cx = widthEmus, Cy = heightEmus },
                            new DW.EffectExtent() { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                            new DW.DocProperties() { Id = (UInt32Value)1U, Name = "Tooth Highlight" },
                            new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                            new A.Graphic(
                                new A.GraphicData(
                                    new PIC.Picture(
                                        new PIC.NonVisualPictureProperties(
                                            new PIC.NonVisualDrawingProperties() { Id = 0U, Name = "output.jpg" },
                                            new PIC.NonVisualPictureDrawingProperties()
                                        ),
                                        new PIC.BlipFill(
                                            new A.Blip() { Embed = imagePartId, CompressionState = A.BlipCompressionValues.Print },
                                            new A.Stretch(new A.FillRectangle())
                                        ),
                                        new PIC.ShapeProperties(
                                            new A.Transform2D(
                                                new A.Offset() { X = 0L, Y = 0L },
                                                new A.Extents() { Cx = widthEmus, Cy = heightEmus }
                                            ),
                                            new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                                        )
                                    )
                                )
                                { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                            )
                        )
                        {
                            DistanceFromTop = 0U,
                            DistanceFromBottom = 0U,
                            DistanceFromLeft = 0U,
                            DistanceFromRight = 0U
                        });

                    cellPic.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                    cellPic.Append(new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                        new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties(new Justification() { Val = JustificationValues.Center }),
                        new DocumentFormat.OpenXml.Wordprocessing.Run(drawing)
                    ));

                    await LogAudit("DocEditor - Image Create", string.Empty);

                    #endregion

                    #region Checkbox Creation

                    var rows = firstTable.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>().ToList();

                    var rowMaxilla = rows[4];
                    var rowMaxillaCarries = rows[5];
                    var rowMandible = rows[11];
                    var rowMandibleCarries = rows[10];

                    var upperRightToothMap = new Dictionary<int, int> { { 18, 1 }, { 17, 2 }, { 16, 3 }, { 15, 4 }, { 14, 5 }, { 13, 6 }, { 12, 7 }, { 11, 8 } };
                    var upperLeftToothMap = new Dictionary<int, int> { { 21, 9 }, { 22, 10 }, { 23, 11 }, { 24, 12 }, { 25, 13 }, { 26, 14 }, { 27, 15 }, { 28, 16 } };
                    var lowerRightToothMap = new Dictionary<int, int> { { 48, 1 }, { 47, 2 }, { 46, 3 }, { 45, 4 }, { 44, 5 }, { 43, 6 }, { 42, 7 }, { 41, 8 } };
                    var lowerLeftToothMap = new Dictionary<int, int> { { 31, 9 }, { 32, 10 }, { 33, 11 }, { 34, 12 }, { 35, 13 }, { 36, 14 }, { 37, 15 }, { 38, 16 }, };

                    foreach (var tooth in input.UpperRight)
                    {
                        if (tooth.CariesYes == true && upperRightToothMap.TryGetValue(tooth.ToothNumber, out int cellIndex))
                        {
                            var cells = rowMaxilla.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();
                            var cellsCarries = rowMaxillaCarries.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();

                            if (cells.Count > cellIndex)
                            {
                                var cell = cells[cellIndex];
                                var cellCarries = cellsCarries[cellIndex];

                                cell.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                                var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                                        new DocumentFormat.OpenXml.Wordprocessing.Text(tooth.SelectedPE ?? string.Empty)
                                    )
                                );
                                cell.Append(para);
                                CheckLegacyCheckbox(cellCarries);
                            }
                        }
                    }
                    foreach (var tooth in input.UpperLeft)
                    {
                        if (tooth.CariesYes == true && upperLeftToothMap.TryGetValue(tooth.ToothNumber, out int cellIndex))
                        {
                            var cells = rowMaxilla.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();
                            var cellsCarries = rowMaxillaCarries.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();

                            if (cells.Count > cellIndex)
                            {
                                var cell = cells[cellIndex];
                                var cellCarries = cellsCarries[cellIndex];

                                cell.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                                var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                                        new DocumentFormat.OpenXml.Wordprocessing.Text(tooth.SelectedPE ?? string.Empty)
                                    )
                                );
                                cell.Append(para);
                                CheckLegacyCheckbox(cellCarries);
                            }
                        }
                    }
                    foreach (var tooth in input.LowerRight)
                    {
                        if (tooth.CariesYes == true && lowerRightToothMap.TryGetValue(tooth.ToothNumber, out int cellIndex))
                        {
                            var cells = rowMandible.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();
                            var cellsCarries = rowMandibleCarries.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();

                            if (cells.Count > cellIndex)
                            {
                                var cell = cells[cellIndex];
                                var cellCarries = cellsCarries[cellIndex];

                                cell.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
                                var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                                        new DocumentFormat.OpenXml.Wordprocessing.Text(tooth.SelectedPE ?? string.Empty)
                                    )
                                );
                                cell.Append(para);
                                CheckLegacyCheckbox(cellCarries);
                            }
                        }
                    }
                    foreach (var tooth in input.LowerLeft)
                    {
                        if (tooth.CariesYes == true && lowerLeftToothMap.TryGetValue(tooth.ToothNumber, out int cellIndex))
                        {
                            var cells = rowMandible.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();
                            var cellsManCarries = rowMandibleCarries.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().ToList();
                            if (cells.Count > cellIndex)
                            {
                                var cell = cells[cellIndex];
                                var cellManCarries = cellsManCarries[cellIndex];
                                cell.RemoveAllChildren<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();

                                var para = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(
                                    new DocumentFormat.OpenXml.Wordprocessing.Run(
                                        new DocumentFormat.OpenXml.Wordprocessing.Text(tooth.SelectedPE ?? string.Empty)
                                    )
                                );
                                cell.Append(para);
                                CheckLegacyCheckbox(cellManCarries);
                            }
                        }
                    }

                    await LogAudit("DocEditor - Checkbox Create", string.Empty);

                    #endregion

                    #region Dycom Images

                    var targetCell = tblSixteenRow.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>().First();
                    var allImages = await GetAllSelectedImages(input.ImageNames,input.FileBaseAddress);

                    foreach (var imageBytes in allImages)
                    {
                        var imagePartDc = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                        using (var streamDc = new MemoryStream(imageBytes))
                        {
                            imagePartDc.FeedData(streamDc);
                        }

                        string relationshipId = wordDocument.MainDocumentPart.GetIdOfPart(imagePartDc);
                        var imageDrawing = CreateImageElement(relationshipId, 200, 150);
                        var imageParagraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(imageDrawing));
                        targetCell.Append(imageParagraph);
                    }

                    #endregion



                    wordDocument.MainDocumentPart.Document.Save();
                    await LogAudit("DocEditor - File Create", string.Empty);
                }


                #region File Creation And Upload On S3 Bucket

                var modifiedFileBytes = stream.ToArray();

                // S3 configuration
                var bucketName = "smartsurgerytek.foundation";
                var region = RegionEndpoint.USWest2;
                var accessKey = "AKIAZI2LGNNVDTFYF57P";
                var secretKey = "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";

                // Generate S3 file key
                var root = "foundation/reports/";
                var fileNameOnly = $"{input.PatientId}_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
                var s3Key = root + fileNameOnly;

                using var s3Client = new AmazonS3Client(accessKey, secretKey, region);
                using var streamData = new MemoryStream(modifiedFileBytes);

                // Upload file to S3
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    InputStream = streamData,
                    ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                };

                var response = await s3Client.PutObjectAsync(putRequest);
                await LogAudit("DocEditor - Save S3 bucket", string.Empty);
                if (response.HttpStatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Failed to upload file to S3");
                }

                #endregion

                #region Generate Pre-Signed URL

                var preSignedRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    Verb = HttpVerb.GET
                };

                string preSignedUrl = s3Client.GetPreSignedURL(preSignedRequest);

                #endregion

                return preSignedUrl;
            }
            catch (Exception ex)
            {
                await LogAudit("DocEditor Error " + ex.Message, string.Empty);
                string str = ex.ToString();
                throw;
            }
        }

        private DocumentFormat.OpenXml.Wordprocessing.Drawing CreateImageElement(string relationshipId, int width, int height)
        {
            long emuWidth = width * 9525;
            long emuHeight = height * 9525;

            return new DocumentFormat.OpenXml.Wordprocessing.Drawing(
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = emuWidth, Cy = emuHeight },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture"
                    },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                        new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks { NoChangeAspect = true }),
                    new DocumentFormat.OpenXml.Drawing.Graphic(
                        new DocumentFormat.OpenXml.Drawing.GraphicData(
                            new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties
                                    {
                                        Id = 0U,
                                        Name = "New Image"
                                    },
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()
                                ),
                                new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                    new DocumentFormat.OpenXml.Drawing.Blip
                                    {
                                        Embed = relationshipId,
                                        CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                                    },
                                    new DocumentFormat.OpenXml.Drawing.Stretch(
                                        new DocumentFormat.OpenXml.Drawing.FillRectangle())
                                ),
                                new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                    new DocumentFormat.OpenXml.Drawing.Transform2D(
                                        new DocumentFormat.OpenXml.Drawing.Offset { X = 0L, Y = 0L },
                                        new DocumentFormat.OpenXml.Drawing.Extents { Cx = emuWidth, Cy = emuHeight }
                                    ),
                                    new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                        new DocumentFormat.OpenXml.Drawing.AdjustValueList())
                                    { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle })
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U,
                });
        }

        private async Task<List<byte[]>> GetAllSelectedImages(string imageNames,string bAddress)
        {
            Console.WriteLine($"{imageNames}");
            var allImages = new List<byte[]>();
            var imageNamesList = imageNames.Split(',');
            var httpClient = new HttpClient();
            foreach (var imageName in imageNamesList)
            {
                var imageStream = await httpClient.GetStreamAsync(bAddress + "api/FileProvider/AmazonS3GetImage?Path="+ imageName);
                var imageBytes = await imageStream.GetAllBytesAsync();
                Console.WriteLine($"{imageName} {imageBytes.Length}");
                allImages.Add(imageBytes);
            }

            return allImages;
        }

        private void CheckLegacyCheckbox(OpenXmlElement tableCell)
        {
            var checkBoxRun = tableCell
                    .Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>()
                .FirstOrDefault(r => r.Descendants<CheckBox>().Any());

            if (checkBoxRun == null) return;

            var checkBox = checkBoxRun.Descendants<CheckBox>().FirstOrDefault();
            if (checkBox == null) return;

            checkBox.Elements<DocumentFormat.OpenXml.Wordprocessing.Checked>().ToList().ForEach(c => c.Remove());

            checkBox.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Checked { Val = OnOffValue.FromBoolean(true) });
        }

        private async Task LogAudit(string methodName, object parameters)
        {
            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(ExaminationAppService),
                MethodName = $"Report Examination - {methodName}",
                Parameters = JsonSerializer.Serialize(parameters),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
        }

    }
}
