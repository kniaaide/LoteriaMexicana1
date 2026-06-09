using LoteriaMexicana.Services;

namespace LoteriaMexicana.Forms;

public partial class FormConexion : Form
{
    private static class Paleta
    {
        public static readonly Color Fondo = Color.FromArgb(254, 243, 210);
        public static readonly Color Superficie = Color.FromArgb(255, 255, 240);
        public static readonly Color Rojo = Color.FromArgb(196, 30, 30);
        public static readonly Color RojoOscuro = Color.FromArgb(140, 15, 15);
        public static readonly Color Amarillo = Color.FromArgb(240, 180, 0);
        public static readonly Color AmarilloOscuro = Color.FromArgb(180, 120, 0);
        public static readonly Color Verde = Color.FromArgb(30, 120, 60);
        public static readonly Color TextoPrimario = Color.FromArgb(40, 20, 10);
        public static readonly Color TextoSecund = Color.FromArgb(120, 80, 40);
        public static readonly Color Borde = Color.FromArgb(196, 30, 30);
    }

    public FormConexion()
    {
        InitializeComponent();
        AplicarTema();
        _lblIpLocal.Text = $"📡  Tu IP local: {ServidorSignalR.ObtenerIpLocal()}";
    }

    
    private void AplicarTema()
    {
        BackColor = Paleta.Fondo;
        ForeColor = Paleta.TextoPrimario;
        Font = new Font("Georgia", 9.5f);

        if (lblTitulo != null)
        {
            lblTitulo.ForeColor = Paleta.Rojo;
            lblTitulo.Font = new Font("Georgia", 26f, FontStyle.Bold);
            lblTitulo.Text = "¡LOTERÍA!";
        }

        if (lblSubtitulo != null)
        {
            lblSubtitulo.ForeColor = Paleta.TextoSecund;
            lblSubtitulo.Font = new Font("Georgia", 11f, FontStyle.Italic);
        }

        _lblIpLocal.ForeColor = Paleta.Verde;
        _lblIpLocal.Font = new Font("Segoe UI", 9f, FontStyle.Bold);

        lblEstado.ForeColor = Paleta.TextoSecund;
        lblEstado.Font = new Font("Segoe UI", 9f, FontStyle.Italic);

        EstilarTextBox(txtNombre);
        EstilarTextBox(txtIp);

        foreach (Control c in Controls)
        {
            if (c is Label lbl && c != _lblIpLocal && c != lblEstado
                && c != lblTitulo && c != lblSubtitulo)
            {
                lbl.ForeColor = Paleta.TextoSecund;
                lbl.Font = new Font("Georgia", 9.5f, FontStyle.Bold);
            }
        }

        EstilarBotonPrincipal(btnCrear, "🎴  Crear Sala", Paleta.Rojo, Color.White);
        EstilarBotonPrincipal(btnUnirse, "🃏  Unirse a Sala", Paleta.Amarillo, Paleta.RojoOscuro);

        foreach (Control c in Controls)
        {
            if (c is Panel pnl)
            {
                pnl.BackColor = Paleta.Superficie;
                pnl.Paint += PintarBordeDecorado;
            }
            if (c is GroupBox grp)
            {
                grp.BackColor = Paleta.Superficie;
                grp.ForeColor = Paleta.Rojo;
                grp.Font = new Font("Georgia", 9.5f, FontStyle.Bold);
                foreach (Control inner in grp.Controls)
                {
                    if (inner is Label lbl2)
                    {
                        lbl2.ForeColor = Paleta.TextoSecund;
                        lbl2.Font = new Font("Georgia", 9f);
                    }
                    if (inner is TextBox tb) EstilarTextBox(tb);
                    if (inner is Button bt)
                        EstilarBotonPrincipal(bt, bt.Text, Paleta.Rojo, Color.White);
                }
            }
        }

        Paint += PintarBordeForm;
    }

    private static void EstilarTextBox(TextBox? tb)
    {
        if (tb == null) return;
        tb.BackColor = Paleta.Superficie;
        tb.ForeColor = Paleta.TextoPrimario;
        tb.BorderStyle = BorderStyle.FixedSingle;
        tb.Font = new Font("Segoe UI", 11f);
    }

    private static void EstilarBotonPrincipal(Button? b, string texto, Color fondo, Color textColor)
    {
        if (b == null) return;
        b.Text = texto;
        b.BackColor = fondo;
        b.ForeColor = textColor;
        b.FlatStyle = FlatStyle.Flat;
        b.Font = new Font("Georgia", 11f, FontStyle.Bold);
        b.Cursor = Cursors.Hand;
        b.Height = 42;
        b.FlatAppearance.BorderSize = 2;
        b.FlatAppearance.BorderColor = Paleta.RojoOscuro;
        b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(fondo, 0.2f);
        b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(fondo, 0.1f);
    }

