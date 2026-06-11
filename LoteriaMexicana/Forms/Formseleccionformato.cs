using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Services;

namespace LoteriaMexicana.Forms;

/// <summary>
/// Diálogo de configuración antes de iniciar partida.
/// Permite elegir: formato ganador, dobles y tamaño de tabla.
/// </summary>
public class FormSeleccionFormato : Form
{
    // ── Resultado ─────────────────────────────────────────────────────────────
    public string FormatoElegido { get; private set; } = "TablaLlena";
    public bool PermitirDobles { get; private set; }
    public JuegoService.TamañoTabla TamañoTabla { get; private set; }

    // ── Controles internos ────────────────────────────────────────────────────
    private readonly ComboBox _cmbFormato = new();
    private Button? _btnDobles;
    private Button? _btn4x4;
    private Button? _btn5x5;

    // ── Formatos: solo los valores que existen en FormatoGanador ─────────────
    private static readonly (string Key, string Label)[] Formatos =
    [
        ("FilaCompleta",      "Fila completa"),
        ("ColumnaCompleta",   "Columna completa"),
        ("DiagonalCompleta",  "Diagonal"),
        ("EsquinasCompletas", "4 Esquinas"),
        ("TablaLlena",        "Tabla llena"),
        ("ElleArriba",        "L arriba-izquierda"),
        ("ElleAbajo",         "L abajo-derecha"),
        ("ElleEspejo",        "L espejo"),
        ("CruzCentral",       "Cruz central"),
        ("FormaTee",          "Forma T"),
        ("MarcoCompleto",     "Marco exterior"),
        ("MarcoInterior",     "Marco interior (5×5)"),
        ("DosDiagonales",     "Doble diagonal (X)"),
        ("PrimeraCarta",      "Primera carta"),
        ("TresFilas",         "Tres filas"),
        ("TresColumnas",      "Tres columnas"),
        ("TodasLasFormas",    "⭐ Todas las formas"),
    ];

    // =========================================================================
    // CONSTRUCTOR
    // =========================================================================
    public FormSeleccionFormato(
        bool permitirDobles = false,
        JuegoService.TamañoTabla tamañoTabla = JuegoService.TamañoTabla.Cuatro)
    {
        PermitirDobles = permitirDobles;
        TamañoTabla = tamañoTabla;

        Text = "Configurar partida";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(340, 310);
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f);

