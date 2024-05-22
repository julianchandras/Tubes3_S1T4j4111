using System;

public class BM
{
    public static int bmMatch(string text, string pattern)
    {
        int n = text.Length;
        int m = pattern.Length;
        int[] last = buildLast(pattern);
        int i = m - 1;
        if (i > n - 1)
        {
            return -1;
        }
        int j = m - 1;
        do
        {
            if (pattern[j] == text[i])
            {
                if (j == 0)
                    return i;
                else
                {
                    i--;
                    j--;
                }
            }
            else
            {
                i = i + m - Math.Min(j, 1 + last[text[i]]);
                j = m - 1;
            }
        } while (i <= n - 1);
        return -1;
    }

    public static int[] buildLast(string pattern)
    {
        const int N = 256;
        int[] last = new int[N];
        for (int i = 0; i < N; i++)
            last[i] = -1;
        for (int i = 0; i < pattern.Length; i++)
            last[pattern[i]] = i;
        return last;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine(bmMatch("ababcabcacbab", "abcac"));
    }
}
