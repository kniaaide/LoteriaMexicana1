using LoteriaMexicana.Domain;
using LoteriaMexicana.Services;
using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Hubs;
using System.Drawing.Drawing2D;
using System.Text.Json;

namespace LoteriaMexicana.Forms;

public partial class FormJuegoRed : Form
{
    // ── Servicios ─────────────────────────────────────────────────────────────
    private readonly ClienteSignalR _cliente;
    private readonly ServidorSignalR? _servidor;
    private readonly TtsService _tts;
    private readonly ImagenService _imagenes;
    private readonly FichaService _fichas;
    private readonly string _miNombre;
    private readonly bool _esHost;

    // ── Estado local ──────────────────────────────────────────────────────────
    private readonly Dictionary<int, List<string>> _fichasEnTabla = new();
    private bool _juegoIniciado = false;

    private string? _fichaSeleccionada = null;
    private Panel? _panelFichaSeleccionada = null;

    private const int CEL_W = 110;
    private const int CEL_H = 128;
    private const int TAM_FICHA_PANEL = 70;

    private const int TOTAL_CARTAS = 54;
    private int _cartasCantadas = 0;

    private readonly List<ToolTip> _tooltips = new();
    private readonly string _carpetaTablas;

    // ── Paleta visual ─────────────────────────────────────────────────────────
    private static class Paleta
    {
        public static readonly Color Fondo          = Color.FromArgb(18, 18, 24);
        public static readonly Color Superficie     = Color.FromArgb(28, 28, 38);
        public static readonly Color SuperficieAlt  = Color.FromArgb(38, 38, 52);
        public static readonly Color Acento         = Color.FromArgb(255, 75, 75);
        public static readonly Color AcentoVerde    = Color.FromArgb(0, 210, 130);
        public static readonly Color AcentoAmbar    = Color.FromArgb(255, 190, 50);
        public static readonly Color TextoPrimario  = Color.FromArgb(240, 240, 248);
        public static readonly Color TextoSecund    = Color.FromArgb(150, 150, 170);
        public static readonly Color Borde          = Color.FromArgb(55, 55, 75);
        public static readonly Color Seleccionado   = Color.FromArgb(80, 160, 255);
        public static readonly Color Ganador        = Color.FromArgb(255, 215, 0);   // oro
    }

    // Todos los patrones ganadores — cada jugador puede ganar con CUALQUIERA
    private static readonly (string Nombre, FormatoGanador Valor)[] TodosLosFormatos =
    {
        ("Línea H",     FormatoGanador.LineaHorizontal),
        ("Línea V",     FormatoGanador.LineaVertical),
        ("Diagonal",    FormatoGanador.Diagonal),
        ("Cruz",        FormatoGanador.Cruz),
        ("Cruzita",     FormatoGanador.Cruzita),
        ("Tabla Llena", FormatoGanador.TablaLlena),
    };

    // Lleva registro de los ganadores de la ronda actual
    private readonly List<(string Nombre, string Patron)> _ganadoresRonda = new();

    public FormJuegoRed(ClienteSignalR cliente, ServidorSignalR? servidor,
                        string nombre, bool esHost)
    {
        _cliente  = cliente;
        _servidor = servidor;
        _miNombre = nombre;
        _esHost   = esHost;

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        _carpetaTablas = Path.Combine(baseDir, "Tablas");
        Directory.CreateDirectory(_carpetaTablas);

        _tts      = new TtsService();
        _imagenes = new ImagenService(Path.Combine(baseDir, "Resources", "Cartas"));
        _fichas   = new FichaService(Path.Combine(baseDir, "Resources", "Fichas"));
        _fichas.Precargar();

        InitializeComponent();
        WindowState = FormWindowState.Maximized;

        Text         = $"Lotería Mexicana — {nombre}{(esHost ? " (Gritón)" : "")}";
        lblUsuario.Text = $"{nombre}{(esHost ? "  🎤 Gritón" : "")}";

        if (esHost)
            lblSalaInfo.Text = $"📡  {ServidorSignalR.ObtenerIpLocal()}:{ServidorSignalR.Puerto}";
        else
        {
            btnCantarCarta.Visible   = false;
            pnlConfigHost.Visible    = false;
        }

        ActualizarLabelPatrones();
        ConstruirPanelFichas();
        SuscribirSignalR();
        BindearUI();
        AplicarTema();

        Shown += async (_, _) => await UnirseAsync();
    }

    // =========================================================================
    // PATRONES
    // =========================================================================

    private void ActualizarLabelPatrones()
    {
        if (lblPatrones == null) return;
        lblPatrones.Text = "🏆 Ganás con: " +
            string.Join("  •  ", TodosLosFormatos.Select(f => f.Nombre));
    }

    // =========================================================================
    // TEMA VISUAL
    // =========================================================================

    private void AplicarTema()
    {
        BackColor = Paleta.Fondo;
        ForeColor = Paleta.TextoPrimario;

        pnlTopBar.BackColor = Paleta.Superficie;
        pnlTopBar.Paint    += PintarBordeSuperior;

        lblUsuario.ForeColor = Paleta.TextoPrimario;
        lblUsuario.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);

