namespace Split_Receipt.Constants
{
    public enum SortOption
    {
        PriceAsc = 0,
        PriceDesc = 1,
        DateAsc = 2,
        DateDesc = 3
    }

    public static class SortOptionExtensions
    {
        public static string GetDescription(this SortOption sortOption)
        {
            switch (sortOption)
            {
                case SortOption.PriceAsc:
                    return "Price ASC";
                case SortOption.PriceDesc:
                    return "Price DESC";
                case SortOption.DateAsc:
                    return "Date ASC";
                case SortOption.DateDesc:
                    return "Date DESC";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOption), sortOption, null);
            }
        }
    }
}