        ConstruirUI();
    }

    // =========================================================================
    // UI
    // =========================================================================
    private void ConstruirUI()
    {
        int y = 16;

        Controls.Add(new Label
        {
            Text = "Configurar partida",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.FromArgb(206, 17, 38),
            AutoSize = true,
            Location = new Point(16, y)
        });
        y += 36;

        // ── Formato ganador ───────────────────────────────────────────────────
        Controls.Add(new Label
        {
            Text = "Formato para ganar:",
            AutoSize = true,
            Location = new Point(16, y),
            ForeColor = Color.FromArgb(60, 60, 60)
        });
        y += 22;

        _cmbFormato.Location = new Point(16, y);
        _cmbFormato.Size = new Size(308, 26);
        _cmbFormato.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbFormato.FlatStyle = FlatStyle.Flat;

        foreach (var (key, label) in Formatos)
            _cmbFormato.Items.Add(new FormatoItem(key, label));

        int idxDefault = Array.FindIndex(Formatos, f => f.Key == "TablaLlena");
        _cmbFormato.SelectedIndex = idxDefault >= 0 ? idxDefault : 0;

        Controls.Add(_cmbFormato);
        y += 40;

        // ── Dobles ────────────────────────────────────────────────────────────
        Controls.Add(new Label
        {
            Text = "Cartas repetidas en tabla:",
            AutoSize = true,
            Location = new Point(16, y),
            ForeColor = Color.FromArgb(60, 60, 60)
        });
        y += 22;

        _btnDobles = CrearBoton(
            PermitirDobles ? "🎴  Con dobles ✔" : "🎴  Sin dobles",
            new Point(16, y),
            new Size(160, 32));
        ActualizarEstiloDobles();
        _btnDobles.Click += (s, e) =>
        {
            PermitirDobles = !PermitirDobles;
            _btnDobles.Text = PermitirDobles ? "🎴  Con dobles ✔" : "🎴  Sin dobles";
            ActualizarEstiloDobles();
        };
        Controls.Add(_btnDobles);
        y += 44;

        // ── Tamaño de tabla ───────────────────────────────────────────────────
        Controls.Add(new Label
        {
            Text = "Tamaño de tabla:",
            AutoSize = true,
            Location = new Point(16, y),
            ForeColor = Color.FromArgb(60, 60, 60)
        });
        y += 22;

        _btn4x4 = CrearBoton("4 × 4", new Point(16, y), new Size(100, 32));
        _btn5x5 = CrearBoton("5 × 5", new Point(124, y), new Size(100, 32));

        _btn4x4.Click += (s, e) => { TamañoTabla = JuegoService.TamañoTabla.Cuatro; RefrescarBotonesTabla(); };
        _btn5x5.Click += (s, e) => { TamañoTabla = JuegoService.TamañoTabla.Cinco;  RefrescarBotonesTabla(); };

        Controls.Add(_btn4x4);
        Controls.Add(_btn5x5);
        RefrescarBotonesTabla();
        y += 50;

        // ── OK / Cancelar ─────────────────────────────────────────────────────
        var btnOk = new Button
        {
            Text = "✔  Iniciar partida",
            Size = new Size(150, 34),
            Location = new Point(16, y),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(206, 17, 38),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnOk.FlatAppearance.BorderSize = 0;
        btnOk.Click += (s, e) =>
        {
            if (_cmbFormato.SelectedItem is FormatoItem item)
                FormatoElegido = item.Key;
            DialogResult = DialogResult.OK;
            Close();
        };

        var btnCancelar = new Button
        {
            Text = "Cancelar",
            Size = new Size(90, 34),
            Location = new Point(174, y),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 240, 240),
            ForeColor = Color.FromArgb(60, 60, 60),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancelar.FlatAppearance.BorderSize = 0;

        Controls.Add(btnOk);
        Controls.Add(btnCancelar);
        AcceptButton = btnOk;
        CancelButton = btnCancelar;
    }

    private void ActualizarEstiloDobles()
    {
        if (_btnDobles == null) return;
        _btnDobles.BackColor = PermitirDobles ? Color.FromArgb(0, 104, 56) : Color.FromArgb(240, 240, 240);
        _btnDobles.ForeColor = PermitirDobles ? Color.White : Color.FromArgb(40, 40, 40);
    }

    private void RefrescarBotonesTabla()
    {
        if (_btn4x4 == null || _btn5x5 == null) return;
        bool es4 = TamañoTabla == JuegoService.TamañoTabla.Cuatro;
        _btn4x4.BackColor = es4  ? Color.FromArgb(206, 17, 38) : Color.FromArgb(240, 240, 240);
        _btn4x4.ForeColor = es4  ? Color.White : Color.FromArgb(40, 40, 40);
        _btn5x5.BackColor = !es4 ? Color.FromArgb(206, 17, 38) : Color.FromArgb(240, 240, 240);
        _btn5x5.ForeColor = !es4 ? Color.White : Color.FromArgb(40, 40, 40);
    }

    private static Button CrearBoton(string texto, Point loc, Size size) => new Button
    {
        Text = texto,
        Location = loc,
        Size = size,
        FlatStyle = FlatStyle.Flat,
        Font = new Font("Segoe UI", 9f, FontStyle.Bold),
        Cursor = Cursors.Hand
    };

    private record FormatoItem(string Key, string Label)
    {
        public override string ToString() => Label;
    }
}