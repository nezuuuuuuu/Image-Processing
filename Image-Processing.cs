using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace ImageProcessingApp
{
    public partial class MainForm : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;

        public MainForm()
        {
            InitializeComponent();
        }

        // Load Image
        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    originalImage = new Bitmap(ofd.FileName);
                    pictureBoxOriginal.Image = originalImage;
                    processedImage = new Bitmap(originalImage);
                    pictureBoxProcessed.Image = processedImage;
                }
            }
        }

        // Basic Copy
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;
            processedImage = new Bitmap(originalImage);
            pictureBoxProcessed.Image = processedImage;
        }

        // Grayscale
        private void btnGrayscale_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    int gray = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    processedImage.SetPixel(x, y, newColor);
                }
            }
            pictureBoxProcessed.Image = processedImage;
        }

        // Color Inversion
        private void btnInvertColors_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                    processedImage.SetPixel(x, y, invertedColor);
                }
            }
            pictureBoxProcessed.Image = processedImage;
        }

        // Histogram
        private void btnHistogram_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            int[] histogram = new int[256];
            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    int grayValue = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                    histogram[grayValue]++;
                }
            }

            // Display histogram (for simplicity, showing the data in console)
            Console.WriteLine("Histogram Data:");
            for (int i = 0; i < histogram.Length; i++)
            {
                Console.WriteLine($"Gray level {i}: {histogram[i]} pixels");
            }
        }

        // Sepia
        private void btnSepia_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    int tr = (int)(0.393 * pixelColor.R + 0.769 * pixelColor.G + 0.189 * pixelColor.B);
                    int tg = (int)(0.349 * pixelColor.R + 0.686 * pixelColor.G + 0.168 * pixelColor.B);
                    int tb = (int)(0.272 * pixelColor.R + 0.534 * pixelColor.G + 0.131 * pixelColor.B);

                    Color sepiaColor = Color.FromArgb(
                        Math.Min(tr, 255),
                        Math.Min(tg, 255),
                        Math.Min(tb, 255)
                    );

                    processedImage.SetPixel(x, y, sepiaColor);
                }
            }
            pictureBoxProcessed.Image = processedImage;
        }

        // Save Image
        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    processedImage.Save(sfd.FileName, ImageFormat.Png);
                }
            }
        }

        // Image Subtraction with Background Replacement
        private void btnSubtractImages_Click(object sender, EventArgs e)
        {
            // Load Image A and Image B
            Bitmap imageA = LoadImage(); // Load Image with background
            Bitmap imageB = LoadImage(); // Load Image with green background
            if (imageA == null || imageB == null) return;

            Bitmap resultImage = new Bitmap(imageA.Width, imageA.Height);
            Color greenScreen = Color.FromArgb(0, 255, 0); // Set the green color to replace
            int threshold = 20; // Set a threshold for subtraction

            for (int y = 0; y < imageA.Height; y++)
            {
                for (int x = 0; x < imageA.Width; x++)
                {
                    Color pixelA = imageA.GetPixel(x, y);
                    Color pixelB = imageB.GetPixel(x, y);

                    if (pixelB.ToArgb() == greenScreen.ToArgb())
                    {
                        resultImage.SetPixel(x, y, pixelA);
                    }
                    else
                    {
                        int grayA = (pixelA.R + pixelA.G + pixelA.B) / 3;
                        int grayB = (pixelB.R + pixelB.G + pixelB.B) / 3;
                        int diff = Math.Abs(grayA - grayB);

                        if (diff > threshold)
                        {
                            resultImage.SetPixel(x, y, Color.Black); // Set different pixels to black
                        }
                        else
                        {
                            resultImage.SetPixel(x, y, pixelA); // Set similar pixels to original
                        }
                    }
                }
            }

            pictureBoxResult.Image = resultImage; // Display the result
        }

        private Bitmap LoadImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return new Bitmap(ofd.FileName);
                }
            }
            return null;
        }
    }
}
