using LoteriaMexicana.Domain;

namespace LoteriaMexicana.Forms;

public partial class FormCrearTabla : Form
{
    private static class Paleta
    {
        public static readonly Color Fondo = Color.FromArgb(254, 243, 210);
        public static readonly Color Superficie = Color.FromArgb(255, 255, 240);
        public static readonly Color Rojo = Color.FromArgb(206, 17, 38);
        public static readonly Color Verde = Color.FromArgb(0, 104, 56);
        public static readonly Color Amarillo = Color.FromArgb(240, 185, 11);
        public static readonly Color TextoPrimario = Color.FromArgb(40, 20, 10);
        public static readonly Color TextoSecund = Color.FromArgb(120, 80, 40);
    }

    public Tabla TablaCreada { get; private set; }
    private int _tamaño;
    private List<Carta> _cartasDisponibles;
    private TableLayoutPanel _grilla;
    private List<int> _cartasSeleccionadas;

    public FormCrearTabla(int tamaño, List<Carta> cartasDisponibles)
    {
        _tamaño = tamaño;
        _cartasDisponibles = cartasDisponibles;
        _cartasSeleccionadas = new List<int>();

        Text = "Crear Tabla Personalizada";
        Size = new Size(800, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        BackColor = Paleta.Fondo;
        ForeColor = Paleta.TextoPrimario;

        ConstruirUI();
    }

    private void ConstruirUI()
    {
        // Header
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Paleta.Rojo
        };

        var lblTitulo = new Label
        {
            Dock = DockStyle.Fill,
            Text = $"📋  Crear Tabla {_tamaño}x{_tamaño} Personalizada",
            Font = new Font("Georgia", 14f, FontStyle.Bold),
            ForeColor = Paleta.Amarillo,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblTitulo);
        Controls.Add(pnlHeader);

        var banda = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Paleta.Amarillo };
        Controls.Add(banda);

