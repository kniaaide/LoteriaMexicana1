using LoteriaMexicana.Services;

namespace LoteriaMexicana.Forms;

/// <summary>
/// Permite al jugador elegir manualmente sus cartas para armar su tabla.
/// Muestra las 54 cartas en una cuadrícula y el jugador selecciona
/// exactamente las que necesita (16 para 4×4 o 25 para 5×5).
/// </summary>
public class FormElegirCartas : Form
{
    // ── Resultado ─────────────────────────────────────────────────────────────
    public List<int> NumerosElegidos { get; private set; } = [];

    // ── Config ────────────────────────────────────────────────────────────────
    private readonly int _requeridas;
    private readonly string _carpetaCartas;

    // ── Estado ────────────────────────────────────────────────────────────────
    private readonly HashSet<int> _seleccionados = [];
    private readonly Dictionary<int, Panel> _paneles = [];

    // ── Controles ─────────────────────────────────────────────────────────────
    private Label _lblContador = new();
    private Button _btnConfirmar = new();
    private Button _btnCancelar = new();
    private Panel _pnlGrid = new();

    // ── Colores ───────────────────────────────────────────────────────────────
    private static readonly Color ROJO = Color.FromArgb(206, 17, 38);
    private static readonly Color VERDE = Color.FromArgb(0, 104, 56);
    private static readonly Color DORADO = Color.FromArgb(240, 185, 11);
    private static readonly Color FONDO = Color.FromArgb(255, 250, 235);
    private static readonly Color SEL_BG = Color.FromArgb(220, 255, 220);
    private static readonly Color SEL_BOR = Color.FromArgb(0, 150, 80);

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================
    public FormElegirCartas(int tamañoTabla)
    {
        _requeridas = tamañoTabla * tamañoTabla;
        _carpetaCartas = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Resources", "Cartas");

        Text = $"Elige tus {_requeridas} cartas";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = FONDO;
        Font = new Font("Segoe UI", 9f);
        ClientSize = new Size(920, 680);

        ConstruirUI();
        CargarCartas();
    }

    // =========================================================================
    // UI
    // =========================================================================
    private void ConstruirUI()
    {
        // ── Encabezado ────────────────────────────────────────────────────────
        var pnlTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 56,
            BackColor = ROJO
        };

