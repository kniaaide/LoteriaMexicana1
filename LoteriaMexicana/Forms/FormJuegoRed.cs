using LoteriaMexicana.Domain;
using LoteriaMexicana.Services;
using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Hubs;
using System.Drawing.Drawing2D;

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
    private readonly Dictionary<int, string> _fichasEnTabla = new();
    private bool _juegoIniciado = false;

    // Drag & drop
    private string? _fichaArrastrada = null;
    private PictureBox? _ghostPic = null;

    private const int CEL_W = 110;
    private const int CEL_H = 128;
    private const int TAM_FICHA_PANEL = 70;
    private const int TAM_FICHA_TABLA = 80;

    private readonly List<ToolTip> _tooltips = new();

    private static readonly (string Etiqueta, FormatoGanador Valor)[] Formatos =
    {
        ("Línea Horizontal", FormatoGanador.LineaHorizontal),
        ("Línea Vertical",   FormatoGanador.LineaVertical),
        ("Diagonal",         FormatoGanador.Diagonal),
        ("Cruz",             FormatoGanador.Cruz),
        ("Cruzita",          FormatoGanador.Cruzita),
        ("Tabla Llena",      FormatoGanador.TablaLlena),
    };

    // ── Paleta visual ─────────────────────────────────────────────────────────
    private static class Paleta
    {
        public static readonly Color Fondo = Color.FromArgb(18, 18, 24);
        public static readonly Color Superficie = Color.FromArgb(28, 28, 38);
        public static readonly Color SuperficieAlt = Color.FromArgb(38, 38, 52);
        public static readonly Color Acento = Color.FromArgb(255, 75, 75);
        public static readonly Color AcentoVerde = Color.FromArgb(0, 210, 130);
        public static readonly Color AcentoAmbar = Color.FromArgb(255, 190, 50);
        public static readonly Color TextoPrimario = Color.FromArgb(240, 240, 248);
        public static readonly Color TextoSecund = Color.FromArgb(150, 150, 170);
        public static readonly Color Borde = Color.FromArgb(55, 55, 75);
    }

    public FormJuegoRed(ClienteSignalR cliente, ServidorSignalR? servidor,
                        string nombre, bool esHost)
    {
        _cliente = cliente;
        _servidor = servidor;
        _miNombre = nombre;
        _esHost = esHost;

        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        _tts = new TtsService();
        _imagenes = new ImagenService(Path.Combine(baseDir, "Resources", "Cartas"));
        _fichas = new FichaService(Path.Combine(baseDir, "Resources", "Fichas"));
        _fichas.Precargar();

        InitializeComponent();
        this.WindowState = FormWindowState.Maximized;

        Text = $"Lotería Mexicana — {nombre}{(esHost ? " (Gritón)" : "")}";
        lblUsuario.Text = $"{nombre}{(esHost ? "  🎤 Gritón" : "")}";

        if (esHost)
        {
            lblSalaInfo.Text = $"📡  {ServidorSignalR.ObtenerIpLocal()}:{ServidorSignalR.Puerto}";
            foreach (var (e, v) in Formatos)
                cmbFormato.Items.Add(new FormatoItem(e, v));
            cmbFormato.SelectedIndex = 0;
        }
        else
        {
            pnlConfigHost.Visible = false;
            btnCantarCarta.Visible = false;
        }

        ConstruirPanelFichas();
        SuscribirSignalR();
        BindearUI();
        AplicarTema();

        Shown += async (_, _) => await UnirseAsync();
    }

    // =========================================================================
    // TEMA VISUAL
    // =========================================================================

    private void AplicarTema()
    {
        BackColor = Paleta.Fondo;
        ForeColor = Paleta.TextoPrimario;

        // Barra superior
        pnlTopBar.BackColor = Paleta.Superficie;
        pnlTopBar.Paint += PintarBordeSuperior;

        lblUsuario.ForeColor = Paleta.TextoPrimario;
        lblUsuario.Font = new Font("Segoe UI", 11f, FontStyle.Bold);

        lblSalaInfo.ForeColor = Paleta.AcentoVerde;
        lblSalaInfo.Font = new Font("Segoe UI", 9f);

        lblMensaje.ForeColor = Paleta.TextoSecund;
        lblMensaje.Font = new Font("Segoe UI", 9f, FontStyle.Italic);

        // Panel config host
        pnlConfigHost.BackColor = Paleta.Superficie;
        EstilarBoton(btnIniciar, Paleta.AcentoVerde, Color.FromArgb(10, 30, 20));
        EstilarBoton(btnCantarCarta, Paleta.Acento, Color.White);
        EstilarBoton(btnLoteria, Paleta.AcentoAmbar, Color.FromArgb(30, 20, 0));
        EstilarBoton(btnNuevaTabla, Paleta.SuperficieAlt, Paleta.TextoPrimario, borde: true);
        EstilarCombo(cmbFormato);

        // Carta cantada
        picCarta.BackColor = Paleta.Superficie;
        picCarta.BorderStyle = BorderStyle.None;
        lblNombreCarta.ForeColor = Paleta.TextoPrimario;
        lblNombreCarta.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
        lblFrase.ForeColor = Paleta.AcentoAmbar;
        lblFrase.Font = new Font("Segoe UI", 9.5f, FontStyle.Italic);

        // Historial
        flpHistorial.BackColor = Paleta.Superficie;

        // Fichas
        flpFichas.BackColor = Paleta.Superficie;

        // Grilla
        grilla.BackColor = Paleta.Fondo;
        grilla.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

        // Jugadores
        lblConectados.ForeColor = Paleta.TextoSecund;
        lblConectados.Font = new Font("Segoe UI", 8.5f);
        lstJugadores.BackColor = Paleta.Superficie;
        lstJugadores.ForeColor = Paleta.TextoPrimario;
        lstJugadores.BorderStyle = BorderStyle.None;
        lstJugadores.Font = new Font("Segoe UI", 9.5f);

        // Chat
        rtbChat.BackColor = Paleta.Superficie;
        rtbChat.ForeColor = Paleta.TextoPrimario;
        rtbChat.BorderStyle = BorderStyle.None;
        rtbChat.Font = new Font("Segoe UI", 9.5f);

        txtMensaje.BackColor = Paleta.SuperficieAlt;
        txtMensaje.ForeColor = Paleta.TextoPrimario;
        txtMensaje.BorderStyle = BorderStyle.FixedSingle;
        txtMensaje.Font = new Font("Segoe UI", 10f);

        EstilarBoton(btnEnviar, Paleta.Acento, Color.White);

        chkTts.ForeColor = Paleta.TextoSecund;
        chkTts.BackColor = Color.Transparent;
        chkTts.Font = new Font("Segoe UI", 8.5f);
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
        b.BackColor = fondo;
        b.ForeColor = texto;
        b.FlatStyle = FlatStyle.Flat;
        b.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        b.Cursor = Cursors.Hand;
        b.FlatAppearance.BorderSize = borde ? 1 : 0;
        b.FlatAppearance.BorderColor = borde ? Paleta.Borde : fondo;
        b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(fondo, 0.15f);
        b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(fondo, 0.1f);
    }

    private static void EstilarCombo(ComboBox? c)
    {
        if (c == null) return;
        c.BackColor = Paleta.SuperficieAlt;
        c.ForeColor = Paleta.TextoPrimario;
        c.FlatStyle = FlatStyle.Flat;
        c.Font = new Font("Segoe UI", 9.5f);
    }

    // =========================================================================
    // PANEL DE FICHAS
    // =========================================================================

    private void ConstruirPanelFichas()
    {
        flpFichas.Controls.Clear();
        foreach (var nombre in FichaService.NombresFichas)
        {
            var img = _fichas.ObtenerFicha(nombre);
            if (img == null) continue;

            var contenedor = new Panel
            {
                Size = new Size(TAM_FICHA_PANEL + 8, TAM_FICHA_PANEL + 8),
                BackColor = Paleta.SuperficieAlt,
                Cursor = Cursors.Hand,
                Margin = new Padding(4),
                Tag = nombre
            };
            contenedor.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pen = new Pen(Paleta.Borde, 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, contenedor.Width - 1, contenedor.Height - 1);
            };

            var pic = new PictureBox
            {
                Size = new Size(TAM_FICHA_PANEL, TAM_FICHA_PANEL),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = img,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Location = new Point(4, 4),
                Tag = nombre
            };

            var tt = new ToolTip();
            tt.SetToolTip(pic, nombre);
            _tooltips.Add(tt);

            pic.MouseDown += OnFichaMouseDown;
            contenedor.MouseDown += (s, e) => OnFichaMouseDown(pic, e);

            pic.MouseEnter += (_, _) => contenedor.BackColor = Color.FromArgb(55, 55, 80);
            pic.MouseLeave += (_, _) => contenedor.BackColor = Paleta.SuperficieAlt;
            contenedor.MouseEnter += (_, _) => contenedor.BackColor = Color.FromArgb(55, 55, 80);
            contenedor.MouseLeave += (_, _) => contenedor.BackColor = Paleta.SuperficieAlt;

            contenedor.Controls.Add(pic);
            flpFichas.Controls.Add(contenedor);
        }
    }

    // =========================================================================
    // DRAG & DROP
    // =========================================================================

    private void OnFichaMouseDown(object? sender, MouseEventArgs e)
    {
        if (sender is not PictureBox src) return;
        _fichaArrastrada = src.Tag as string;
        IniciarGhost(src.Image, Cursor.Position);
    }

    private void IniciarGhost(Image imagen, Point posInicial)
    {
        EliminarGhost();
        _ghostPic = new PictureBox
        {
            Size = new Size(60, 60),
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = imagen,
            BackColor = Color.Transparent,
            Enabled = false
        };
        _ghostPic.Location = PointToClient(posInicial);
        Controls.Add(_ghostPic);
        _ghostPic.BringToFront();

        MouseMove += OnFormMouseMove;
        MouseUp += OnFormMouseUp;
        grilla.MouseMove += OnFormMouseMove;
        grilla.MouseUp += OnFormMouseUp;
        foreach (Control c in grilla.Controls)
        { c.MouseMove += OnFormMouseMove; c.MouseUp += OnFormMouseUp; }
    }

    private void OnFormMouseMove(object? sender, MouseEventArgs e)
    {
        if (_ghostPic == null) return;
        var pos = PointToClient(Cursor.Position);
        _ghostPic.Location = new Point(pos.X - 30, pos.Y - 30);
    }

    private void OnFormMouseUp(object? sender, MouseEventArgs e)
    {
        if (_fichaArrastrada == null) { EliminarGhost(); return; }

        var fichaGuardada = _fichaArrastrada;
        _fichaArrastrada = null;
        EliminarGhost();

        MouseMove -= OnFormMouseMove;
        MouseUp -= OnFormMouseUp;
        grilla.MouseMove -= OnFormMouseMove;
        grilla.MouseUp -= OnFormMouseUp;
        foreach (Control c in grilla.Controls)
        { c.MouseMove -= OnFormMouseMove; c.MouseUp -= OnFormMouseUp; }

        var celda = DetectarCeldaEnCursor(PointToClient(Cursor.Position));
        if (celda != null) ColocarFichaEnCelda(celda, fichaGuardada);
    }

    private void EliminarGhost()
    {
        if (_ghostPic == null) return;
        Controls.Remove(_ghostPic);
        _ghostPic.Dispose();
        _ghostPic = null;
    }

    private PictureBox? DetectarCeldaEnCursor(Point cursorEnForm)
    {
        foreach (Control ctrl in grilla.Controls)
        {
            if (ctrl is not PictureBox pic) continue;
            var screenPt = grilla.PointToScreen(pic.Location);
            var rect = new Rectangle(PointToClient(screenPt), pic.Size);
            if (rect.Contains(cursorEnForm)) return pic;
        }
        return null;
    }

    // =========================================================================
    // FICHAS EN CELDA
    // =========================================================================

    private void ColocarFichaEnCelda(PictureBox celda, string nombreFicha)
    {
        if (!_juegoIniciado) return;
        int num = (int)celda.Tag!;

        if (_fichasEnTabla.TryGetValue(num, out var actual) && actual == nombreFicha)
        {
            _fichasEnTabla.Remove(num);
            RefrescarCelda(celda, num);
            _ = _cliente.ToggleCarta(num);
            return;
        }

        _fichasEnTabla[num] = nombreFicha;
        RefrescarCelda(celda, num);
        _ = _cliente.ToggleCarta(num);
    }

    private void RefrescarCelda(PictureBox celda, int num)
    {
        var imgCarta = _imagenes.ObtenerImagenCarta(num);
        celda.Image = _fichasEnTabla.TryGetValue(num, out var f)
            ? ComponerImagenConFicha(imgCarta, _fichas.ObtenerFicha(f), celda.Width, celda.Height)
            : EscalarImagen(imgCarta, celda.Width, celda.Height);
        celda.Invalidate();
    }

    private static Image EscalarImagen(Image? src, int w, int h)
    {
        var bmp = new Bitmap(w, h);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.Clear(Color.White);
        if (src != null) g.DrawImage(src, 0, 0, w, h);
        return bmp;
    }

    private static Image ComponerImagenConFicha(Image? carta, Image? ficha, int w, int h)
    {
        var bmp = new Bitmap(w, h);
        using var g = Graphics.FromImage(bmp);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.Clear(Color.White);
        if (carta != null) g.DrawImage(carta, 0, 0, w, h);
        if (ficha != null)
        {
            int sz = (int)(Math.Min(w, h) * 0.70);
            int ox = (w - sz) / 2;
            int oy = (h - sz) / 2;
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
            Size = new Size(CEL_W, CEL_H),
            SizeMode = PictureBoxSizeMode.Normal,
            BorderStyle = BorderStyle.None,
            BackColor = Paleta.Fondo,
            Margin = new Padding(1),
            Dock = DockStyle.Fill,
            Tag = numero,
            Image = EscalarImagen(_imagenes.ObtenerImagenCarta(numero), CEL_W, CEL_H)
        };
        var tt = new ToolTip();
        tt.SetToolTip(pic, $"{numero} — {nombre}");
        _tooltips.Add(tt);

        pic.MouseEnter += (_, _) => { if (pic.BackColor == Paleta.Fondo) pic.BackColor = Paleta.SuperficieAlt; };
        pic.MouseLeave += (_, _) => { if (pic.BackColor == Paleta.SuperficieAlt) pic.BackColor = Paleta.Fondo; };

        pic.MouseUp += OnFormMouseUp;
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

        _cliente.JuegoIniciado += fmt => UI(() =>
        {
            _juegoIniciado = true;
            btnLoteria.Enabled = true;
            btnNuevaTabla.Enabled = false;
            if (_esHost)
            {
                btnCantarCarta.Enabled = true;
                cmbFormato.Enabled = false;
                btnIniciar.Enabled = false;
            }
            Mensaje($"¡Juego iniciado! Formato: {fmt}", Paleta.AcentoVerde);
        });

        _cliente.JuegoYaIniciado += (fmt, casillas) => UI(() =>
        {
            _juegoIniciado = true;
            btnLoteria.Enabled = true;
            btnNuevaTabla.Enabled = true;
            RenderizarTabla(casillas);
            Mensaje($"Te uniste a una partida en curso. Formato: {fmt}", Paleta.TextoSecund);
        });

        _cliente.CartaCantada += (num, nombre, frase) => UI(() =>
        {
            lblNombreCarta.Text = $"{num} — {nombre}";
            lblFrase.Text = $"\"{frase}\"";
            picCarta.Image = _imagenes.ObtenerImagenCarta(num);
            AgregarHistorial(num, nombre);
            ResaltarCantadaEnTabla(num);
            _tts.CantarCarta(frase, nombre);
        });

        _cliente.BarajaRebrajada += () => UI(() =>
        {
            Mensaje("🔀 La baraja se agotó — ¡se volvió a barajar! La partida sigue.", Paleta.AcentoAmbar);
        });

        _cliente.BarajaReiniciada += () => UI(() =>
        {
            LimpiarHistorial();
            Mensaje("Baraja reiniciada. Lista para nueva partida.", Paleta.TextoSecund);
        });

        _cliente.MarcasActualizadas += marcas => UI(() =>
        {
            var set = marcas.ToHashSet();
            foreach (var k in _fichasEnTabla.Keys.Where(k => !set.Contains(k)).ToList())
                _fichasEnTabla.Remove(k);
            RefrescarTodasLasCeldas();
        });

        _cliente.HayGanador += nombre => UI(() =>
        {
            _juegoIniciado = false;
            btnLoteria.Enabled = false;
            btnNuevaTabla.Enabled = true;
            if (_esHost)
            {
                btnCantarCarta.Enabled = false;
                cmbFormato.Enabled = true;
                btnIniciar.Enabled = true;
            }
            var msg = nombre == _miNombre ? "🏆 ¡GANASTE! ¡LOTERÍA!" : $"🏅 {nombre} ganó la partida.";
            Mensaje(msg, Paleta.Acento);
            MessageBox.Show(msg, "¡LOTERÍA!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        });

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

        _cliente.Desconectado += msg => UI(() => Mensaje($"Desconectado: {msg}", Paleta.Acento));
        _cliente.MensajeRecibido += (n, t) => UI(() => AgregarMensajeChat(n, t));
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
                    var fmt = (FormatoItem)cmbFormato.SelectedItem!;
                    btnIniciar.Enabled = false;
                    await _cliente.IniciarJuego(fmt.Valor.ToString());
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
                try { await _cliente.CantarCarta(); }
                catch (Exception ex) { Mensaje($"Error: {ex.Message}", Paleta.Acento); }
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
            Size = new Size(54, 54),
            BackColor = Paleta.SuperficieAlt,
            Margin = new Padding(3),
            Cursor = Cursors.Default
        };
        contenedor.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Paleta.Acento, 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, contenedor.Width - 1, contenedor.Height - 1);
        };

        var pic = new PictureBox
        {
            Size = new Size(50, 50),
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent,
            Location = new Point(2, 2),
            Image = _imagenes.ObtenerImagenCarta(numero)
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
        lblFrase.Text = string.Empty;
        picCarta.Image = null;
    }

    private void Mensaje(string txt, Color c)
    { lblMensaje.Text = txt; lblMensaje.ForeColor = c; }

    private void UI(Action a)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired) BeginInvoke(a);
        else a();
    }

    private record FormatoItem(string Etiqueta, FormatoGanador Valor)
    { public override string ToString() => Etiqueta; }

    private record JugadorItem(string Nombre, bool EsGriton, int Victorias)
    { public override string ToString() => Nombre; }

    private async Task EnviarMensajeChat()
    {
        var texto = txtMensaje?.Text.Trim();
        if (string.IsNullOrEmpty(texto)) return;
        txtMensaje!.Clear();
        try { await _cliente.EnviarMensaje(texto); }
        catch { /* chat no crítico */ }
    }

    private void AgregarMensajeChat(string nombre, string texto)
    {
        bool esYo = nombre == _miNombre;
        var hora = DateTime.Now.ToString("HH:mm");

        rtbChat.SelectionStart = rtbChat.TextLength;
        rtbChat.SelectionLength = 0;

        using var fNombre = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        using var fHora = new Font("Segoe UI", 7.5f);
        using var fTexto = new Font("Segoe UI", 9.5f);

        rtbChat.SelectionColor = esYo ? Paleta.AcentoVerde : Paleta.Acento;
        rtbChat.SelectionFont = fNombre;
        rtbChat.AppendText($"{nombre}  ");

        rtbChat.SelectionColor = Paleta.TextoSecund;
        rtbChat.SelectionFont = fHora;
        rtbChat.AppendText($"{hora}\n");

        rtbChat.SelectionColor = Paleta.TextoPrimario;
        rtbChat.SelectionFont = fTexto;
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

        // Indicador lateral de acento si es Gritón
        if (item.EsGriton)
        {
            using var brushAccent = new SolidBrush(Paleta.Acento);
            e.Graphics.FillRectangle(brushAccent,
                new Rectangle(e.Bounds.X, e.Bounds.Y, 3, e.Bounds.Height));
        }

        var icono = item.EsGriton ? "🎤" : "👤";
        var texto = $"{icono} {item.Nombre}";
        var victorias = item.Victorias > 0 ? $"🏆 {item.Victorias}" : "";

        using var fNombre = new Font("Segoe UI", 9f, item.EsGriton ? FontStyle.Bold : FontStyle.Regular);
        using var fVictorias = new Font("Segoe UI", 8f, FontStyle.Bold);
        using var brushNombre = new SolidBrush(item.EsGriton ? Paleta.Acento : Paleta.TextoPrimario);
        using var brushV = new SolidBrush(Paleta.AcentoAmbar);

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

    private void lblChatTitulo_Click(object sender, EventArgs e)
    {

    }
}