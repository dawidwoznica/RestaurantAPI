namespace RestaurantAPI.Exceptions
{
    [Serializable]
    internal class BadRequestException(string? message) : Exception(message);
}