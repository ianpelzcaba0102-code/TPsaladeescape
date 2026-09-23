namespace sala_de_escape.Models;

public class GameProgress
{
    public string PlayerName { get; set; } = string.Empty;
    public int CurrentRoomIndex { get; set; }
    public List<int> SolvedNumbers { get; set; } = new();
    public bool IsCompleted { get; set; }
    public bool IsUnlocked { get; set; }
    public string FinalCode { get; set; } = string.Empty;
}
