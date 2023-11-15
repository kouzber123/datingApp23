namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob) //ADD this method to DateOnly  property
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dob.Year;
            //check current year and - user date of birth to get age
            if (dob > today.AddYears(-age)) age--;  //if they already had birth day we take off years

            return age; //not super accurate calculation of age
        }
    }
}
