using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FormAI;


public class GeminiRequest
{
    [JsonPropertyName("contents")]
    public List<Content> Contents { get; set; } = new();
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; } = new ();
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; } 
}

public class Candidate
{
    [JsonPropertyName("content")] 
    public Content Content { get; set; }
}

public class Part
{
    [JsonPropertyName("text")]
    public string Text { get; set; } 
}

public static class MealGeneration
{
    private const string ApiKey = "AIzaSyAD_hplIkz6HEMD-F74wWKnig7FDAYqon8";
    private const string Model = "gemini-1.5-flash-latest";
    
    // Http Client
    
    private static HttpClient client = new();

    public static async Task<string> GenerateMealPlansAsync(User user, int numberOfDays)
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={ApiKey}";

        // Use a StringBuilder to dynamically construct the prompt based on user data.
        var promptBuilder = new StringBuilder();
        
        promptBuilder.AppendLine(@"
Jste výživový poradce, který je zodpovědný za vytvoření personalizovaného jídelníčku pro vašeho klienta.
Zde jsou důležité informace o klientovi:");
        promptBuilder.AppendLine($"- Jejich cíl je {user.Goal}");
        promptBuilder.AppendLine($"- Jejich pohlaví je {user.Gender}");
        promptBuilder.AppendLine($"- Jejich věk je {user.Age}");
        promptBuilder.AppendLine($"- Jejich výška je {user.Height} cm");
        promptBuilder.AppendLine($"- Jejich váha je {user.Weight} kg");

        if (!string.IsNullOrWhiteSpace(user.Allergies))
        {
            promptBuilder.AppendLine($"- **DŮLEŽITÉ ALERGIE (NUTNO SE VYVAROVAT):** {user.Allergies}");
        }

        if (!string.IsNullOrWhiteSpace(user.DislikedFoods))
        {
            promptBuilder.AppendLine($"- **Neoblíbená jídla (snažte se vyhnout):** {user.DislikedFoods}");
        }
        
        promptBuilder.Append($@"

**ÚKOL**:
Vygenerujte personalizovaný jídelníček na příštích {numberOfDays} dní. Plán by měl obsahovat Snídani, Oběd, Večeři a volitelně Svačiny.

**Pravidla formátování (MUSÍ DODRŽOVAT TATO PRAVIDLA)**:
- Celý výstup MUSÍ být ve formátu MARKDOWN.
- Pokud je {numberOfDays} 1, nepřidávejte nadpis jako Den 1...
- Pokud je {numberOfDays} více než 1, MUSÍTE vložit oddělovač '--- ---' na samostatný řádek mezi každým dnem.
- Pro každý den použijte kreativní nadpis shrnující téma denních jídel.
- Každé jídlo (Snídaně, Oběd, Večeře, Svačiny) musí být pod vlastním podnadpisem (### Snídaně).
- Pro každé jídlo:
  * Seznam potravin jako odrážky (* položka).
  * Poskytněte přesný recept nebo pokyny k přípravě.
  * Uveďte, zda lze jídlo skladovat a jak dlouho.


        string prompt = promptBuilder.ToString();

        try
        {
            var request = new GeminiRequest
            {
                Contents = { new Content { Parts = { new Part { Text = prompt } } } }
            };
            
            string json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpResponseMessage response = await client.PostAsync(url, httpContent);
            
            response.EnsureSuccessStatusCode();
            
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(jsonResponse);
            
            string resultText = geminiResponse?.Candidates?[0].Content?.Parts?[0].Text;

            return resultText ?? "An error occurred while parsing the AI response.";
        }
        catch (Exception ex)
        {
            return $"An error occurred while generating meal plans: {ex.Message}";
        }
    }


}