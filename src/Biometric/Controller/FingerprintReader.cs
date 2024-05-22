using System;
using System.Drawing;
using System.Collections.Generic;

public class FingerprintReader
{
    // Converts a bitmap to a binary image using a specified threshold
    private Bitmap ConvertToBinary(Bitmap img, int threshold)
    {
        Bitmap binaryImage = new Bitmap(img.Width, img.Height);

        for (int y = 0; y < img.Height; y++)
        {
            for (int x = 0; x < img.Width; x++)
            {
                Color pixel = img.GetPixel(x, y);
                int avg = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

                binaryImage.SetPixel(x, y, avg < threshold ? Color.Black : Color.White);
            }
        }

        return binaryImage;
    }

    // Calculates the average value of a 3x3 area centered at (x, y)
    private double Calculate3x3Average(Bitmap img, int x, int y)
    {
        double sum = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                sum += img.GetPixel(x + i, y + j).R;
            }
        }

        return sum / 9 / 255.0;
    }

    // Counts the number of commutations (transitions between black and white pixels) in a w x w area centered at (x, y)
    private int CountCommutations(Bitmap img, int x, int y, int w)
    {
        int commutations = 0;
        for (int j = x - w / 2; j < x + w / 2; j++)
        {
            if ((img.GetPixel(j, y + w / 2) == Color.Black && img.GetPixel(j + 1, y + w / 2) == Color.White) ||
                (img.GetPixel(j, y + w / 2) == Color.White && img.GetPixel(j + 1, y + w / 2) == Color.Black))
            {
                commutations++;
            }
            if ((img.GetPixel(j, y - w / 2) == Color.Black && img.GetPixel(j + 1, y - w / 2) == Color.White) ||
                (img.GetPixel(j, y - w / 2) == Color.White && img.GetPixel(j + 1, y - w / 2) == Color.Black))
            {
                commutations++;
            }
        }

        for (int i = y - w / 2; i < y + w / 2; i++)
        {
            if ((img.GetPixel(x - w / 2, i) == Color.Black && img.GetPixel(x - w / 2, i + 1) == Color.White) ||
                (img.GetPixel(x - w / 2, i) == Color.White && img.GetPixel(x - w / 2, i + 1) == Color.Black))
            {
                commutations++;
            }
            if ((img.GetPixel(x + w / 2, i) == Color.Black && img.GetPixel(x + w / 2, i + 1) == Color.White) ||
                (img.GetPixel(x + w / 2, i) == Color.White && img.GetPixel(x + w / 2, i + 1) == Color.Black))
            {
                commutations++;
            }
        }

        return commutations;
    }

    // Computes the average value of the perimeter of a w x w area centered at (x, y)
    private double ComputePerimAvg(Bitmap img, int x, int y, int w)
    {
        int sum = 0;
        int count = 0;

        for (int i = -w / 2; i <= w / 2; i++)
        {
            for (int j = -w / 2; j <= w / 2; j++)
            {
                if (i == -w / 2 || i == w / 2 || j == -w / 2 || j == w / 2)
                {
                    sum += img.GetPixel(x + i, y + j).R;
                    count++;
                }
            }
        }

        return sum / (double)count / 255.0;
    }

    // Determines whether a pixel at (x, y) is a minutiae point
    private int IsMinutiae(Bitmap img, int x, int y)
    {
        double avg3X3 = Calculate3x3Average(img, x, y);
        Console.WriteLine("Average 3x3: {0}", avg3X3);
        if (avg3X3 < 0.25)
        {
            return 1; // Ridge ending
        }
        else if (avg3X3 > 0.75)
        {
            return 2; // Bifurcation
        }

        return -1; // Neither
    }

    // Gets a list of regions of interest (ROIs) from a binary image
    // Gets a list of regions of interest (ROIs) from a binary image and prints the binary image
    public static SortedDictionary<int, int> GetROIList(Bitmap img, double threshold)
    {
        FingerprintReader reader = new FingerprintReader(); // Create an instance of FingerprintReader
        Bitmap image = reader.ConvertToBinary(img, 128); // Call ConvertToBinary using the instance

        // Print the binary image to the console
        PrintBinaryImage(image);

        var roiList = new SortedDictionary<int, int>();
        int startIdx = -1;
        int count = 0;
        bool isROI = false;

        for (int j = 7; j < image.Width - 7; j++)
        {
            for (int i = 7; i < image.Height - 7; i++)
            {
                Console.WriteLine("Processing pixel ({0}, {1})", j, i);
                int type = reader.IsMinutiae(image, j, i); // Call IsMinutiae using the instance
                Console.WriteLine("Minutiae type: {0}", type);
                if (type != -1 && reader.CountCommutations(image, j, i, 15) >= 2) // Call CountCommutations using the instance
                {
                    if (type == 1 && reader.ComputePerimAvg(image, j, i, 15) > threshold) // Call ComputePerimAvg using the instance
                    {
                        isROI = true;
                    }
                    else if (type == 2 && reader.ComputePerimAvg(image, j, i, 15) < 1 - threshold) // Call ComputePerimAvg using the instance
                    {
                        isROI = true;
                    }

                if (isROI)
                {
                    count++;
                    if (startIdx == -1)
                    {
                        startIdx = i;
                    }
                }
                else
                {
                    if (count >= 10)
                    {
                        roiList.Add(startIdx, i - 1);
                    }
                    startIdx = -1;
                    count = 0;
                        
                }
                }
            }
        }

        return roiList;
    }

    // Prints the binary image to the console
    private static void PrintBinaryImage(Bitmap image)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                Console.Write(image.GetPixel(x, y) == Color.Black ? "X" : " ");
            }
            Console.WriteLine();
        }
    }


    public static void Main(string[] args)
    {
        Bitmap img = new Bitmap("9__M_Left_index_finger.BMP");
        SortedDictionary<int, int> roiList = GetROIList(img, 0.5);
        Console.WriteLine("Number of ROIs: {0}", roiList.Count);
        foreach (var roi in roiList)
        {
            Console.WriteLine("Start: {0}, End: {1}", roi.Key, roi.Value);
        }
    }
}