        var lblTitulo = new Label
        {
            Text = "🃏  ELIGE TUS CARTAS",
            Font = new Font("Georgia", 13f, FontStyle.Bold | FontStyle.Italic),
            ForeColor = DORADO,
            Dock = DockStyle.Left,
            Width = 320,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 0, 0)
        };

        _lblContador = new Label
        {
            Text = $"Seleccionadas: 0 / {_requeridas}",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Padding = new Padding(0, 0, 16, 0)
        };

        pnlTop.Controls.Add(_lblContador);
        pnlTop.Controls.Add(lblTitulo);

        // ── Banda dorada ──────────────────────────────────────────────────────
        var banda = new Panel
        {
            Dock = DockStyle.Top,
            Height = 5,
            BackColor = DORADO
        };

        // ── Instrucción ───────────────────────────────────────────────────────
        var lblHint = new Label
        {
            Text = $"Haz clic en cada carta para seleccionarla/deseleccionarla. Necesitas exactamente {_requeridas}.",
            Dock = DockStyle.Top,
            Height = 32,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
            ForeColor = Color.FromArgb(80, 80, 80),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.White,
            Padding = new Padding(0, 4, 0, 4)
        };

        // ── Grid de cartas ────────────────────────────────────────────────────
        _pnlGrid = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = FONDO,
            Padding = new Padding(10)
        };

        // ── Pie ───────────────────────────────────────────────────────────────
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 54,
            BackColor = Color.White,
            Padding = new Padding(16, 8, 16, 8)
        };

        _btnCancelar = new Button
        {
            Text = "Cancelar",
            Size = new Size(110, 36),
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 240, 240),
            ForeColor = Color.FromArgb(60, 60, 60),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        _btnCancelar.FlatAppearance.BorderSize = 0;

        _btnConfirmar = new Button
        {
            Text = $"✔  Usar estas {_requeridas} cartas",
            Size = new Size(200, 36),
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = VERDE,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Enabled = false
        };
        _btnConfirmar.FlatAppearance.BorderSize = 0;
        _btnConfirmar.Click += BtnConfirmar_Click;

        // Espaciador entre botones
        var sep = new Panel { Width = 8, Dock = DockStyle.Right, BackColor = Color.White };

        pnlBottom.Controls.Add(_btnCancelar);
        pnlBottom.Controls.Add(sep);
        pnlBottom.Controls.Add(_btnConfirmar);

        // ── Separador dorado pie ──────────────────────────────────────────────
        var bandaPie = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 4,
            BackColor = DORADO
        };

        Controls.Add(_pnlGrid);
        Controls.Add(lblHint);
        Controls.Add(banda);
        Controls.Add(pnlTop);
        Controls.Add(pnlBottom);
        Controls.Add(bandaPie);

        AcceptButton = _btnConfirmar;
        CancelButton = _btnCancelar;
    }

    // =========================================================================
    // CARGAR LAS 54 CARTAS
    // =========================================================================
    private void CargarCartas()
    {
        // Calcula cuántas columnas caben en el panel (cada celda ~100px)
        const int CEL = 100;
        int cols = Math.Max(1, (ClientSize.Width - 20) / CEL);

        int x = 10, y = 10;
        int col = 0;

        for (int numero = 1; numero <= 54; numero++)
        {
            int n = numero; // captura para lambda

            var pnl = new Panel
            {
                Size = new Size(CEL - 4, 116),
                Location = new Point(x, y),
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                Tag = n
            };

            // Borde
            pnl.Paint += (s, e) =>
            {
                bool sel = _seleccionados.Contains(n);
                using var pen = new Pen(
                    sel ? SEL_BOR : Color.FromArgb(200, 200, 200),
                    sel ? 2.5f : 1f);
                e.Graphics.DrawRectangle(pen, 0, 0, pnl.Width - 1, pnl.Height - 1);
            };

            // Imagen
            var pic = new PictureBox
            {
                Size = new Size(CEL - 10, 84),
                Location = new Point(3, 3),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            var ruta = Path.Combine(_carpetaCartas, $"{n}.jpg");
            if (File.Exists(ruta))
                pic.Image = Image.FromFile(ruta);

            // Número
            var lbl = new Label
            {
                Text = $"#{n}",
                Font = new Font("Segoe UI", 7f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 20,
                BackColor = Color.Transparent
            };

            pnl.Controls.Add(pic);
            pnl.Controls.Add(lbl);

            // Click en panel, imagen y label
            EventHandler handler = (s, e) => ToggleSeleccion(n, pnl);
            pnl.Click += handler;
            pic.Click += handler;
            lbl.Click += handler;

            _pnlGrid.Controls.Add(pnl);
            _paneles[n] = pnl;

            // Avanza posición
            col++;
            if (col >= cols) { col = 0; x = 10; y += pnl.Height + 4; }
            else { x += CEL; }
        }
    }

    // =========================================================================
    // TOGGLE SELECCIÓN
    // =========================================================================
    private void ToggleSeleccion(int numero, Panel pnl)
    {
        if (_seleccionados.Contains(numero))
        {
            _seleccionados.Remove(numero);
            pnl.BackColor = Color.White;
        }
        else
        {
            if (_seleccionados.Count >= _requeridas)
            {
                // Ya tiene las suficientes — parpadea el contador
                _lblContador.ForeColor = ROJO;
                Task.Delay(400).ContinueWith(_ =>
                    InvokeIfNeeded(() => _lblContador.ForeColor = Color.White));
                return;
            }
            _seleccionados.Add(numero);
            pnl.BackColor = SEL_BG;
        }

        pnl.Invalidate();
        ActualizarContador();
    }

    private void ActualizarContador()
    {
        int n = _seleccionados.Count;
        _lblContador.Text = $"Seleccionadas: {n} / {_requeridas}";
        _lblContador.ForeColor = n == _requeridas ? DORADO : Color.White;
        _btnConfirmar.Enabled = n == _requeridas;
    }

    // =========================================================================
    // CONFIRMAR
    // =========================================================================
    private void BtnConfirmar_Click(object? sender, EventArgs e)
    {
        if (_seleccionados.Count != _requeridas) return;
        NumerosElegidos = [.. _seleccionados];
        DialogResult = DialogResult.OK;
        Close();
    }

    // =========================================================================
    // HELPER
    // =========================================================================
    private void InvokeIfNeeded(Action a)
    {
        if (IsDisposed) return;
        if (InvokeRequired) Invoke(a);
        else a();
    }
}