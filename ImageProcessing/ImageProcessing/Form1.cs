using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace ImageProcessing
{
    public partial class Form1 : Form
    {
        private Bitmap originalImage;
        private Bitmap processedImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialization code if needed
        }

        // Load Image (button1)
        private void button1_Click(object sender, EventArgs e)
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

        // Basic Copy (button2)
        private void button2_Click(object sender, EventArgs e)
        {
            if (originalImage == null) return;
            processedImage = new Bitmap(originalImage);
            pictureBoxProcessed.Image = processedImage;
        }

        // Color Inversion
        private void invertcolorsbtn_Click(object sender, EventArgs e)
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
        private void histogrambtn_Click(object sender, EventArgs e)
        {
            if (processedImage == null) return;

            int[] histogram = new int[256];

            // Calculate histogram values
            for (int y = 0; y < processedImage.Height; y++)
            {
                for (int x = 0; x < processedImage.Width; x++)
                {
                    Color pixelColor = processedImage.GetPixel(x, y);
                    int grayValue = (int)((pixelColor.R + pixelColor.G + pixelColor.B) / 3);
                    histogram[grayValue]++;
                }
            }

            // Find the maximum value for scaling
            int max = histogram.Max();

            // Create a bitmap to draw the histogram
            Bitmap histogramBitmap = new Bitmap(256, 100);
            using (Graphics g = Graphics.FromImage(histogramBitmap))
            {
                g.Clear(Color.White);

                // Draw each histogram bar
                for (int i = 0; i < histogram.Length; i++)
                {
                    int barHeight = (int)((histogram[i] / (float)max) * histogramBitmap.Height);
                    g.DrawLine(Pens.Black, i, histogramBitmap.Height, i, histogramBitmap.Height - barHeight);
                }
            }

            // Display the histogram image in pictureBoxHistogram
            pictureBoxResult.Image = histogramBitmap;
        }

        // Sepia
        private void sepiabtn_Click(object sender, EventArgs e)
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
        private void saveimagebtn_Click(object sender, EventArgs e)
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

        // Subtract Images
        private void subtractimagesbtn_Click(object sender, EventArgs e)
        {
            // Load Image A and Image B
            Bitmap imageA = LoadImage(); // Image with original background
            pictureBoxOriginal.Image = imageA;

            Bitmap imageB = LoadImage(); // Image with green background

            pictureBoxProcessed.Image = imageB;

            if (imageA == null || imageB == null) return;

            Bitmap resultImage = new Bitmap(imageA.Width, imageA.Height);
            Color greenScreen = Color.FromArgb(0, 255, 0); // Set the green color to replace
            int threshold = 20; // Threshold for subtraction

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
                            resultImage.SetPixel(x, y, Color.Black); // Mark different pixels
                        }
                        else
                        {
                            resultImage.SetPixel(x, y, pixelA); // Similar pixels stay the same
                        }
                    }
                }
            }

            pictureBoxResult.Image = resultImage;
        }

        // Helper to load images for subtraction
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

        private void grayscalebtn_Click_1(object sender, EventArgs e)
        {
            if (originalImage == null) return;

            // Create a new bitmap to avoid modifying the original image
            processedImage = new Bitmap(originalImage.Width, originalImage.Height);

            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    Color pixelColor = originalImage.GetPixel(x, y);

                    // Use a weighted average to get a more accurate grayscale value
                    int gray = (int)(0.3 * pixelColor.R + 0.59 * pixelColor.G + 0.11 * pixelColor.B);
                    Color newColor = Color.FromArgb(gray, gray, gray);

                    processedImage.SetPixel(x, y, newColor);
                }
            }

            pictureBoxProcessed.Image = processedImage;
        }
    }
}
