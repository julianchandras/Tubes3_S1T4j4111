namespace Biometric.Algorithms
{
    class Levenshtein
    {
        public static int levenshteinDistance(string string1, string string2)
        {
            int[,] d = new int[string1.Length + 1, string2.Length + 1];

            for (int i = 0; i <= string1.Length; i++)
                d[i, 0] = i;

            for (int j = 0; j <= string2.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= string2.Length; j++)
            {
                for (int i = 1; i <= string1.Length; i++)
                {
                    int cost = (string1[i - 1] == string2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[string1.Length, string2.Length];
        }
    }
}
