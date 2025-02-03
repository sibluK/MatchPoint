namespace TournamentManager.Utility;

public class Helpers
{
    public int? CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue)
            return 0;
        
        var age = DateTime.Today.Year - birthDate.Value.Year;
        
        if (birthDate.Value.Date > DateTime.Today.AddYears(-age)) 
            age--;

        return age;
    }
}