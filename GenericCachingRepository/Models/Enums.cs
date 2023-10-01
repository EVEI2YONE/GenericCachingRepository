namespace GenericCachingRepository.Models
{
    public enum SortOrder
    {
        Asc,
        Desc,
    }

    public enum ComparativeOperation
    {
        Like,
        NotLike,

        Equal,
        NotEqual,

        GreaterThan,
        NotGreaterThan,

        GreaterThanEqual,
        NotGreaterThanEqual,

        LessThan,
        NotLessThan,

        LessThanEqual,
        NotLessThanEqual
    }

    public enum RangeOperation
    {
        Between,
        NotBetween
    }
}
