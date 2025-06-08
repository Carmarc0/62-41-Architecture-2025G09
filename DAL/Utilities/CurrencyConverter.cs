namespace DAL.Utilities
{
    /// <summary>
    /// Currency conversion utility - CHF to pages (1 page = 0.08 CHF)
    /// </summary>
    public static class CurrencyConverter
    {
        private static readonly double CHF_PER_PAGE = 0.08;

        // Convert CHF amount to equivalent pages
        public static float ConvertCHFToPages(double chfAmount)
        {
            return (float)(chfAmount / CHF_PER_PAGE);
        }

        // Convert pages to CHF value
        public static double ConvertPagesToCHF(float pages)
        {
            return pages * CHF_PER_PAGE;
        }
    }
}