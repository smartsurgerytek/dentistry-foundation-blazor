using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class ImageEditorUploadAndReplaceDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileStream { get; set; }
        public string OldFileName { get; set; }
    }
}
