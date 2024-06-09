using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace Biometric.Controller
{
    class FingerprintReader
    {
        private static (Mat, Mat) createSegmentedImg(Mat img, int w, double threshold)
        {
            int width = img.Cols;
            int height = img.Rows;

            Mat imageVariance = new Mat(new Size(width, height), DepthType.Cv64F, 1);
            Mat segmentedImage = img.Clone();
            Mat mask = new Mat(new Size(width, height), DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(255));

            MCvScalar mean = new MCvScalar();
            MCvScalar stddev = new MCvScalar();
            CvInvoke.MeanStdDev(img, ref mean, ref stddev);
            double globalStd = stddev.V0;
            double localThreshold = globalStd * threshold;

            for (int i = 0; i < width; i += w)
            {
                for (int j = 0; j < height; j += w)
                {
                    int blockWidth = Math.Min(w, width - i);
                    int blockHeight = Math.Min(w, height - j);
                    Rectangle rect = new Rectangle(i, j, blockWidth, blockHeight);

                    Mat block = new Mat(img, rect);
                    CvInvoke.MeanStdDev(block, ref mean, ref stddev);
                    double blockStdDev = stddev.V0;

                    Mat blockMask = new Mat(new Size(blockWidth, blockHeight), DepthType.Cv8U, 1);
                    if (blockStdDev < localThreshold)
                    {
                        blockMask.SetTo(new MCvScalar(0));
                    }
                    else
                    {
                        blockMask.SetTo(new MCvScalar(255));
                    }

                    Mat maskRegion = new Mat(mask, rect);
                    blockMask.CopyTo(maskRegion);
                }
            }

            mask = applyMorphologicalOperations(mask, w * 2);

            Mat invertedMask = new Mat();
            CvInvoke.BitwiseNot(mask, invertedMask);
            segmentedImage.SetTo(new MCvScalar(0), invertedMask);
            Mat normalizedImage = normalizeImage(segmentedImage, mask);

            return (normalizedImage, mask);
        }

        private static Mat applyMorphologicalOperations(Mat img, int size)
        {
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Ellipse, new Size(size, size), new Point(-1, -1));
            Mat result = new Mat();

            CvInvoke.MorphologyEx(img, result, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));
            CvInvoke.MorphologyEx(result, result, MorphOp.Close, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar(0));

            return result;
        }

        private static Mat normalizeImage(Mat img, Mat mask)
        {
            Mat result = new Mat(img.Size, DepthType.Cv64F, 1);

            MCvScalar mean = new MCvScalar();
            MCvScalar stddev = new MCvScalar();

            if (mask != null)
            {
                CvInvoke.MeanStdDev(img, ref mean, ref stddev, mask);
            }
            else
            {
                CvInvoke.MeanStdDev(img, ref mean, ref stddev);
            }

            img.ConvertTo(result, DepthType.Cv64F);
            result = (result - mean.V0) / stddev.V0;

            Mat normalizedWithinMask = new Mat();
            if (mask != null)
            {
                result.CopyTo(normalizedWithinMask, mask);
            }
            else
            {
                result.CopyTo(normalizedWithinMask);
            }

            Mat finalResult = new Mat(img.Size, DepthType.Cv64F, 1);
            finalResult.SetTo(new MCvScalar(0));
            if (mask != null)
            {
                normalizedWithinMask.CopyTo(finalResult, mask);
            }
            else
            {
                normalizedWithinMask.CopyTo(finalResult);
            }

            finalResult.ConvertTo(finalResult, DepthType.Cv8U, 255.0);

            return finalResult;
        }


        private static Mat convertToBinary(Mat img)
        {
            Mat binaryImg = new Mat();
            CvInvoke.AdaptiveThreshold(img, binaryImg, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 11, 2);
            return binaryImg;
        }

        private static Mat extractCenterRegion(Mat img, Mat mask, int width, int height)
        {
            Moments moments = CvInvoke.Moments(mask, true);
            int cx = (int)(moments.M10 / moments.M00);
            int cy = (int)(moments.M01 / moments.M00);

            int x = Math.Max(cx - width / 2, 0);
            int y = Math.Max(cy - height / 2, 0);

            x = Math.Min(x, img.Cols - width);
            y = Math.Min(y, img.Rows - height);

            x = (x / 8) * 8;
            y = (y / 8) * 8;

            if (x + width > img.Cols)
            {
                x = img.Cols - width;
            }

            if (y + height > img.Rows)
            {
                y = img.Rows - height;
            }

            Rectangle center = new Rectangle(x, y, width, height);
            return new Mat(img, center);
        }


        private static List<string> convertToAscii(Mat binaryImage)
        {
            List<string> patterns = new List<string>();
            IntPtr dataPtr = binaryImage.DataPointer;
            unsafe
            {
                byte* data = (byte*)dataPtr.ToPointer();
                for (int y = 0; y < binaryImage.Rows; y++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int x = 0; x < binaryImage.Cols; x += 8)
                    {
                        byte asciiValue = 0;
                        for (int bit = 0; bit < 8; bit++)
                        {
                            if (x + bit < binaryImage.Cols)
                            {
                                byte pixel = data[y * binaryImage.Step + x + bit];
                                if (pixel > 0)
                                {
                                    asciiValue |= (byte)(1 << (7 - bit));
                                }
                            }
                        }
                        sb.Append((char)asciiValue);
                    }
                    patterns.Add(sb.ToString());
                }
            }
            return patterns;
        }

        public static List<string> imgToPattern(string inputFilePath)
        {
            Mat img = CvInvoke.Imread(inputFilePath, ImreadModes.Grayscale);
            int blockSize = 16;
            double threshold = 0.6;
            var (normalizedImage, mask) = createSegmentedImg(img, blockSize, threshold);
            Mat binaryImage = convertToBinary(normalizedImage);
            Mat centerRegion = extractCenterRegion(binaryImage, mask, 64, 64);
            return convertToAscii(centerRegion);
        }

        public static string imgToText(string inputFilePath)
        {
            Mat img = CvInvoke.Imread(inputFilePath, ImreadModes.Grayscale);
            int blockSize = 16;
            double threshold = 0.6;
            var (normalizedImage, mask) = createSegmentedImg(img, blockSize, threshold);
            Mat binaryImage = convertToBinary(normalizedImage);
            List<string> patterns = convertToAscii(binaryImage);
            StringBuilder sb = new StringBuilder();
            foreach (string line in patterns)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}
