namespace CardService.Application.Common.Helpers
{
    public static class Utils
    {
        static readonly Random random = new();
        static readonly char[] keys = "0123456789".ToCharArray();

        public static string GenerateCode(int lengthOfVoucher = 6)
        {
            return Enumerable
                .Range(1, lengthOfVoucher)
                .Select(k => keys[random.Next(0, keys.Length - 1)])
                .Aggregate("", (e, c) => e + c);
        }
    }
}
