using System.Speech.Synthesis;

namespace LoteriaMexicana.Services;

public sealed class TtsService : IDisposable
{
    private static readonly TtsService _instancia = new();
    private readonly SpeechSynthesizer _synth = new();
    private bool _disposed;
    public bool Habilitado { get; set; } = true;

    public TtsService()
    {
        _synth.SetOutputToDefaultAudioDevice();
        _synth.Rate = -1;
        var voz = _synth.GetInstalledVoices()
            .FirstOrDefault(v => v.VoiceInfo.Culture.Name.StartsWith("es", StringComparison.OrdinalIgnoreCase));
        if (voz != null) _synth.SelectVoice(voz.VoiceInfo.Name);
    }

    /// <summary>
    /// Método estático para llamar desde cualquier parte sin instancia.
    /// Equivalente al TtsService.Hablar(...) usado en FormJuegoRed.
    /// </summary>
    public static void Hablar(string texto)
    {
        if (!_instancia.Habilitado) return;
        _instancia._synth.SpeakAsyncCancelAll();
        _instancia._synth.SpeakAsync(texto);
    }

    /// <summary>
    /// Canta una carta con su frase y nombre (uso con instancia).
    /// </summary>
    public void CantarCarta(string frase, string nombre)
    {
        if (!Habilitado) return;
        _synth.SpeakAsyncCancelAll();
        _synth.SpeakAsync($"{frase}... {nombre}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _synth.SpeakAsyncCancelAll();
        _synth.Dispose();
        _disposed = true;
    }
}