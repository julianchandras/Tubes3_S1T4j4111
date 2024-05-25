using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

class FingerprintReader
{
    public static (Mat, Mat, Mat) createSegmentedImg(Mat img, int w, double threshold)
    {
        int width = img.Cols;
        int height = img.Rows;

        Mat imageVariance = new Mat(height, width, DepthType.Cv64F, 1);
        Mat segmentedImage = img.Clone();
        Mat mask = new Mat(height, width, DepthType.Cv8U, 1, new MCvScalar(255));

        double globalStd = CvInvoke.MeanStdDev(img, out _, out _).V1;
        double localThreshold = globalStd * threshold;

        for (int i = 0; i < width; i += w)
        {
            for (int j = 0; j < height; j += w)
            {
                int blockWidth = Math.Min(w, width - i);
                int blockHeight = Math.Min(w, height - j);
                Rectangle rect = new Rectangle(i, j, blockWidth, blockHeight);

                Mat block = new Mat(img, rect);
                double blockStdDev = CvInvoke.MeanStdDev(block, out _, out _).V1;
                imageVariance.SetTo(new MCvScalar(blockStdDev), new Mat(mask, rect));

                if (blockStdDev < localThreshold)
                {
                    mask.SetTo(new MCvScalar(0), new Mat(mask, rect));
                }
                else
                {
                    mask.SetTo(new MCvScalar(255), new Mat(mask, rect));
                }
            }
        }

        mask = ApplyMorphologicalOperations(mask, w * 2);

        segmentedImage.SetTo(new MCvScalar(0), mask);
        Mat normalizedImage = NormalizeImage(segmentedImage, mask);

        return (segmentedImage, normalizedImage, mask);
    }

    public static Mat ApplyMorphologicalOperations(Mat img, int size)
    {
        Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(size, size), new Point(-1, -1));
        Mat result = new Mat();

        CvInvoke.MorphologyEx(img, result, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));
        CvInvoke.MorphologyEx(result, result, MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));

        return result;
    }

    public static Mat NormalizeImage(Mat img, Mat mask)
    {
        Mat result = new Mat(img.Size, DepthType.Cv8U, 1);

        Mat mean, stddev;
        CvInvoke.MeanStdDev(img, out mean, out stddev, mask);

        CvInvoke.Subtract(img, mean, result, mask, DepthType.Cv64F);
        CvInvoke.Divide(result, stddev, result, 1, DepthType.Cv64F);

        result.ConvertTo(result, DepthType.Cv8U, 255.0);

        return result;
    }

    public static Mat ConvertToBinary(Mat img, double threshold)
    {
        Mat binaryImg = new Mat();
        CvInvoke.Threshold(img, binaryImg, threshold, 255, ThresholdType.Binary);
        return binaryImg;
    }

    public static Mat ExtractCenterRegion(Mat img, int width, int height)
    {
        int x = (img.Cols - width) / 2;
        int y = (img.Rows - height) / 2;
        Rectangle center = new Rectangle(x, y, width, height);
        return new Mat(img, center);
    }

    public static List<string> ConvertToAscii(Mat binaryImage)
    {
        List<string> patterns = new List<string>();
        byte[,,] img = binaryImage.GetData();

        string asciiChars = " .:-=+*#%@";

        for (int y = 0; y < binaryImage.Rows; y++)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < binaryImage.Cols; x++)
            {
                byte pixel = img[y, x, 0];
                int idx = (pixel * (asciiChars.Length - 1)) / 255;
                sb.Append(asciiChars[idx]);
            }
            patterns.Add(sb.ToString());
        }

        return patterns;
    }

    public static void Main(string[] args)
    {
        string inputFilePath = "9__M_Left_index_finger.BMP";
        Mat img = CvInvoke.Imread(inputFilePath, ImreadModes.Grayscale);

        int blockSize = 16;
        double threshold = 0.2;

        var (segmentedImage, _, mask) = createSegmentedImg(img, blockSize, threshold);

        Mat centerRegion = ExtractCenterRegion(segmentedImage, 30, 30);
        Mat binaryCenterRegion = ConvertToBinary(centerRegion, 128);
        List<string> patterns = ConvertToAscii(binaryCenterRegion);
        foreach (string line in patterns)
        {
            Console.WriteLine(line);
        }
    }
}
