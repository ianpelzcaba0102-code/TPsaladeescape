using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using sala_de_escape.Models;
using sala_de_escape.Services;

namespace sala_de_escape.Controllers;

public class HomeController : Controller
{
    private const string GameProgressKey = "EscapeGameProgress";

    private readonly ILogger<HomeController> _logger;
    private readonly EscapeGameService _gameService;

    public HomeController(ILogger<HomeController> logger, EscapeGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    public IActionResult Index()
    {
        var currentProgress = GetCurrentProgress();
        if (currentProgress != null && !currentProgress.IsCompleted)
        {
            return RedirectToAction(nameof(Room));
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Start(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            TempData["Error"] = "Ingresá tu nombre para comenzar la partida.";
            return View("Index");
        }

        var progress = new GameProgress
        {
            PlayerName = playerName.Trim(),
            CurrentRoomIndex = 0,
            SolvedNumbers = new List<int>()
        };

        SaveProgress(progress);
        return RedirectToAction(nameof(Room));
    }

    public IActionResult Room()
    {
        var progress = GetCurrentProgress();
        if (progress == null)
        {
            return RedirectToAction(nameof(Index));
        }

        if (progress.IsCompleted)
        {
            return RedirectToAction(nameof(Final));
        }

        if (progress.CurrentRoomIndex >= _gameService.Rooms.Count)
        {
            progress.IsCompleted = true;
            SaveProgress(progress);
            return RedirectToAction(nameof(Final));
        }

        var room = _gameService.GetRoom(progress.CurrentRoomIndex);
        ViewBag.PlayerName = progress.PlayerName;
        ViewBag.Progress = $"Sala {progress.CurrentRoomIndex + 1} de {_gameService.Rooms.Count}";
        return View(room);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitAnswer(string answer)
    {
        var progress = GetCurrentProgress();
        if (progress == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var room = _gameService.GetRoom(progress.CurrentRoomIndex);
        if (!string.Equals(answer?.Trim(), room.Answer, StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] = "Respuesta incorrecta. Intentá otra vez.";
            return View("Room", room);
        }

        if (!progress.SolvedNumbers.Contains(room.Number))
        {
            progress.SolvedNumbers.Add(room.Number);
        }

        if (progress.CurrentRoomIndex == _gameService.Rooms.Count - 1)
        {
            progress.IsCompleted = true;
            progress.FinalCode = _gameService.BuildFinalCode(progress.SolvedNumbers);
            SaveProgress(progress);
            return RedirectToAction(nameof(Final));
        }

        progress.CurrentRoomIndex++;
        SaveProgress(progress);
        TempData["Success"] = $"¡Correcto! Obtuviste el número {room.Number}.";
        return RedirectToAction(nameof(Room));
    }

    public IActionResult Final()
    {
        var progress = GetCurrentProgress();
        if (progress == null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(progress);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Unlock(string finalCode)
    {
        var progress = GetCurrentProgress();
        if (progress == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var expectedCode = _gameService.BuildFinalCode(progress.SolvedNumbers);
        if (string.Equals(finalCode?.Trim(), expectedCode, StringComparison.OrdinalIgnoreCase))
        {
            progress.IsUnlocked = true;
            SaveProgress(progress);
            return View("Victory", progress);
        }

        TempData["Error"] = "CONTRATO RECHAZADO.";
        return RedirectToAction(nameof(Final));
    }

    public IActionResult Restart()
    {
        HttpContext.Session.Remove(GameProgressKey);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private GameProgress? GetCurrentProgress()
    {
        var json = HttpContext.Session.GetString(GameProgressKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<GameProgress>(json);
    }

    private void SaveProgress(GameProgress progress)
    {
        var json = JsonSerializer.Serialize(progress);
        HttpContext.Session.SetString(GameProgressKey, json);
    }
}
