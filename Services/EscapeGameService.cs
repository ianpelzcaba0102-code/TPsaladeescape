using sala_de_escape.Models;

namespace sala_de_escape.Services;

public class EscapeGameService
{
    private readonly List<GameRoom> _rooms = new()
    {
        new GameRoom
        {
            Order = 1,
            Title = "Formación",
            Description = "Una pizarra táctica muestra una formación incompleta. Debés descubrir el número escondido en el orden de las posiciones.",
            Prompt = "¿Cuál es el número que aparece cuando ordenás la formación correctamente?",
            Hint = "Fijate en el orden de las posiciones y el símbolo que aparece más veces.",
            Answer = "7",
            Number = 7
        },
        new GameRoom
        {
            Order = 2,
            Title = "Casilleros",
            Description = "Los casilleros tienen marcas y cada jugador dejó una pista en su espacio. La clave es comparar la información de todos.",
            Prompt = "¿Qué número aparece al juntar las pistas de los casilleros?",
            Hint = "Busca la cifra que se repite en las tarjetas y luego ordena los casilleros.",
            Answer = "3",
            Number = 3
        },
        new GameRoom
        {
            Order = 3,
            Title = "El Partido",
            Description = "Un partido hipotético aparece en la pantalla con pases, tiros y corners. El patrón es la clave para encontrar el código.",
            Prompt = "¿Cuál es el número del patrón correcto que aparece en el partido?",
            Hint = "El patrón es lógico: observa la secuencia y no te enfoques solo en la acción del balón.",
            Answer = "9",
            Number = 9
        },
        new GameRoom
        {
            Order = 4,
            Title = "Mensaje oculto",
            Description = "Los detalles del container escondían un mensaje. Ahora podés ver la secuencia completa y llegar al número final.",
            Prompt = "¿Cuál es el último número que completa la combinación?",
            Hint = "La respuesta está en la última pista que ya tenías frente a vos.",
            Answer = "2",
            Number = 2
        }
    };

    public IReadOnlyList<GameRoom> Rooms => _rooms;

    public GameRoom GetRoom(int index)
    {
        return _rooms[index];
    }

    public string BuildFinalCode(IEnumerable<int> numbers)
    {
        return string.Concat(numbers.Select(n => Math.Abs(n).ToString()));
    }
}