        // Contenido principal
        var pnlMain = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Paleta.Fondo,
            Padding = new Padding(15),
            AutoScroll = true
        };

        var lblInfo = new Label
        {
            Text = "Haz clic en las cartas para seleccionarlas. Necesitas " + (_tamaño * _tamaño) + " cartas.",
            Font = new Font("Segoe UI", 9f),
            ForeColor = Paleta.TextoSecund,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        pnlMain.Controls.Add(lblInfo);

        // Grilla de cartas disponibles
        var lblCartas = new Label
        {
            Text = "Cartas Disponibles:",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Paleta.TextoPrimario,
            Location = new Point(0, 30),
            AutoSize = true
        };
        pnlMain.Controls.Add(lblCartas);

        var flpCartas = new FlowLayoutPanel
        {
            Location = new Point(0, 60),
            Size = new Size(700, 200),
            BackColor = Paleta.Superficie,
            BorderStyle = BorderStyle.FixedSingle,
            AutoScroll = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(5)
        };

        foreach (var carta in _cartasDisponibles)
        {
            var btn = new Button
            {
                Text = $"{carta.Numero}\n{carta.Nombre}",
                Size = new Size(70, 80),
                Font = new Font("Segoe UI", 7f),
                BackColor = Paleta.Superficie,
                ForeColor = Paleta.TextoPrimario,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3),
                Cursor = Cursors.Hand,
                Tag = carta
            };

            btn.Click += (s, e) => SeleccionarCarta(btn, carta);
            flpCartas.Controls.Add(btn);
        }

        pnlMain.Controls.Add(flpCartas);

        // Preview de tabla
        var lblPreview = new Label
        {
            Text = "Preview de tu Tabla (0/" + (_tamaño * _tamaño) + " casillas):",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Paleta.TextoPrimario,
            Location = new Point(0, 270),
            AutoSize = true,
            Tag = "preview"
        };
        pnlMain.Controls.Add(lblPreview);

        // Grilla de tabla
        _grilla = new TableLayoutPanel
        {
            Location = new Point(0, 300),
            Size = new Size(_tamaño * 80, _tamaño * 100),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.All
        };

        for (int i = 0; i < _tamaño; i++)
            _grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        for (int i = 0; i < _tamaño; i++)
            _grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));

        for (int f = 0; f < _tamaño; f++)
        {
            for (int c = 0; c < _tamaño; c++)
            {
                var lbl = new Label
                {
                    Text = "—",
                    BackColor = Paleta.Superficie,
                    ForeColor = Paleta.TextoSecund,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 8f),
                    Dock = DockStyle.Fill
                };
                _grilla.Controls.Add(lbl, c, f);
            }
        }

        pnlMain.Controls.Add(_grilla);

        Controls.Add(pnlMain);

        // Botones
        var pnlBotones = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Paleta.Superficie
        };

        var btnLimpiar = new Button
        {
            Text = "🗑️  Limpiar",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            BackColor = Paleta.Amarillo,
            ForeColor = Paleta.TextoPrimario,
            FlatStyle = FlatStyle.Flat,
            Width = 100,
            Height = 38,
            Location = new Point(15, 11),
            Cursor = Cursors.Hand
        };
        btnLimpiar.Click += (s, e) => LimpiarTabla();
        pnlBotones.Controls.Add(btnLimpiar);

        var btnCrear = new Button
        {
            Text = "✓ Crear Tabla",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            BackColor = Paleta.Verde,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width = 120,
            Height = 38,
            Location = new Point(130, 11),
            Cursor = Cursors.Hand
        };
        btnCrear.Click += (s, e) => CrearTabla();
        pnlBotones.Controls.Add(btnCrear);

        var btnCancelar = new Button
        {
            Text = "✕ Cancelar",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            BackColor = Paleta.Rojo,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width = 100,
            Height = 38,
            Location = new Point(270, 11),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancelar.Click += (s, e) => Close();
        pnlBotones.Controls.Add(btnCancelar);

        Controls.Add(pnlBotones);
    }

    private void SeleccionarCarta(Button btn, Carta carta)
    {
        if (_cartasSeleccionadas.Count >= _tamaño * _tamaño)
        {
            MessageBox.Show($"Ya has seleccionado {_tamaño * _tamaño} cartas.", "Tabla completa",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _cartasSeleccionadas.Add(carta.Numero);
        btn.BackColor = Paleta.Verde;
        btn.ForeColor = Color.White;
        btn.Enabled = false;

        ActualizarPreview();
    }

    private void ActualizarPreview()
    {
        int fila = 0, col = 0;

        foreach (Control ctrl in _grilla.Controls)
        {
            if (ctrl is Label lbl)
            {
                int idx = _grilla.Controls.IndexOf(lbl);
                fila = idx / _tamaño;
                col = idx % _tamaño;

                if (idx < _cartasSeleccionadas.Count)
                {
                    var carta = _cartasDisponibles.FirstOrDefault(c => c.Numero == _cartasSeleccionadas[idx]);
                    if (carta != null)
                    {
                        lbl.Text = $"{carta.Numero}\n{carta.Nombre}";
                        lbl.BackColor = Paleta.Superficie;
                        lbl.ForeColor = Paleta.TextoPrimario;
                    }
                }
                else
                {
                    lbl.Text = "—";
                    lbl.BackColor = Color.White;
                    lbl.ForeColor = Paleta.TextoSecund;
                }
            }
        }

        // Actualizar contador
        foreach (Control c in Parent?.Controls ?? Controls)
        {
            if (c.Tag?.ToString() == "preview" && c is Label lbl)
            {
                lbl.Text = $"Preview de tu Tabla ({_cartasSeleccionadas.Count}/{_tamaño * _tamaño} casillas):";
            }
        }
    }

    private void LimpiarTabla()
    {
        _cartasSeleccionadas.Clear();
        ActualizarPreview();

        foreach (Control c in Controls.OfType<Panel>().FirstOrDefault()?.Controls ?? Enumerable.Empty<Control>())
        {
            if (c is Button btn && btn.Tag is Carta)
            {
                btn.BackColor = Paleta.Superficie;
                btn.ForeColor = Paleta.TextoPrimario;
                btn.Enabled = true;
            }
        }
    }

    private void CrearTabla()
    {
        if (_cartasSeleccionadas.Count != _tamaño * _tamaño)
        {
            MessageBox.Show($"Debes seleccionar exactamente {_tamaño * _tamaño} cartas.", "Tabla incompleta",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var cartasOrdenadas = new List<Carta>();
        foreach (var num in _cartasSeleccionadas)
        {
            var carta = _cartasDisponibles.FirstOrDefault(c => c.Numero == num);
            if (carta != null)
                cartasOrdenadas.Add(carta);
        }

        var casillas = new Carta[_tamaño, _tamaño];
        for (int i = 0; i < cartasOrdenadas.Count; i++)
            casillas[i / _tamaño, i % _tamaño] = cartasOrdenadas[i];

        TablaCreada = new Tabla(_tamaño, casillas);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.ResumeLayout(false);
    }
}

// Extensión de Tabla para aceptar tamaño variable
public partial class Tabla
{
    public Tabla(int tamaño, Carta[,] casillas)
    {
        var casillasVar = new Carta[tamaño, tamaño];
        for (int i = 0; i < tamaño; i++)
            for (int j = 0; j < tamaño; j++)
                casillasVar[i, j] = casillas[i, j];
        
        Filas = tamaño;
        Columnas = tamaño;
        Casillas = casillasVar;
    }

    private int _filas;
    private int _columnas;

    public int Filas 
    { 
        get => _filas; 
        private set => _filas = value; 
    }
    
    public int Columnas 
    { 
        get => _columnas; 
        private set => _columnas = value; 
    }
}
