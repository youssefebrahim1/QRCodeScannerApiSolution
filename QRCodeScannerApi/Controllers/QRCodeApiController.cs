using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace QRCodeScannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeApiController : ControllerBase
    {
        // POST api/qrcodeapi/scan
        [HttpPost("scan")]
        public IActionResult Scan([FromForm] List<IFormFile> imageFiles)
        {
            var decodedResults = new List<string>();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile.Length > 0)
                {
                    try
                    {
                        // Process each image and decode QR code
                        using var stream = imageFile.OpenReadStream();
                        var bitmap = new Bitmap(stream);

                        // Preprocess the image (grayscale and thresholding)
                        var processedBitmap = PreprocessImage(bitmap);

                        // Decode the QR code
                        var reader = new BarcodeReader();
                        var result = reader.Decode(processedBitmap);

                        if (result != null)
                        {
                            decodedResults.Add(result.Text);
                        }
                        else
                        {
                            decodedResults.Add("No QR code found in this image.");
                        }
                    }
                    catch (Exception ex)
                    {
                        decodedResults.Add($"Error processing image: {ex.Message}");
                    }
                }
                else
                {
                    decodedResults.Add("Invalid image file.");
                }
            }

            return Ok(decodedResults);
        }

        // Method to preprocess the image (convert to grayscale and apply thresholding)
        private Bitmap PreprocessImage(Bitmap originalBitmap)
        {
            // Convert to grayscale
            var grayscaleBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);
            using (var g = Graphics.FromImage(grayscaleBitmap))
            {
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix(new float[][]
                {
                    new float[] { 0.3f, 0.3f, 0.3f, 0, 0 },
                    new float[] { 0.59f, 0.59f, 0.59f, 0, 0 },
                    new float[] { 0.11f, 0.11f, 0.11f, 0, 0 },
                    new float[] { 0, 0, 0, 1, 0 },
                    new float[] { 0, 0, 0, 0, 1 }
                });
                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(originalBitmap, new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height),
                    0, 0, originalBitmap.Width, originalBitmap.Height, GraphicsUnit.Pixel, attributes);
            }

            // Apply thresholding to make the image more "black and white"
            var thresholdBitmap = new Bitmap(grayscaleBitmap.Width, grayscaleBitmap.Height);
            for (int x = 0; x < grayscaleBitmap.Width; x++)
            {
                for (int y = 0; y < grayscaleBitmap.Height; y++)
                {
                    var pixelColor = grayscaleBitmap.GetPixel(x, y);
                    var avg = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); // Grayscale value
                    var threshold = avg > 128 ? 255 : 0; // Apply a simple threshold
                    thresholdBitmap.SetPixel(x, y, Color.FromArgb(threshold, threshold, threshold)); // Black and white
                }
            }

            return thresholdBitmap;
        }
    }
}
