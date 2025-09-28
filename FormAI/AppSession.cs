using System.Windows;

namespace FormAI;

public static class AppSession
{
    // Holds the info about the which user is logged in. 
    public static User? CurrentUser { get; set; }
    
}