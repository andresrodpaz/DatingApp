namespace API.Extensions
{
    /// <summary>
    /// Provides extension methods for working with date and time.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates the age of a person based on their date of birth.
        /// </summary>
        /// <param name="dob">The date of birth as a <see cref="DateOnly"/> object.</param>
        /// <returns>The calculated age as an integer.</returns>
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dob.Year;

            // Check if the birthday has occurred this year
            if (dob > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