    private static void PintarBordeDecorado(object? sender, PaintEventArgs e)
    {
        if (sender is not Control ctrl) return;
        using var pen = new Pen(Paleta.Borde, 2f);
        e.Graphics.DrawRectangle(pen, 1, 1, ctrl.Width - 3, ctrl.Height - 3);
    }

    private void PintarBordeForm(object? sender, PaintEventArgs e)
    {
        using var brushRojo = new SolidBrush(Paleta.Rojo);
        using var brushAmarillo = new SolidBrush(Paleta.Amarillo);
        e.Graphics.FillRectangle(brushRojo, 0, 0, Width, 6);
        e.Graphics.FillRectangle(brushAmarillo, 0, 6, Width, 4);
        e.Graphics.FillRectangle(brushAmarillo, 0, Height - 10, Width, 4);
        e.Graphics.FillRectangle(brushRojo, 0, Height - 6, Width, 6);
    }
    private async void btnCrear_Click(object sender, EventArgs e)
    {
        if (!ValidarNombre()) return;
        btnCrear.Enabled = false;
        lblEstado.Text = "Iniciando servidor...";
        lblEstado.ForeColor = Paleta.AmarilloOscuro;
        try
        {
            var servidor = new ServidorSignalR();
            await servidor.IniciarAsync();

            var ipLocal = ServidorSignalR.ObtenerIpLocal();
            var cliente = new ClienteSignalR(
                $"http://{ipLocal}:{ServidorSignalR.Puerto}"); // ← CORRECCIÓN

            lblEstado.Text = $"Sala creada. IP para compartir: {ipLocal}:{ServidorSignalR.Puerto}";
            lblEstado.ForeColor = Paleta.Verde;
            _lblIpLocal.Text = $"📡  IP: {ipLocal}:{ServidorSignalR.Puerto}";

            MessageBox.Show(
                $"Sala creada correctamente.\n\n" +
                $"Comparte esta IP con los demas jugadores:\n\n" +
                $"     {ipLocal}\n\n" +
                $"Si alguien no puede conectarse:\n" +
                $"- Acepta el permiso de firewall de Windows cuando aparezca.\n" +
                $"- O ejecuta la app como Administrador para que lo configure automaticamente.",
                "Sala lista", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AbrirJuego(cliente, servidor, txtNombre.Text.Trim(), true);
        }
        catch (Exception ex)
        {
            lblEstado.Text = $"Error: {ex.Message}";
            lblEstado.ForeColor = Paleta.Rojo;
            btnCrear.Enabled = true;
        }
    }

    private async void btnUnirse_Click(object sender, EventArgs e)
    {
        if (!ValidarNombre()) return;
        if (string.IsNullOrWhiteSpace(txtIp.Text))
        {
            MessageBox.Show("Ingresa la IP del Griton.", "Campo requerido",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        btnUnirse.Enabled = false;
        lblEstado.Text = "Conectando...";
        lblEstado.ForeColor = Paleta.AmarilloOscuro;
        try
        {
            var cliente = new ClienteSignalR(
                $"http://{txtIp.Text.Trim()}:{ServidorSignalR.Puerto}"); // ← CORRECCIÓN
            lblEstado.Text = "Abriendo sala...";
            lblEstado.ForeColor = Paleta.Verde;
            AbrirJuego(cliente, null, txtNombre.Text.Trim(), false);
        }
        catch (Exception ex)
        {
            lblEstado.Text = $"No se pudo conectar: {ex.Message}";
            lblEstado.ForeColor = Paleta.Rojo;
            btnUnirse.Enabled = true;
        }
    }

    private void AbrirJuego(ClienteSignalR cliente, ServidorSignalR? servidor, string nombre, bool esHost)
    {
        var form = new FormJuegoRed(cliente, servidor, nombre, esHost);
        form.Show();
        Hide();
        form.FormClosed += (_, _) =>
        {
            Show();
            btnCrear.Enabled = true;
            btnUnirse.Enabled = true;
            lblEstado.Text = "";
            lblEstado.ForeColor = Paleta.TextoSecund;
        };
    }

    private bool ValidarNombre()
    {
        if (!string.IsNullOrWhiteSpace(txtNombre.Text)) return true;
        MessageBox.Show("Ingresa tu nombre.", "Campo requerido",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private void FormConexion_Load(object sender, EventArgs e) { }
}