using System.Speech.Synthesis;

namespace LoteriaMexicana.Services;

public sealed class TtsService : IDisposable
{
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

    public void CantarCarta(string frase, string nombre)
    {
        if (!Habilitado) return;
        _synth.SpeakAsyncCancelAll();
        _synth.SpeakAsync($"{frase}... {nombre}");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _synth.SpeakAsyncCancelAll(); _synth.Dispose(); _disposed = true;
    }
}