        lblSalaInfo.ForeColor = Paleta.AcentoVerde;
        lblSalaInfo.Font      = new Font("Segoe UI", 9f);

        lblMensaje.ForeColor = Paleta.TextoSecund;
        lblMensaje.Font      = new Font("Segoe UI", 9f, FontStyle.Italic);

        if (lblPatrones != null)
        {
            lblPatrones.ForeColor = Paleta.AcentoAmbar;
            lblPatrones.Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        }

        pnlConfigHost.BackColor = Paleta.Superficie;
        EstilarBoton(btnIniciar,            Paleta.AcentoVerde,               Color.FromArgb(10, 30, 20));
        EstilarBoton(btnCantarCarta,        Paleta.Acento,                    Color.White);
        EstilarBoton(btnLoteria,            Paleta.AcentoAmbar,               Color.FromArgb(30, 20, 0));
        EstilarBoton(btnNuevaTabla,         Paleta.SuperficieAlt,             Paleta.TextoPrimario, borde: true);
        EstilarBoton(btnGuardarTabla,       Color.FromArgb(50, 80, 140),      Color.White);
        EstilarBoton(btnCargarTabla,        Color.FromArgb(80, 50, 120),      Color.White);
        EstilarBoton(btnReiniciarPartida,   Color.FromArgb(180, 100, 0),      Color.White);

        picCarta.BackColor = Paleta.Superficie;
        picCarta.BorderStyle = BorderStyle.None;
        lblNombreCarta.ForeColor = Paleta.TextoPrimario;
        lblNombreCarta.Font      = new Font("Segoe UI", 13f, FontStyle.Bold);
        lblFrase.ForeColor       = Paleta.AcentoAmbar;
        lblFrase.Font            = new Font("Segoe UI", 9.5f, FontStyle.Italic);

        flpHistorial.BackColor = Paleta.Superficie;
        flpFichas.BackColor    = Paleta.Superficie;

        grilla.BackColor         = Paleta.Fondo;
        grilla.CellBorderStyle   = TableLayoutPanelCellBorderStyle.None;

        lblConectados.ForeColor = Paleta.TextoSecund;
        lblConectados.Font      = new Font("Segoe UI", 8.5f);
        lstJugadores.BackColor  = Paleta.Superficie;
        lstJugadores.ForeColor  = Paleta.TextoPrimario;
        lstJugadores.BorderStyle= BorderStyle.None;
        lstJugadores.Font       = new Font("Segoe UI", 9.5f);

        rtbChat.BackColor    = Paleta.Superficie;
        rtbChat.ForeColor    = Paleta.TextoPrimario;
        rtbChat.BorderStyle  = BorderStyle.None;
        rtbChat.Font         = new Font("Segoe UI", 9.5f);

        txtMensaje.BackColor  = Paleta.SuperficieAlt;
        txtMensaje.ForeColor  = Paleta.TextoPrimario;
        txtMensaje.BorderStyle= BorderStyle.FixedSingle;
        txtMensaje.Font       = new Font("Segoe UI", 10f);

        EstilarBoton(btnEnviar, Paleta.Acento, Color.White);

        chkTts.ForeColor  = Paleta.TextoSecund;
        chkTts.BackColor  = Color.Transparent;
        chkTts.Font       = new Font("Segoe UI", 8.5f);

        if (lblFichaHint != null)
        {
            lblFichaHint.ForeColor = Paleta.TextoSecund;
            lblFichaHint.Font      = new Font("Segoe UI", 8f, FontStyle.Italic);
        }

