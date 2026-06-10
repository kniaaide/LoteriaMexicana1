using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;

namespace LoteriaMexicana.Forms;

public partial class FormConfiguracion : Form
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

    public ConfiguracionJuego Configuracion { get; private set; }

    private NumericUpDown nudTamaño;
    private CheckBox chkPermitirDobles;
    private GroupBox grpFormatos;
    private Button btnAceptar;
    private Button btnCancelar;
    private Label lblInfo;

    public FormConfiguracion(ConfiguracionJuego configActual)
    {
        Configuracion = configActual;
        Text = "Configuración del Juego";
        Size = new Size(500, 600);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Paleta.Fondo;
        ForeColor = Paleta.TextoPrimario;

        ConstruirUI();
        AplicarTema();
        CargarConfiguracion();
    }

    private void ConstruirUI()
    {
        // Panel superior con título
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Paleta.Rojo
        };

        var lblTitulo = new Label
        {
            Dock = DockStyle.Fill,
            Text = "⚙️  Configuración del Juego",
            Font = new Font("Georgia", 16f, FontStyle.Bold),
            ForeColor = Paleta.Amarillo,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblTitulo);
        Controls.Add(pnlHeader);

        var banda = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Paleta.Amarillo };
        Controls.Add(banda);

        // Panel de contenido
        var pnlContenido = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Paleta.Fondo,
            Padding = new Padding(20, 20, 20, 20),
            AutoScroll = true
        };

        // Tamaño de tabla
        var lblTamaño = new Label
        {
            Text = "Tamaño de la Tabla:",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Paleta.TextoPrimario,
            AutoSize = true,
            Location = new Point(0, 0)
        };
        pnlContenido.Controls.Add(lblTamaño);

        nudTamaño = new NumericUpDown
        {
            Minimum = 4,
            Maximum = 10,
            Value = 5,
            Location = new Point(0, 30),
            Width = 100,
            Height = 30,
            Font = new Font("Segoe UI", 10f)
        };
        pnlContenido.Controls.Add(nudTamaño);

        var lblTamaño2 = new Label
        {
            Text = "x",
            Font = new Font("Segoe UI", 10f),
            Location = new Point(110, 30),
            Width = 20,
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlContenido.Controls.Add(lblTamaño2);

        var lblTamaño3 = new Label
        {
            Text = "5",
            Font = new Font("Segoe UI", 10f),
            Location = new Point(135, 30),
            Width = 30,
            Height = 30,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlContenido.Controls.Add(lblTamaño3);

        nudTamaño.ValueChanged += (s, e) => lblTamaño3.Text = nudTamaño.Value.ToString();

        // Permitir cartas dobles
        chkPermitirDobles = new CheckBox
        {
            Text = "✓ Permitir cartas dobles en la tabla",
            Font = new Font("Segoe UI", 10f),
            ForeColor = Paleta.TextoPrimario,
            Location = new Point(0, 80),
            Width = 400,
            Height = 25,
            Checked = true
        };
        pnlContenido.Controls.Add(chkPermitirDobles);

        // Formas de ganar
        var lblFormatos = new Label
        {
            Text = "Formas de Ganar (marca las que activan la victoria):",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Paleta.TextoPrimario,
            Location = new Point(0, 120),
            AutoSize = true
        };
        pnlContenido.Controls.Add(lblFormatos);

        grpFormatos = new GroupBox
        {
            Text = "Patrones Ganadores",
            Font = new Font("Segoe UI", 9f),
            ForeColor = Paleta.Rojo,
            Location = new Point(0, 150),
            Width = 420,
            Height = 200,
            BackColor = Paleta.Superficie
        };

        var formatos = new[]
        {
            ("Línea Horizontal", FormatoGanador.LineaHorizontal),
            ("Línea Vertical", FormatoGanador.LineaVertical),
            ("Diagonal", FormatoGanador.Diagonal),
            ("Cruz", FormatoGanador.Cruz),
            ("Cruzita (Plus)", FormatoGanador.Cruzita),
            ("Tabla Llena", FormatoGanador.TablaLlena)
        };

        int y = 25;
        foreach (var (nombre, formato) in formatos)
        {
            var chk = new CheckBox
            {
                Text = nombre,
                Font = new Font("Segoe UI", 9f),
                Location = new Point(20, y),
                Width = 300,
                Height = 25,
                Checked = true,
                Tag = formato
            };
            grpFormatos.Controls.Add(chk);
            y += 30;
        }

        pnlContenido.Controls.Add(grpFormatos);

        // Info
        lblInfo = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9f, FontStyle.Italic),
            ForeColor = Paleta.TextoSecund,
            Location = new Point(0, 360),
            Width = 400,
            Height = 60,
            AutoSize = false
        };
        pnlContenido.Controls.Add(lblInfo);

        Controls.Add(pnlContenido);

        // Botones
        var pnlBotones = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Paleta.Superficie
        };

        btnAceptar = new Button
        {
            Text = "✓ Aceptar",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            BackColor = Paleta.Verde,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width = 120,
            Height = 38,
            Location = new Point(20, 11),
            Cursor = Cursors.Hand
        };
        btnAceptar.Click += (s, e) => GuardarYCerrar();
        pnlBotones.Controls.Add(btnAceptar);

        btnCancelar = new Button
        {
            Text = "✕ Cancelar",
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            BackColor = Paleta.Rojo,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Width = 120,
            Height = 38,
            Location = new Point(160, 11),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancelar.Click += (s, e) => Close();
        pnlBotones.Controls.Add(btnCancelar);

        Controls.Add(pnlBotones);

        AcceptButton = btnAceptar;
        CancelButton = btnCancelar;
    }

    private void AplicarTema()
    {
        foreach (Control ctrl in GetAllControls(this))
        {
            if (ctrl is NumericUpDown nud)
            {
                nud.BackColor = Paleta.Superficie;
                nud.ForeColor = Paleta.TextoPrimario;
            }
            if (ctrl is CheckBox chk)
            {
                chk.BackColor = Color.Transparent;
            }
        }
    }

    private void CargarConfiguracion()
    {
        nudTamaño.Value = Configuracion.TamañoTabla;
        chkPermitirDobles.Checked = Configuracion.PermitirCartasDobles;

        foreach (CheckBox chk in grpFormatos.Controls.OfType<CheckBox>())
        {
            if (chk.Tag is FormatoGanador formato)
                chk.Checked = Configuracion.FormatosActivos.Contains(formato);
        }

        ActualizarInfo();
    }

    private void GuardarYCerrar()
    {
        Configuracion.TamañoTabla = (int)nudTamaño.Value;
        Configuracion.PermitirCartasDobles = chkPermitirDobles.Checked;

        var formatosSeleccionados = new List<FormatoGanador>();
        foreach (CheckBox chk in grpFormatos.Controls.OfType<CheckBox>())
        {
            if (chk.Checked && chk.Tag is FormatoGanador formato)
                formatosSeleccionados.Add(formato);
        }

        if (formatosSeleccionados.Count == 0)
        {
            MessageBox.Show("Debes seleccionar al menos una forma de ganar.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Configuracion.FormatosActivos = formatosSeleccionados;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ActualizarInfo()
    {
        int formas = grpFormatos.Controls.OfType<CheckBox>().Count(c => c.Checked);
        lblInfo.Text = $"📋 Tabla: {nudTamaño.Value}x{nudTamaño.Value}\n" +
                       $"🎲 Cartas dobles: {(chkPermitirDobles.Checked ? "Permitidas" : "No permitidas")}\n" +
                       $"🏆 Formas de ganar: {formas}";
    }

    private IEnumerable<Control> GetAllControls(Control parent)
    {
        foreach (Control ctrl in parent.Controls)
        {
            yield return ctrl;
            foreach (var child in GetAllControls(ctrl))
                yield return child;
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.ResumeLayout(false);
    }
}
