#pragma warning disable IDE1006

using LoteriaMexicana.Domain;
using LoteriaMexicana.Services;
using LoteriaMexicana.Domain.Enums;
using System.Drawing.Drawing2D;
using System.Text.Json;

namespace LoteriaMexicana.Forms;

public partial class 
    FormJuegoRed : Form
{
    // ── Dependencias ──────────────────────────────────────────────────────────
    private readonly ClienteSignalR _cliente;
    private readonly ServidorSignalR? _servidor;
    private readonly string _miNombre;
    private readonly bool _esHost;

    // ── Servicios de imágenes ─────────────────────────────────────────────────
    private readonly ImagenService _imagenes;
    private readonly TtsService _tts = new();

    // ── Estado de la tabla ────────────────────────────────────────────────────
    private List<CasillaDto> _casillas = [];
    private List<int> _marcadas = [];
    private List<int> _cantadas = [];

    // ── Estado UI ─────────────────────────────────────────────────────────────
    private bool _juegoIniciado = false;
    private string _formatoActual = "TablaLlena";
    private int _cartasCantadas = 0;

    // ── Configuración de partida ──────────────────────────────────────────────
    private bool _permitirDobles = false;
    private JuegoService.TamañoTabla _tamañoTabla = JuegoService.TamañoTabla.Cuatro;

    // ── Fichas ────────────────────────────────────────────────────────────────
    private string? _fichaSeleccionada = null;
    private Panel? _panelFichaSeleccionada = null;
    private readonly Dictionary<int, string> _fichaEnCasilla = [];

    // ── Constantes de layout ──────────────────────────────────────────────────
    private const int CEL_W = 108;
    private const int CEL_H = 126;
    private int COLS => (int)_tamañoTabla;
    private int ROWS => (int)_tamañoTabla;
    private const int TOTAL_CARTAS = 54;

    private readonly List<ToolTip> _tooltips = [];

    // ── Fichas disponibles ────────────────────────────────────────────────────
    private static readonly string[] FichasImagenes =
        ["moneda10", "50c", "moeda1", "moneda2"];

    private static readonly string _carpetaFichas = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "Resources", "Fichas");

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================
    public FormJuegoRed(
        ClienteSignalR cliente,
        ServidorSignalR? servidor,
        string nombre,
        bool esHost)
    {
        InitializeComponent();
        _cliente = cliente;
        _servidor = servidor;
        _miNombre = nombre;
        _esHost = esHost;

        _imagenes = ImagenService.Instancia;

        btnIniciar.Click += btnIniciar_Click;
        btnCantarCarta.Click += btnCantarCarta_Click;
        btnLoteria.Click += btnLoteria_Click;
        btnNuevaTabla.Click += btnNuevaTabla_Click;
        btnGuardarTabla.Click += btnGuardarTabla_Click;
        btnCargarTabla.Click += btnCargarTabla_Click;
        btnCrearTabla.Click += btnCrearTabla_Click;

        btnReiniciarPartida.Click += btnReiniciarPartida_Click;
        btnEnviar.Click += btnEnviar_Click;
        txtMensaje.KeyDown += txtMensaje_KeyDown;
    }

    // =========================================================================
    // LOAD
    // =========================================================================
    private async void FormJuegoRed_Load(object? sender, EventArgs e)
    {
        lblUsuario.Text = $"👤  {_miNombre}";
        lblSalaInfo.Text = _esHost
            ? $"🎙️ Eres el Gritón  |  IP: {ServidorSignalR.ObtenerIpLocal()}"
            : "🎮  Eres jugador";

        AjustarVisibilidadHost();
        CargarFichas();
        SuscribirEventos();
        ActualizarContador();

        try
        {
            await _cliente.ConectarAsync();
            await _cliente.UnirseAlJuego(_miNombre);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"❌ Error de conexión: {ex.Message}", Color.Red);
        }
    }

    // =========================================================================
    // SUSCRIPCIÓN DE EVENTOS SignalR
    // =========================================================================
    private void SuscribirEventos()
    {
        _cliente.RolAsignado += (esHost, _) => InvokeUI(AjustarVisibilidadHost);
        _cliente.TablaAsignada += casillas => InvokeUI(() => OnTablaAsignada(casillas));
        _cliente.JuegoIniciado += formato => InvokeUI(() => OnJuegoIniciado(formato));
        _cliente.JuegoYaIniciado += (fmt, cs) => InvokeUI(() => OnJuegoYaIniciado(fmt, cs));
        _cliente.FormatoActual += fmt => InvokeUI(() => OnFormatoActual(fmt));
        _cliente.CartaCantada += (n, nm, f) => InvokeUI(() => OnCartaCantada(n, nm, f));
        _cliente.MarcasActualizadas += marcas => InvokeUI(() => OnMarcasActualizadas(marcas));
        _cliente.JugadoresActualizados += jugs => InvokeUI(() => OnJugadoresActualizados(jugs));
        _cliente.HayGanador += nombre => InvokeUI(() => OnHayGanador(nombre));
        _cliente.FalsaAlarma += () => InvokeUI(OnFalsaAlarma);
        _cliente.Trampa += numeros => InvokeUI(() => OnTrampa(numeros));
        _cliente.BarajaReiniciada += () => InvokeUI(OnBarajaReiniciada);
        _cliente.MensajeRecibido += (nm, msg) => InvokeUI(() => OnMensajeRecibido(nm, msg));
        _cliente.ConfiguracionActualizada += (tamaño, dobles) => InvokeUI(() => OnConfiguracionActualizada(tamaño, dobles));
    }

    // =========================================================================
    // HANDLERS SignalR
    // =========================================================================
    private void OnTablaAsignada(List<CasillaDto> casillas)
    {
        _casillas = casillas;
        _marcadas.Clear();
        _fichaEnCasilla.Clear();
        DibujarGrilla();
        MostrarMensaje("📋 Tabla asignada. ¡Prepárate!", Color.FromArgb(0, 104, 56));
    }

    private void OnJuegoIniciado(string formato)
    {
        _juegoIniciado = true;
        _formatoActual = formato;
        _cartasCantadas = 0;
        _cantadas.Clear();
        ActualizarContador();
        lblPatrones.Text = $"🏆  Formato: {FormatoLabel(formato)}";
        btnLoteria.Enabled = true;
        btnNuevaTabla.Enabled = true;
        btnGuardarTabla.Enabled = true;
        btnCantarCarta.Enabled = _esHost;
        btnReiniciarPartida.Enabled = _esHost;
        MostrarMensaje($"🎉 ¡Juego iniciado!  Formato: {FormatoLabel(formato)}", Color.FromArgb(0, 104, 56));
    }

    private void OnJuegoYaIniciado(string formato, List<CasillaDto> casillas)
    {
        _casillas = casillas;
        _formatoActual = formato;
        _juegoIniciado = true;
        DibujarGrilla();
        lblPatrones.Text = $"🏆  Formato: {FormatoLabel(formato)}";
        btnLoteria.Enabled = true;
        btnCantarCarta.Enabled = _esHost;
        MostrarMensaje("🔄 Te uniste a una partida en curso.", Color.DimGray);
    }

    private void OnFormatoActual(string formato)
    {
        _formatoActual = formato;
        lblPatrones.Text = $"🏆  Formato: {FormatoLabel(formato)}";
    }

    private void OnCartaCantada(int numero, string nombre, string frase)
    {
        _cantadas.Add(numero);
        _cartasCantadas++;
        ActualizarContador();
        lblNombreCarta.Text = nombre;
        lblFrase.Text = $"\"{frase}\"";
        picCarta.Image = _imagenes.ObtenerImagenCarta(numero);
        ResaltarCasilla(numero);
        AgregarAlHistorial(numero, nombre);
        if (chkTts.Checked)
            _ = Task.Run(() => _tts.CantarCarta(frase, nombre));
    }

    private void OnMarcasActualizadas(List<int> marcas)
    {
        _marcadas = marcas;
        RefrescarMarcasEnGrilla();
    }

    private void OnJugadoresActualizados(List<JugadorDto> jugadores)
    {
        lstJugadores.Items.Clear();
        foreach (var j in jugadores)
            lstJugadores.Items.Add(j);
        lblConectados.Text = $"Conectados: {jugadores.Count}";
    }

    private void OnHayGanador(string nombre)
    {
        _juegoIniciado = false;
        btnLoteria.Enabled = false;
        btnCantarCarta.Enabled = false;
        string msg = nombre == _miNombre
            ? "🏆 ¡GANASTE! ¡LOTERÍA!"
            : $"🏆 ¡{nombre} ganó la partida!";
        MostrarMensaje(msg, Color.FromArgb(180, 100, 0));
        MessageBox.Show(msg, "¡Lotería!", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnFalsaAlarma() =>
        MostrarMensaje("⚠️  Falsa alarma — tu tabla no cumple el formato.", Color.FromArgb(180, 60, 0));

    private void OnTrampa(List<int> numeros) =>
        MostrarMensaje($"🚨  Trampa en cartas: {string.Join(", ", numeros)}", Color.Red);

    private void OnBarajaReiniciada()
    {
        _juegoIniciado = false;
        _cartasCantadas = 0;
        _cantadas.Clear();
        _marcadas.Clear();
        _fichaEnCasilla.Clear();
        ActualizarContador();
        lblNombreCarta.Text = "— Ninguna —";
        lblFrase.Text = "";
        picCarta.Image = null;
        btnLoteria.Enabled = false;
        btnCantarCarta.Enabled = false;
        btnReiniciarPartida.Enabled = false;
        flpHistorial.Controls.Clear();
        RefrescarMarcasEnGrilla();
        MostrarMensaje("🔄  Baraja reiniciada. El Gritón puede iniciar nueva partida.", Color.DimGray);
    }

    private void OnMensajeRecibido(string nombre, string mensaje) =>
        AgregarAlChat(nombre, mensaje);

    private void OnConfiguracionActualizada(int tamaño, bool dobles)
    {
        _tamañoTabla = (JuegoService.TamañoTabla)tamaño;
        _permitirDobles = dobles;
        string msgTabla = tamaño == 5 ? "5×5" : "4×4";
        string msgDobles = dobles ? "con dobles" : "sin dobles";
        MostrarMensaje($"⚙️  Configuración: Tabla {msgTabla}, {msgDobles}", Color.FromArgb(80, 80, 160));
    }

    // =========================================================================
    // DIBUJAR GRILLA
    // =========================================================================
    private void DibujarGrilla()
    {
        foreach (var tt in _tooltips) tt.Dispose();
        _tooltips.Clear();

        grilla.SuspendLayout();
        grilla.Controls.Clear();
        grilla.RowCount = ROWS;
        grilla.ColumnCount = COLS;
        grilla.RowStyles.Clear();
        grilla.ColumnStyles.Clear();

        for (int i = 0; i < COLS; i++)
            grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, CEL_W));
        for (int i = 0; i < ROWS; i++)
            grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, CEL_H));

        for (int i = 0; i < _casillas.Count && i < ROWS * COLS; i++)
            grilla.Controls.Add(CrearCelda(_casillas[i]), i % COLS, i / COLS);

        grilla.ResumeLayout(true);
    }
    
    private Panel CrearCelda(CasillaDto casilla)
    {
        bool cantada = _cantadas.Contains(casilla.Numero);
        bool marcada = _marcadas.Contains(casilla.Numero);
        _fichaEnCasilla.TryGetValue(casilla.Numero, out string? ficha);

        var pnl = new Panel
        {
            Width = CEL_W - 4,
            Height = CEL_H - 4,
            Margin = new Padding(2),
            BackColor = marcada ? Color.FromArgb(255, 245, 180)
                      : cantada ? Color.FromArgb(220, 255, 220)
                      : Color.White,
            Cursor = Cursors.Hand,
            Tag = casilla.Numero
        };

        pnl.Paint += (s, e) =>
        {
            var col = marcada ? Color.FromArgb(240, 185, 11)
                    : cantada ? Color.FromArgb(0, 150, 80)
                    : Color.FromArgb(200, 200, 200);
            using var pen = new Pen(col, marcada ? 2.5f : 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
        };

        var pic = new PictureBox
        {
            Size = new Size(CEL_W - 12, CEL_H - 36),
            Location = new Point(4, 4),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Image = _imagenes.ObtenerImagenCarta(casilla.Numero)
        };

        var lbl = new Label
        {
            Text = casilla.Nombre,
            Font = new Font("Segoe UI", 6.5f, FontStyle.Bold),
            ForeColor = marcada ? Color.FromArgb(140, 80, 0)
                      : cantada ? Color.FromArgb(0, 104, 56)
                      : Color.FromArgb(60, 60, 60),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Bottom,
            Height = 26,
            BackColor = Color.Transparent
        };

        if (ficha != null)
        {
            var rutaFicha = Path.Combine(_carpetaFichas, $"{ficha}.png");
            var picFicha = new PictureBox
            {
                Size = new Size(44, 44),
                Location = new Point((CEL_W - 44) / 2, (CEL_H - 60) / 2),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            if (File.Exists(rutaFicha))
                picFicha.Image = Image.FromFile(rutaFicha);
            pnl.Controls.Add(picFicha);
            picFicha.BringToFront();
            picFicha.Click += async (s, e) => await OnCeldaClick(casilla.Numero);
        }

        pnl.Controls.Add(pic);
        pnl.Controls.Add(lbl);

        var tip = new ToolTip();
        tip.SetToolTip(pnl, $"#{casilla.Numero} — {casilla.Nombre}");
        _tooltips.Add(tip);

        pnl.Click += async (s, e) => await OnCeldaClick(casilla.Numero);
        foreach (Control c in pnl.Controls)
            c.Click += async (s, e) => await OnCeldaClick(casilla.Numero);

        return pnl;
    }

    private async Task OnCeldaClick(int numero)
    {
        if (!_juegoIniciado) return;

        if (_fichaSeleccionada != null)
        {
            if (!_fichaEnCasilla.Remove(numero))
                _fichaEnCasilla[numero] = _fichaSeleccionada;
            DibujarGrilla();
        }

        if (_cantadas.Contains(numero))
            await _cliente.ToggleCarta(numero);
    }

    private void RefrescarMarcasEnGrilla()
    {
        foreach (Control ctrl in grilla.Controls)
        {
            if (ctrl is not Panel pnl || pnl.Tag is not int numero) continue;
            bool marcada = _marcadas.Contains(numero);
            bool cantada = _cantadas.Contains(numero);
            pnl.BackColor = marcada ? Color.FromArgb(255, 245, 180)
                          : cantada ? Color.FromArgb(220, 255, 220)
                          : Color.White;
            pnl.Invalidate();
            foreach (Control c in pnl.Controls)
                if (c is Label lbl && lbl.Dock == DockStyle.Bottom)
                    lbl.ForeColor = marcada ? Color.FromArgb(140, 80, 0)
                                  : cantada ? Color.FromArgb(0, 104, 56)
                                  : Color.FromArgb(60, 60, 60);
        }
    }

    private void ResaltarCasilla(int numero)
    {
        foreach (Control ctrl in grilla.Controls)
            if (ctrl is Panel pnl && pnl.Tag is int n && n == numero)
            {
                pnl.BackColor = Color.FromArgb(220, 255, 220);
                pnl.Invalidate();
            }
    }

    // =========================================================================
    // FICHAS
    // =========================================================================
    private void CargarFichas()
    {
        flpFichas.Controls.Clear();
        foreach (var nombre in FichasImagenes)
        {
            var pnl = new Panel
            {
                Size = new Size(68, 68),
                Margin = new Padding(4),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = nombre
            };

            var pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Tag = nombre
            };

            var ruta = Path.Combine(_carpetaFichas, $"{nombre}.png");
            if (File.Exists(ruta))
                pic.Image = Image.FromFile(ruta);

            pnl.Controls.Add(pic);

            pnl.Paint += (s, e) =>
            {
                bool sel = _fichaSeleccionada == nombre;
                using var pen = new Pen(
                    sel ? Color.FromArgb(206, 17, 38) : Color.FromArgb(220, 220, 220),
                    sel ? 3f : 1f);
                e.Graphics.DrawRectangle(pen, 1, 1, pnl.Width - 3, pnl.Height - 3);
            };

            pnl.Click += (s, e) => SeleccionarFicha(nombre, pnl);
            pic.Click += (s, e) => SeleccionarFicha(nombre, pnl);
            flpFichas.Controls.Add(pnl);
        }
    }

    private void SeleccionarFicha(string emoji, Panel pnl)
    {
        _panelFichaSeleccionada?.Invalidate();
        if (_fichaSeleccionada == emoji)
        {
            _fichaSeleccionada = null;
            _panelFichaSeleccionada = null;
        }
        else
        {
            _fichaSeleccionada = emoji;
            _panelFichaSeleccionada = pnl;
        }
        pnl.Invalidate();
    }

    // =========================================================================
    // HISTORIAL
    // =========================================================================
    private void AgregarAlHistorial(int numero, string nombre)
    {
        var pnl = new Panel
        {
            Size = new Size(60, 70),
            Margin = new Padding(3),
            BackColor = Color.White
        };
        var pic = new PictureBox
        {
            Size = new Size(56, 52),
            Location = new Point(2, 2),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = _imagenes.ObtenerImagenCarta(numero),
            BackColor = Color.Transparent
        };
        var lbl = new Label
        {
            Text = nombre.Length > 8 ? nombre[..8] : nombre,
            Font = new Font("Segoe UI", 6f),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Bottom,
            Height = 14,
            BackColor = Color.Transparent
        };
        pnl.Controls.Add(pic);
        pnl.Controls.Add(lbl);
        flpHistorial.Controls.Add(pnl);
        flpHistorial.ScrollControlIntoView(pnl);
    }

    // =========================================================================
    // CHAT
    // =========================================================================
    private void AgregarAlChat(string nombre, string mensaje)
    {
        bool mio = nombre == _miNombre;
        rtbChat.SelectionStart = rtbChat.TextLength;
        rtbChat.SelectionLength = 0;
        rtbChat.SelectionColor = mio ? Color.FromArgb(0, 104, 56) : Color.FromArgb(206, 17, 38);
        rtbChat.AppendText($"{nombre}: ");
        rtbChat.SelectionColor = Color.FromArgb(40, 40, 40);
        rtbChat.AppendText($"{mensaje}\n");
        rtbChat.ScrollToCaret();
    }

    // =========================================================================
    // BOTONES
    // =========================================================================
    private async void btnIniciar_Click(object? sender, EventArgs e)
    {
        if (!_esHost) return;

        using var dlg = new FormSeleccionFormato(
            permitirDobles: _permitirDobles,
            tamañoTabla: _tamañoTabla);

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        string formato = dlg.FormatoElegido;
        _permitirDobles = dlg.PermitirDobles;
        _tamañoTabla = dlg.TamañoTabla;

        try
        {
            btnIniciar.Enabled = false;
            await _cliente.ConfigurarPartida(formato, _permitirDobles, (int)_tamañoTabla);
            await _cliente.IniciarJuego(formato);
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error al iniciar: {ex.Message}", Color.Red);
            btnIniciar.Enabled = true;
        }
    }

    private async void btnCantarCarta_Click(object? sender, EventArgs e)
    {
        if (!_esHost || !_juegoIniciado) return;
        try
        {
            btnCantarCarta.Enabled = false;
            await _cliente.CantarCarta();
            await Task.Delay(300);
            btnCantarCarta.Enabled = _juegoIniciado;
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", Color.Red);
            btnCantarCarta.Enabled = true;
        }
    }

    private async void btnLoteria_Click(object? sender, EventArgs e)
    {
        if (!_juegoIniciado) return;
        try
        {
            btnLoteria.Enabled = false;
            await _cliente.ReclamarLoteria();
            await Task.Delay(1500);
            btnLoteria.Enabled = _juegoIniciado;
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", Color.Red);
            btnLoteria.Enabled = true;
        }
    }

    private async void btnNuevaTabla_Click(object? sender, EventArgs e)
    {
        if (_juegoIniciado)
        {
            var r = MessageBox.Show("¿Cambiar tabla con partida en curso?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r != DialogResult.Yes) return;
        }
        await _cliente.PedirNuevaTabla();
    }

    private void btnGuardarTabla_Click(object? sender, EventArgs e)
    {
        if (_casillas.Count == 0) return;
        using var dlg = new SaveFileDialog
        {
            Filter = "Tabla Lotería (*.loteria)|*.loteria",
            FileName = $"tabla_{_miNombre}.loteria"
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        File.WriteAllText(dlg.FileName, JsonSerializer.Serialize(_casillas));
        MostrarMensaje("💾  Tabla guardada.", Color.FromArgb(0, 104, 56));
    }

    private void btnCargarTabla_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Filter = "Tabla Lotería (*.loteria)|*.loteria" };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        try
        {
            var casillas = JsonSerializer.Deserialize<List<CasillaDto>>(
                File.ReadAllText(dlg.FileName));
            if (casillas == null || casillas.Count == 0) return;
            _casillas = casillas;
            _marcadas.Clear();
            _fichaEnCasilla.Clear();
            DibujarGrilla();
            MostrarMensaje("📂  Tabla cargada.", Color.FromArgb(0, 104, 56));
        }
        catch { MostrarMensaje("❌  No se pudo cargar la tabla.", Color.Red); }
    }
    private async void btnCrearTabla_Click(object? sender, EventArgs e)
    {
        int tamaño = (int)_tamañoTabla;
        using var dlg = new FormElegirCartas(tamaño);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            btnCrearTabla.Enabled = false;
            await _cliente.EnviarTablaPersonalizada(dlg.NumerosElegidos);
            MostrarMensaje("🃏  Tabla personalizada aplicada.", Color.FromArgb(0, 104, 56));
        }
        catch (Exception ex)
        {
            MostrarMensaje($"Error: {ex.Message}", Color.Red);
        }
        finally
        {
            btnCrearTabla.Enabled = true;
        }
    }

    
   

   
    private async void btnReiniciarPartida_Click(object? sender, EventArgs e)
    {
        if (!_esHost) return;
        var r = MessageBox.Show("¿Reiniciar la baraja para una nueva partida?",
            "Reiniciar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (r != DialogResult.Yes) return;
        await _cliente.ReiniciarBaraja();
    }

    private async void btnEnviar_Click(object? sender, EventArgs e) =>
        await EnviarMensajeChat();

    private async void txtMensaje_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        await EnviarMensajeChat();
    }

    private async Task EnviarMensajeChat()
    {
        var msg = txtMensaje.Text.Trim();
        if (string.IsNullOrEmpty(msg)) return;
        await _cliente.EnviarMensaje(msg);
        txtMensaje.Clear();
    }

    // =========================================================================
    // HELPERS
    // =========================================================================
    private void AjustarVisibilidadHost()
    {
        pnlConfigHost.Visible = _esHost;
        btnCantarCarta.Visible = _esHost;
        btnCantarCarta.Enabled = _esHost && _juegoIniciado;
        btnReiniciarPartida.Visible = _esHost;
        btnReiniciarPartida.Enabled = _esHost && _juegoIniciado;
        btnCrearTabla.Visible = _esHost;
    }

    private void MostrarMensaje(string texto, Color color)
    {
        lblMensaje.Text = texto;
        lblMensaje.ForeColor = color;
    }

    private void ActualizarContador()
    {
        int restantes = TOTAL_CARTAS - _cartasCantadas;
        lblContadorCartas.Text =
            $"🃏  Cartas: {_cartasCantadas}/{TOTAL_CARTAS}  ({restantes} restantes)";
    }

    private static string FormatoLabel(string fmt) => fmt switch
    {
        "FilaCompleta" => "Fila completa",
        "ColumnaCompleta" => "Columna completa",
        "DiagonalCompleta" => "Diagonal",
        "EsquinasCompletas" => "4 Esquinas",
        "CruzCentral" => "Cruz central",
        "FormaTee" => "Forma T",
        "MarcoCompleto" => "Marco exterior",
        "MarcoInterior" => "Marco interior (5×5)",
        "DosDiagonales" => "Doble diagonal (X)",
        "PrimeraCarta" => "Primera carta",
        "TresColumnas" => "Tres columnas",
        "TodasLasFormas" => "⭐ Todas las formas",
        _ => fmt
    };

    private void InvokeUI(Action accion)
    {
        if (IsDisposed) return;
        if (InvokeRequired) Invoke(accion);
        else accion();
    }

    // =========================================================================
    // DRAW ITEM jugadores
    // =========================================================================
    private void lstJugadores_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= lstJugadores.Items.Count) return;
        e.DrawBackground();
        bool sel = (e.State & DrawItemState.Selected) != 0;

        using var brushFondo = new SolidBrush(sel
            ? Color.FromArgb(206, 17, 38)
            : (e.Index % 2 == 0 ? Color.White : Color.FromArgb(250, 248, 240)));
        e.Graphics.FillRectangle(brushFondo, e.Bounds);

        string texto = lstJugadores.Items[e.Index] is JugadorDto j
            ? $"{(j.EsGriton ? "🎙️" : "🎮")}  {j.Nombre}  ★{j.Victorias}"
            : lstJugadores.Items[e.Index]?.ToString() ?? "";

        using var brushTexto = new SolidBrush(sel ? Color.White : Color.FromArgb(40, 40, 40));
        e.Graphics.DrawString(texto, e.Font ?? lstJugadores.Font, brushTexto,
            new RectangleF(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height),
            new StringFormat { LineAlignment = StringAlignment.Center });
    }

    // =========================================================================
    // STUBS DISEÑADOR
    // =========================================================================
    private void pnlTopBar_Paint(object? sender, PaintEventArgs e) { }
    private void lblFrase_Click(object? sender, EventArgs e) { }
    private void flpFichas_Paint(object? sender, PaintEventArgs e) { }
    private void lblChatTitulo_Click(object? sender, EventArgs e) { }

    // =========================================================================
    // CIERRE
    // =========================================================================
    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        try
        {
            _tts.Dispose();
            await _cliente.DisposeAsync();
            if (_servidor != null)
                await _servidor.DisposeAsync();
        }
        catch { /* ignorar errores al cerrar */ }
        foreach (var tt in _tooltips) tt.Dispose();
    }
}