        if (lblContadorCartas != null)
        {
            lblContadorCartas.ForeColor = Paleta.TextoSecund;
            lblContadorCartas.Font      = new Font("Segoe UI", 8.5f);
        }
        ActualizarContadorCartas();
    }

    private static void PintarBordeSuperior(object? sender, PaintEventArgs e)
    {
        if (sender is not Control ctrl) return;
        using var pen = new Pen(Paleta.Acento, 2f);
        e.Graphics.DrawLine(pen, 0, ctrl.Height - 1, ctrl.Width, ctrl.Height - 1);
    }

    private static void EstilarBoton(Button? b, Color fondo, Color texto, bool borde = false)
    {
        if (b == null) return;
        b.BackColor  = fondo;
        b.ForeColor  = texto;
        b.FlatStyle  = FlatStyle.Flat;
        b.Font       = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        b.Cursor     = Cursors.Hand;
        b.FlatAppearance.BorderSize  = borde ? 1 : 0;
        b.FlatAppearance.BorderColor = borde ? Paleta.Borde : fondo;
        b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(fondo, 0.15f);
        b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(fondo, 0.1f);
    }

    // =========================================================================
    // CONTADOR DE CARTAS
    // =========================================================================

    private void ActualizarContadorCartas()
    {
        if (lblContadorCartas == null) return;
        int restantes = TOTAL_CARTAS - _cartasCantadas;
        lblContadorCartas.Text = $"🃏 Cartas: {_cartasCantadas}/{TOTAL_CARTAS}  ({restantes} restantes)";
        if (restantes <= 5 && restantes > 0)
            lblContadorCartas.ForeColor = Paleta.AcentoAmbar;
        else if (restantes == 0)
            lblContadorCartas.ForeColor = Paleta.Acento;
        else
            lblContadorCartas.ForeColor = Paleta.TextoSecund;
    }

    // =========================================================================
    // PANEL DE FICHAS
    // =========================================================================

    private void ConstruirPanelFichas()
    {
        flpFichas.Controls.Clear();
        if (lblFichaHint != null)
            lblFichaHint.Text = "① Elige ficha  ② Click en casilla";

        foreach (var nombre in FichaService.NombresFichas)
        {
            var img = _fichas.ObtenerFicha(nombre);
            if (img == null) continue;

            var contenedor = new Panel
            {
                Size      = new Size(TAM_FICHA_PANEL + 8, TAM_FICHA_PANEL + 8),
                BackColor = Paleta.SuperficieAlt,
                Cursor    = Cursors.Hand,
                Margin    = new Padding(4),
                Tag       = nombre
            };

            contenedor.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool sel = _fichaSeleccionada == nombre;
                using var pen = new Pen(sel ? Paleta.Seleccionado : Paleta.Borde, sel ? 2.5f : 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, contenedor.Width - 1, contenedor.Height - 1);
                if (sel)
                {
                    using var brushGlow = new SolidBrush(Color.FromArgb(30, 80, 160, 255));
                    e.Graphics.FillRectangle(brushGlow, 1, 1, contenedor.Width - 2, contenedor.Height - 2);
                }
            };

            var pic = new PictureBox
            {
                Size      = new Size(TAM_FICHA_PANEL, TAM_FICHA_PANEL),
                SizeMode  = PictureBoxSizeMode.Zoom,
                Image     = img,
                Cursor    = Cursors.Hand,
                BackColor = Color.Transparent,
                Location  = new Point(4, 4),
                Tag       = nombre
            };

            var tt = new ToolTip();
            tt.SetToolTip(pic, $"Ficha: {nombre}\n(Click para seleccionar)");
            _tooltips.Add(tt);

            EventHandler clickHandler = (_, _) => SeleccionarFicha(nombre, contenedor);
            pic.Click       += clickHandler;
            contenedor.Click += clickHandler;

            pic.MouseEnter       += (_, _) => { if (_fichaSeleccionada != nombre) contenedor.BackColor = Color.FromArgb(55, 55, 80); };
            pic.MouseLeave       += (_, _) => { if (_fichaSeleccionada != nombre) contenedor.BackColor = Paleta.SuperficieAlt; };
            contenedor.MouseEnter += (_, _) => { if (_fichaSeleccionada != nombre) contenedor.BackColor = Color.FromArgb(55, 55, 80); };
            contenedor.MouseLeave += (_, _) => { if (_fichaSeleccionada != nombre) contenedor.BackColor = Paleta.SuperficieAlt; };

            contenedor.Controls.Add(pic);
            flpFichas.Controls.Add(contenedor);
        }

        // Borrador
        var borrador = new Panel
        {
            Size      = new Size(TAM_FICHA_PANEL + 8, TAM_FICHA_PANEL + 8),
            BackColor = Paleta.SuperficieAlt,
            Cursor    = Cursors.Hand,
            Margin    = new Padding(4),
            Tag       = "__borrador__"
        };
        borrador.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bool sel = _fichaSeleccionada == "__borrador__";
            using var pen = new Pen(sel ? Color.FromArgb(255, 120, 50) : Paleta.Borde, sel ? 2.5f : 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, borrador.Width - 1, borrador.Height - 1);
        };
        var lblBorrador = new Label
        {
            Text      = "🗑️\nBorrar",
            Size      = new Size(TAM_FICHA_PANEL, TAM_FICHA_PANEL),
            BackColor = Color.Transparent,
            ForeColor = Paleta.TextoSecund,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor    = Cursors.Hand,
            Location  = new Point(4, 4),
            Font      = new Font("Segoe UI", 9f),
            Tag       = "__borrador__"
        };
        EventHandler borrarClick = (_, _) => SeleccionarFicha("__borrador__", borrador);
        lblBorrador.Click  += borrarClick;
        borrador.Click     += borrarClick;
        borrador.Controls.Add(lblBorrador);
        flpFichas.Controls.Add(borrador);
    }

    private void SeleccionarFicha(string nombre, Panel contenedor)
    {
        if (_panelFichaSeleccionada != null)
        {
            _panelFichaSeleccionada.BackColor = Paleta.SuperficieAlt;
            _panelFichaSeleccionada.Invalidate();
        }

        if (_fichaSeleccionada == nombre)
        {
            _fichaSeleccionada        = null;
            _panelFichaSeleccionada   = null;
            ActualizarHintFicha("① Elige ficha  ② Click en casilla");
        }
        else
        {
            _fichaSeleccionada      = nombre;
            _panelFichaSeleccionada = contenedor;
            contenedor.BackColor    = Color.FromArgb(30, 80, 160);
            contenedor.Invalidate();
            var hint = nombre == "__borrador__"
                ? "🗑️ Click en casilla para borrar fichas"
                : $"Ficha '{nombre}' lista — click en casilla";
            ActualizarHintFicha(hint);
        }
    }

    private void ActualizarHintFicha(string texto)
    {
        if (lblFichaHint != null) lblFichaHint.Text = texto;
    }

    // =========================================================================
    // CLICK EN CELDA
    // =========================================================================

    private void OnCeldaClick(PictureBox celda)
    {
        if (!_juegoIniciado) return;
        int num = (int)celda.Tag!;
        if (_fichaSeleccionada == null) return;

        if (_fichaSeleccionada == "__borrador__")
        {
            if (_fichasEnTabla.ContainsKey(num))
            {
                _fichasEnTabla.Remove(num);
                RefrescarCelda(celda, num);
                _ = _cliente.ToggleCarta(num);
            }
            return;
        }

        if (!_fichasEnTabla.TryGetValue(num, out var lista))
        {
            lista = new List<string>();
            _fichasEnTabla[num] = lista;
        }

        if (lista.Contains(_fichaSeleccionada))
        {
            lista.Remove(_fichaSeleccionada);
            if (lista.Count == 0) _fichasEnTabla.Remove(num);
        }
        else
        {
            lista.Add(_fichaSeleccionada);
        }

        RefrescarCelda(celda, num);
        _ = _cliente.ToggleCarta(num);
    }

    // =========================================================================
    // COMPOSICIÓN CON FICHAS
    // =========================================================================

    private void RefrescarCelda(PictureBox celda, int num)
    {
        var imgCarta = _imagenes.ObtenerImagenCarta(num);
        if (_fichasEnTabla.TryGetValue(num, out var lista) && lista.Count > 0)
        {
            var imgs = lista.Select(n => _fichas.ObtenerFicha(n)).ToList();
            celda.Image = ComponerConFichas(imgCarta, imgs, celda.Width, celda.Height);
        }
        else
        {
            celda.Image = EscalarImagen(imgCarta, celda.Width, celda.Height);
        }
        celda.Invalidate();
    }

    private static Image EscalarImagen(Image? src, int w, int h)
    {
        var bmp = new Bitmap(w, h);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode     = SmoothingMode.HighQuality;
        g.Clear(Color.White);
        if (src != null) g.DrawImage(src, 0, 0, w, h);
        return bmp;
    }

    private static Image ComponerConFichas(Image? carta, List<Image?> fichas, int w, int h)
    {
        var bmp = new Bitmap(w, h);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode     = SmoothingMode.HighQuality;
        g.Clear(Color.White);
        if (carta != null) g.DrawImage(carta, 0, 0, w, h);

        int count = fichas.Count(f => f != null);
        if (count == 0) return bmp;

        int sz = (int)(Math.Min(w, h) * 0.65);
        for (int i = 0; i < fichas.Count; i++)
        {
            var ficha = fichas[i];
            if (ficha == null) continue;
            int offsetX = count > 1 ? (i % 2 == 0 ? -8 : 8) : 0;
            int offsetY = count > 1 ? (i % 2 == 0 ? -6 : 6) : 0;
            int ox = (w - sz) / 2 + offsetX;
            int oy = (h - sz) / 2 + offsetY;
            g.DrawImage(ficha, ox, oy, sz, sz);
        }
        return bmp;
    }

    // =========================================================================
    // TABLA
    // =========================================================================

    private void RenderizarTabla(List<CasillaDto> casillas)
    {
        grilla.Controls.Clear();
        _fichasEnTabla.Clear();
        for (int i = 0; i < casillas.Count; i++)
        {
            var dto = casillas[i];
            grilla.Controls.Add(
                CrearCelda(dto.Numero, dto.Nombre),
                i % Domain.Tabla.Columnas,
                i / Domain.Tabla.Columnas);
        }
    }

    private PictureBox CrearCelda(int numero, string nombre)
    {
        var pic = new PictureBox
        {
            Size        = new Size(CEL_W, CEL_H),
            SizeMode    = PictureBoxSizeMode.Normal,
            BorderStyle = BorderStyle.None,
            BackColor   = Paleta.Fondo,
            Margin      = new Padding(1),
            Dock        = DockStyle.Fill,
            Tag         = numero,
            Image       = EscalarImagen(_imagenes.ObtenerImagenCarta(numero), CEL_W, CEL_H),
            Cursor      = Cursors.Hand
        };
        var tt = new ToolTip();
        tt.SetToolTip(pic, $"{numero} — {nombre}\n(Click para colocar ficha)");
        _tooltips.Add(tt);

        pic.MouseEnter += (_, _) => { if (pic.BackColor == Paleta.Fondo) pic.BackColor = Paleta.SuperficieAlt; };
        pic.MouseLeave += (_, _) => { if (pic.BackColor == Paleta.SuperficieAlt) pic.BackColor = Paleta.Fondo; };
        pic.Click      += (_, _) => OnCeldaClick(pic);

        return pic;
    }

    private void ResaltarCantadaEnTabla(int numero)
    {
        foreach (Control c in grilla.Controls)
            if (c is PictureBox p && (int)p.Tag! == numero)
                p.BackColor = Paleta.AcentoVerde;
    }

    private void RefrescarTodasLasCeldas()
    {
        foreach (Control c in grilla.Controls)
            if (c is PictureBox p) RefrescarCelda(p, (int)p.Tag!);
    }

    // =========================================================================
    // GUARDAR / CARGAR TABLA
    // =========================================================================

    private void GuardarTabla()
    {
        var casillas = new List<int>();
        for (int i = 0; i < grilla.Controls.Count; i++)
            if (grilla.Controls[i] is PictureBox p)
                casillas.Add((int)p.Tag!);

        if (casillas.Count == 0)
        {
            MessageBox.Show("No hay tabla que guardar.", "Aviso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var nombre = $"tabla_{_miNombre}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
        var ruta   = Path.Combine(_carpetaTablas, nombre);

        var datos = new TablaGuardada
        {
            Jugador          = _miNombre,
            FechaGuardado    = DateTime.Now,
            NumerosCasillas  = casillas,
            FichasColocadas  = _fichasEnTabla.ToDictionary(kv => kv.Key, kv => kv.Value.ToList())
        };

        File.WriteAllText(ruta, JsonSerializer.Serialize(datos,
            new JsonSerializerOptions { WriteIndented = true }));

        Mensaje($"✅ Tabla guardada: {nombre}", Paleta.AcentoVerde);
        MessageBox.Show($"Tabla guardada en:\n{ruta}", "Tabla guardada",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CargarTablaGuardada()
    {
        var archivos = Directory.GetFiles(_carpetaTablas, "*.json")
                                .OrderByDescending(f => File.GetLastWriteTime(f))
                                .ToArray();

        if (archivos.Length == 0)
        {
            MessageBox.Show("No hay tablas guardadas todavía.",
                "Sin tablas guardadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var opciones = archivos.Select(f =>
        {
            var info = new FileInfo(f);
            try
            {
                var j   = File.ReadAllText(f);
                var dto = JsonSerializer.Deserialize<TablaGuardada>(j);
                return $"{dto?.Jugador ?? "?"} — {dto?.FechaGuardado:dd/MM/yyyy HH:mm}  ({info.Name})";
            }
            catch { return info.Name; }
        }).ToArray();

        using var dlg = new FormSeleccionarTabla(opciones, archivos);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        var rutaElegida = dlg.RutaSeleccionada;
        if (string.IsNullOrEmpty(rutaElegida)) return;

        try
        {
            var json  = File.ReadAllText(rutaElegida);
            var datos = JsonSerializer.Deserialize<TablaGuardada>(json);
            if (datos == null || datos.NumerosCasillas.Count == 0)
            {
                MessageBox.Show("El archivo no contiene una tabla válida.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            grilla.Controls.Clear();
            _fichasEnTabla.Clear();

            int cols = Domain.Tabla.Columnas;
            for (int i = 0; i < datos.NumerosCasillas.Count; i++)
            {
                int num   = datos.NumerosCasillas[i];
                var celda = CrearCelda(num, $"Carta {num}");
                grilla.Controls.Add(celda, i % cols, i / cols);
            }

            foreach (var kv in datos.FichasColocadas)
                _fichasEnTabla[kv.Key] = new List<string>(kv.Value);

            RefrescarTodasLasCeldas();
            Mensaje($"✅ Tabla de {datos.Jugador} cargada ({datos.FechaGuardado:dd/MM/yy HH:mm})", Paleta.AcentoVerde);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"No se pudo cargar la tabla:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private class TablaGuardada
    {
        public string Jugador { get; set; } = "";
        public DateTime FechaGuardado { get; set; }
        public List<int> NumerosCasillas { get; set; } = new();
        public Dictionary<int, List<string>> FichasColocadas { get; set; } = new();
    }

    // =========================================================================
    // SIGNALR
    // =========================================================================

    private async Task UnirseAsync()
    {
        try
        {
            await _cliente.ConectarAsync();
            await _cliente.UnirseAlJuego(_miNombre);
        }
        catch (Exception ex)
        {
            Mensaje($"Error al conectar: {ex.Message}", Paleta.Acento);
        }
    }

    private void SuscribirSignalR()
    {
        _cliente.RolAsignado += (esHost, _) =>
            UI(() => Mensaje($"Conectado como {(esHost ? "Gritón" : "Jugador")}", Paleta.TextoSecund));

        _cliente.TablaAsignada += casillas =>
            UI(() =>
            {
                RenderizarTabla(casillas);
                if (!_juegoIniciado) btnNuevaTabla.Enabled = true;
            });

        _cliente.FormatoActual += fmt => UI(() => { /* todos los formatos siempre activos */ });

        _cliente.JuegoIniciado += fmt => UI(() =>
        {
            _juegoIniciado = true;
            _cartasCantadas = 0;
            _ganadoresRonda.Clear();
            ActualizarContadorCartas();
            btnLoteria.Enabled            = true;
            btnNuevaTabla.Enabled         = false;
            btnGuardarTabla.Enabled       = true;
            btnCargarTabla.Enabled        = false;
            btnReiniciarPartida.Enabled   = _esHost;
            if (_esHost)
            {
                btnCantarCarta.Enabled = true;
                btnIniciar.Enabled     = false;
            }
            Mensaje("¡Juego iniciado! Ganás con cualquier patrón.", Paleta.AcentoVerde);
        });

        _cliente.JuegoYaIniciado += (fmt, casillas) => UI(() =>
        {
            _juegoIniciado = true;
            btnLoteria.Enabled          = true;
            btnNuevaTabla.Enabled       = true;
            btnGuardarTabla.Enabled     = true;
            btnCargarTabla.Enabled      = false;
            btnReiniciarPartida.Enabled = _esHost;
            RenderizarTabla(casillas);
            Mensaje("Te uniste a una partida en curso.", Paleta.TextoSecund);
        });

        _cliente.CartaCantada += (num, nombre, frase) => UI(() =>
        {
            lblNombreCarta.Text = $"{num} — {nombre}";
            lblFrase.Text       = $"\"{frase}\"";
            picCarta.Image      = _imagenes.ObtenerImagenCarta(num);
            AgregarHistorial(num, nombre);
            ResaltarCantadaEnTabla(num);
            _tts.CantarCarta(frase, nombre);

            _cartasCantadas++;
            ActualizarContadorCartas();

            if (_cartasCantadas >= TOTAL_CARTAS)
            {
                if (_esHost) btnCantarCarta.Enabled = false;
                Mensaje("🚫 ¡Se acabaron las cartas!", Paleta.Acento);
            }
        });

        _cliente.BarajaRebrajada += () => UI(() =>
        {
            _cartasCantadas = 0;
            ActualizarContadorCartas();
            Mensaje("🔀 Baraja rebrajada.", Paleta.AcentoAmbar);
        });

        _cliente.BarajaReiniciada += () => UI(() =>
        {
            _cartasCantadas = 0;
            ActualizarContadorCartas();
            LimpiarHistorial();
            if (_esHost) btnIniciar.Enabled = true;
            btnReiniciarPartida.Enabled = false;
            Mensaje("Baraja reiniciada. Lista para nueva partida.", Paleta.TextoSecund);
        });

        _cliente.MarcasActualizadas += marcas => UI(() =>
        {
            var set = marcas.ToHashSet();
            foreach (var k in _fichasEnTabla.Keys.Where(k => !set.Contains(k)).ToList())
                _fichasEnTabla.Remove(k);
            RefrescarTodasLasCeldas();
        });

        // ── HayGanador: ahora recibe nombre Y patrón ganador ─────────────────
        // Si el servidor manda solo el nombre, se usa la sobrecarga de abajo.
        // Preferir la versión con patrón.
        _cliente.HayGanador += nombre => UI(() =>
            ProcesarGanador(nombre, ""));

        // Si tu ClienteSignalR expone una versión con patrón, suscribite así:
        // _cliente.HayGanadorConPatron += (nombre, patron) => UI(() =>
        //     ProcesarGanador(nombre, patron));

        _cliente.Trampa += trampas => UI(() =>
        {
            Mensaje("⚠️ ¡Trampa! Tienes fichas en cartas que no han salido.", Paleta.Acento);
            foreach (Control c in grilla.Controls)
                if (c is PictureBox p && trampas.Contains((int)p.Tag!))
                    p.BackColor = Color.FromArgb(100, 255, 75, 75);
        });

        _cliente.FalsaAlarma += () =>
            UI(() => Mensaje("Aún no cumples el patrón ganador.", Paleta.AcentoAmbar));

        _cliente.JugadoresActualizados += lista => UI(() =>
        {
            lstJugadores.Items.Clear();
            foreach (var dto in lista)
                lstJugadores.Items.Add(new JugadorItem(dto.Nombre, dto.EsGriton, dto.Victorias));
            lblConectados.Text = $"Conectados: {lista.Count}";
        });

        _cliente.Desconectado   += msg      => UI(() => Mensaje($"Desconectado: {msg}", Paleta.Acento));
        _cliente.MensajeRecibido += (n, t)  => UI(() => AgregarMensajeChat(n, t));
    }

    // =========================================================================
    // PROCESAR GANADOR — soporta múltiples ganadores simultáneos
    // =========================================================================

    /// <summary>
    /// Registra al ganador. Si varios llegan en la misma ronda (distintos
    /// patrones), se muestran todos juntos en el diálogo final.
    /// </summary>
    private void ProcesarGanador(string nombre, string patron)
    {
        // Evitar duplicados del mismo jugador en la misma ronda
        if (_ganadoresRonda.Any(g => g.Nombre == nombre)) return;

        _ganadoresRonda.Add((nombre, patron));

        // Dar 300 ms de margen para que lleguen más ganadores simultáneos
        Task.Delay(300).ContinueWith(_ => UI(MostrarGanadores));
    }

    private void MostrarGanadores()
    {
        if (_ganadoresRonda.Count == 0) return;

        _juegoIniciado   = false;
        _cartasCantadas  = 0;
        ActualizarContadorCartas();
        btnLoteria.Enabled          = false;
        btnNuevaTabla.Enabled       = true;
        btnCargarTabla.Enabled      = true;
        btnReiniciarPartida.Enabled = _esHost;
        if (_esHost)
        {
            btnCantarCarta.Enabled = false;
            btnIniciar.Enabled     = true;
        }

        bool yoGane = _ganadoresRonda.Any(g => g.Nombre == _miNombre);

        // Construir mensaje detallado con cada ganador y su patrón
        var sb = new System.Text.StringBuilder();

        if (_ganadoresRonda.Count == 1)
        {
            var (gNombre, gPatron) = _ganadoresRonda[0];
            if (gNombre == _miNombre)
            {
                sb.AppendLine("🏆  ¡¡GANASTE!!");
                sb.AppendLine();
                sb.AppendLine($"Jugador:  {gNombre}");
                if (!string.IsNullOrEmpty(gPatron))
                    sb.AppendLine($"Patrón:   {gPatron}");
            }
            else
            {
                sb.AppendLine($"🏅  ¡LOTERÍA para {gNombre}!");
                sb.AppendLine();
                sb.AppendLine($"Jugador:  {gNombre}");
                if (!string.IsNullOrEmpty(gPatron))
                    sb.AppendLine($"Patrón:   {gPatron}");
            }
        }
        else
        {
            // Varios ganadores simultáneos
            sb.AppendLine($"🎉  ¡{_ganadoresRonda.Count} jugadores ganaron al mismo tiempo!");
            sb.AppendLine();
            foreach (var (gNombre, gPatron) in _ganadoresRonda)
            {
                var tuMarca = gNombre == _miNombre ? "  ← ¡TÚ!" : "";
                var patronStr = string.IsNullOrEmpty(gPatron) ? "" : $"  ({gPatron})";
                sb.AppendLine($"  🏆  {gNombre}{patronStr}{tuMarca}");
            }
        }

        var titulo = yoGane ? "🏆 ¡LOTERÍA!" : "🏅 Fin de ronda";
        var color  = yoGane ? Paleta.Ganador : Paleta.Acento;
        var msgCorto = yoGane
            ? $"🏆 ¡GANASTE! — {string.Join(", ", _ganadoresRonda.Select(g => g.Nombre))}"
            : $"🏅 Ganó: {string.Join(", ", _ganadoresRonda.Select(g => g.Nombre))}";

        Mensaje(msgCorto, color);

        // Resaltar fila de ganadores en la lista de jugadores
        lstJugadores.Invalidate();

        MessageBox.Show(sb.ToString(), titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Limpiar lista tras mostrar el diálogo
        _ganadoresRonda.Clear();
    }

    // =========================================================================
    // EVENTOS UI
    // =========================================================================

    private void BindearUI()
    {
        if (_esHost)
        {
            btnIniciar.Click += async (_, _) =>
            {
                try
                {
                    btnIniciar.Enabled = false;
                    await _cliente.IniciarJuego(FormatoGanador.TablaLlena.ToString());
                }
                catch (Exception ex)
                {
                    btnIniciar.Enabled = true;
                    Mensaje($"No se pudo iniciar: {ex.Message}", Paleta.Acento);
                    MessageBox.Show($"Error:\n\n{ex.Message}", "Error al iniciar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnCantarCarta.Click += async (_, _) =>
            {
                if (_cartasCantadas >= TOTAL_CARTAS)
                {
                    MessageBox.Show("¡Ya se cantaron todas las cartas!\nReiniciá la partida.",
                        "Baraja agotada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnCantarCarta.Enabled = false;
                    return;
                }
                try { await _cliente.CantarCarta(); }
                catch (Exception ex) { Mensaje($"Error: {ex.Message}", Paleta.Acento); }
            };

            // ── Botón Reiniciar Partida (solo host) ───────────────────────────
            btnReiniciarPartida.Click += async (_, _) =>
            {
                var resp = MessageBox.Show(
                    "¿Reiniciar la partida?\n\nSe barajará de nuevo y todos recibirán nuevas tablas.",
                    "Reiniciar partida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resp != DialogResult.Yes) return;

                try
                {
                    btnReiniciarPartida.Enabled = false;
                    // Llama al método de reinicio en el servidor (ajusta el nombre si es diferente)
                    await _cliente.ReiniciarBaraja(); 
                }
                catch (Exception ex)
                {
                    btnReiniciarPartida.Enabled = true;
                    Mensaje($"Error al reiniciar: {ex.Message}", Paleta.Acento);
                }
            };
        }

        btnLoteria.Click += async (_, _) =>
        {
            try { await _cliente.ReclamarLoteria(); }
            catch (Exception ex) { Mensaje($"Error: {ex.Message}", Paleta.Acento); }
        };

        btnNuevaTabla.Click += async (_, _) =>
        {
            try
            {
                btnNuevaTabla.Enabled = false;
                await _cliente.PedirNuevaTabla();
            }
            catch (Exception ex)
            {
                btnNuevaTabla.Enabled = true;
                Mensaje($"Error: {ex.Message}", Paleta.Acento);
            }
        };

        btnGuardarTabla.Click += (_, _) => GuardarTabla();
        btnCargarTabla.Click  += (_, _) => CargarTablaGuardada();

        chkTts.CheckedChanged += (_, _) => _tts.Habilitado = chkTts.Checked;

        if (btnEnviar != null)
            btnEnviar.Click += async (_, _) => await EnviarMensajeChat();

        if (txtMensaje != null)
            txtMensaje.KeyDown += async (_, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await EnviarMensajeChat(); }
            };

        FormClosed += async (_, _) =>
        {
            foreach (var tt in _tooltips) tt.Dispose();
            _tooltips.Clear();
            _tts.Dispose();
            _imagenes.Dispose();
            _fichas.Dispose();
            await _cliente.DisposeAsync();
            if (_servidor != null) await _servidor.DisposeAsync();
        };
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private void AgregarHistorial(int numero, string nombre)
    {
        var contenedor = new Panel
        {
            Size      = new Size(54, 54),
            BackColor = Paleta.SuperficieAlt,
            Margin    = new Padding(3),
            Cursor    = Cursors.Default
        };
        contenedor.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Paleta.Acento, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, contenedor.Width - 1, contenedor.Height - 1);
        };

        var pic = new PictureBox
        {
            Size      = new Size(50, 50),
            SizeMode  = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Location  = new Point(2, 2),
            Image     = _imagenes.ObtenerImagenCarta(numero)
        };

        var tt = new ToolTip();
        tt.SetToolTip(pic, $"{numero} — {nombre}");
        _tooltips.Add(tt);

        contenedor.Controls.Add(pic);
        flpHistorial.Controls.Add(contenedor);
        flpHistorial.ScrollControlIntoView(contenedor);
    }

    private void LimpiarHistorial()
    {
        foreach (Control ctrl in flpHistorial.Controls)
            ctrl.Dispose();
        flpHistorial.Controls.Clear();
        _fichasEnTabla.Clear();
        RefrescarTodasLasCeldas();
        lblNombreCarta.Text = string.Empty;
        lblFrase.Text       = string.Empty;
        picCarta.Image      = null;
    }

    private void Mensaje(string txt, Color c)
    { lblMensaje.Text = txt; lblMensaje.ForeColor = c; }

    private void UI(Action a)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired) BeginInvoke(a);
        else a();
    }

    private record JugadorItem(string Nombre, bool EsGriton, int Victorias)
    { public override string ToString() => Nombre; }

    private async Task EnviarMensajeChat()
    {
        var texto = txtMensaje?.Text.Trim();
        if (string.IsNullOrEmpty(texto)) return;
        txtMensaje!.Clear();
        try { await _cliente.EnviarMensaje(texto); }
        catch { }
    }

    private void AgregarMensajeChat(string nombre, string texto)
    {
        bool esYo = nombre == _miNombre;
        var hora  = DateTime.Now.ToString("HH:mm");

        rtbChat.SelectionStart  = rtbChat.TextLength;
        rtbChat.SelectionLength = 0;

        using var fNombre = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var fHora   = new Font("Segoe UI", 7.5f);
        using var fTexto  = new Font("Segoe UI", 9.5f);

        rtbChat.SelectionColor = esYo ? Paleta.AcentoVerde : Paleta.Acento;
        rtbChat.SelectionFont  = fNombre;
        rtbChat.AppendText($"{nombre}  ");

        rtbChat.SelectionColor = Paleta.TextoSecund;
        rtbChat.SelectionFont  = fHora;
        rtbChat.AppendText($"{hora}\n");

        rtbChat.SelectionColor = Paleta.TextoPrimario;
        rtbChat.SelectionFont  = fTexto;
        rtbChat.AppendText($"{texto}\n\n");

        rtbChat.ScrollToCaret();
    }

    private void lstJugadores_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= lstJugadores.Items.Count) return;
        if (lstJugadores.Items[e.Index] is not JugadorItem item) return;

        var bg = e.Index % 2 == 0 ? Paleta.Superficie : Paleta.SuperficieAlt;
        using var brushBg = new SolidBrush(bg);
        e.Graphics.FillRectangle(brushBg, e.Bounds);

        if (item.EsGriton)
        {
            using var brushAccent = new SolidBrush(Paleta.Acento);
            e.Graphics.FillRectangle(brushAccent,
                new Rectangle(e.Bounds.X, e.Bounds.Y, 3, e.Bounds.Height));
        }

        var icono   = item.EsGriton ? "🎤" : "👤";
        var texto   = $"{icono} {item.Nombre}";
        var victorias = item.Victorias > 0 ? $"🏆 {item.Victorias}" : "";

        using var fNombre    = new Font("Segoe UI", 9f, item.EsGriton ? FontStyle.Bold : FontStyle.Regular);
        using var fVictorias = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var brushNombre = new SolidBrush(item.EsGriton ? Paleta.Acento : Paleta.TextoPrimario);
        using var brushV      = new SolidBrush(Paleta.AcentoAmbar);

        var rect = e.Bounds;
        e.Graphics.DrawString(texto, fNombre, brushNombre,
            new RectangleF(rect.X + 10, rect.Y + 4, rect.Width - 50, rect.Height - 4));

        if (victorias.Length > 0)
            e.Graphics.DrawString(victorias, fVictorias, brushV,
                new RectangleF(rect.Right - 46, rect.Y + 6, 44, rect.Height - 4));

        using var penSep = new Pen(Paleta.Borde, 0.5f);
        e.Graphics.DrawLine(penSep, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
        e.DrawFocusRectangle();
    }

    private void lblFrase_Click(object sender, EventArgs e) { }
    private void flpFichas_Paint(object sender, PaintEventArgs e) { }
    private void FormJuegoRed_Load(object sender, EventArgs e) { }
    private void pnlTopBar_Paint(object sender, PaintEventArgs e) { }
    private void lblChatTitulo_Click(object sender, EventArgs e) { }
    private void btnGuardarTabla_Click(object sender, EventArgs e) { }
}