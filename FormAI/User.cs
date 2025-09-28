namespace FormAI;

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    
    public bool HasSubmitedBioInfo { get; set; } 
    
    public Goal Goal { get; set; }
    public Gender Gender { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public int Age { get; set; }
    
    
    // Preferences
    public string DietaryPreference { get; set; } = "None";
    public string Allergies { get; set; } = "";
    public string DislikedFoods { get; set; } = ""; // Add this new property
    public int MealsPerDay { get; set; } = 3;
    
    
}

public enum Gender
{
    Male,
    Female,
    Other
}

public enum Goal
{
    WeightLoss,
    MuscleGain,
    MaintainWeight
}

