/// <summary>
/// Namespace <c>Split_Receipt.Constants</c> namespace contains an enum named SortOption,
/// which represents the different sorting options available in a system. It includes four possible values,
/// each representing a different way to sort data (by ascending or descending price, or by ascending or descending date).
/// </summary>
namespace Split_Receipt.Constants
{

    public enum SortOption
    {
        PriceAsc = 0,
        PriceDesc = 1,
        DateAsc = 2,
        DateDesc = 3
    }

    /// <summary>
    /// Class <c>SortOptionExtensions</c> class contains an extension method for the SortOption enum,
    /// which provides a string description of each option. The GetDescription method takes a SortOption
    /// value as input and returns a string that describes the corresponding sort option.
    /// This can be useful for displaying user-friendly sorting options in a UI or for generating descriptive error messages.
    /// If the input value is not one of the predefined options, the method throws an ArgumentOutOfRangeException.
    /// </summary>
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
