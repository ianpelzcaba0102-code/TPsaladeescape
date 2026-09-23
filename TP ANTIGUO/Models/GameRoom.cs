namespace sala_de_escape.Models;

public class GameRoom
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public int Number { get; set; }
}
