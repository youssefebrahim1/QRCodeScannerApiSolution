using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace QRCodeScannerApi.Models
{
    public class QRCodeModel
    {
        // Property to hold multiple image files
        public IEnumerable<IFormFile> ImageFiles { get; set; }
    }
}
