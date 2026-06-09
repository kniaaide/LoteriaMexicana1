namespace LoteriaMexicana.Forms;

public partial class FormSeleccionarTabla : Form
{
    private static class Paleta
    {
        public static readonly Color Fondo = Color.FromArgb(254, 243, 210);
        public static readonly Color Superficie = Color.FromArgb(255, 255, 240);
        public static readonly Color Rojo = Color.FromArgb(206, 17, 38);
        public static readonly Color RojoOscuro = Color.FromArgb(140, 15, 15);
        public static readonly Color Amarillo = Color.FromArgb(240, 185, 11);
        public static readonly Color Verde = Color.FromArgb(0, 104, 56);
        public static readonly Color TextoPrimario = Color.FromArgb(40, 20, 10);
        public static readonly Color TextoSecund = Color.FromArgb(120, 80, 40);
        public static readonly Color SeleccionBorde = Color.FromArgb(206, 17, 38);
        public static readonly Color CardSombra = Color.FromArgb(60, 0, 0, 0);
    }
    private record TablaEntry(string Descripcion, string Ruta, string[]? Cartas);

    private readonly TablaEntry[] _entradas;
    private int _indiceSeleccionado = -1;

   
    public string? RutaSeleccionada { get; private set; }

    private readonly Panel pnlHeader;
    private readonly Panel pnlFooter;
    private readonly FlowLayoutPanel flpTablas;
    private readonly Button btnSeleccionar;
    private readonly Button btnCancelar;
    private readonly Label lblTitulo;
    private readonly Label lblInfo;

    public FormSeleccionarTabla(string[] descripciones, string[] rutas,
                                string[][]? datosCartas = null)
    {
        // Construir entradas
        _entradas = new TablaEntry[rutas.Length];
        for (int i = 0; i < rutas.Length; i++)
        {
            string[]? cartas = datosCartas != null && i < datosCartas.Length
                               ? datosCartas[i] : null;

            
            if (cartas == null)
                cartas = LeerCartasDeJson(rutas[i]);

            _entradas[i] = new TablaEntry(descripciones[i], rutas[i], cartas);
        }

       
        Text = "Cargar tabla guardada";
        Size = new Size(760, 600);
        MinimumSize = new Size(600, 480);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Paleta.Fondo;

        
        pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Paleta.Rojo,
            Padding = new Padding(16, 0, 16, 0)
        };
        lblTitulo = new Label
        {
            Dock = DockStyle.Fill,
            Text = "Tablas Guardadas",
            Font = new Font("Georgia", 18f, FontStyle.Bold | FontStyle.Italic),
            ForeColor = Paleta.Amarillo,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlHeader.Controls.Add(lblTitulo);

        var bandaSup = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = Paleta.Amarillo };

       
        lblInfo = new Label
        {
            Dock = DockStyle.Top,
            Height = 34,
            Text = "Seleccioná la tabla con la que querés jugar:",
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Paleta.TextoSecund,
            Padding = new Padding(16, 8, 16, 0),
            TextAlign = ContentAlignment.MiddleLeft
        };

       
        var pnlScroll = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Paleta.Fondo,
            Padding = new Padding(12, 8, 12, 8),
            AutoScroll = false
        };

        flpTablas = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Paleta.Fondo,
            Padding = new Padding(4)
        };
        pnlScroll.Controls.Add(flpTablas);

        
        pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            BackColor = Paleta.Superficie,
            Padding = new Padding(16, 10, 16, 10)
        };

        btnCancelar = new Button
        {
            Text = "Cancelar",
            Width = 110,
            Height = 38,
            Dock = DockStyle.Right,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(180, 160, 140),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.Cancel
        };
        btnCancelar.FlatAppearance.BorderSize = 0;

        btnSeleccionar = new Button
        {
            Text = "✅  Usar esta tabla",
            Width = 160,
            Height = 38,
            Dock = DockStyle.Left,
            FlatStyle = FlatStyle.Flat,
            BackColor = Paleta.Verde,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Enabled = false
        };
        btnSeleccionar.FlatAppearance.BorderSize = 0;
        btnSeleccionar.Click += (_, _) => ConfirmarSeleccion();

        pnlFooter.Controls.Add(btnCancelar);
        pnlFooter.Controls.Add(btnSeleccionar);

        var bandaInf = new Panel { Dock = DockStyle.Bottom, Height = 5, BackColor = Paleta.Rojo };

        
        Controls.Add(pnlScroll);
        Controls.Add(lblInfo);
        Controls.Add(bandaSup);
        Controls.Add(pnlHeader);
        Controls.Add(pnlFooter);
        Controls.Add(bandaInf);

        AcceptButton = btnSeleccionar;
        CancelButton = btnCancelar;

        Load += (_, _) => PoblarTarjetas();

       
        flpTablas.Resize += (_, _) => AjustarAnchoTarjetas();
    }

  
    private static string[]? LeerCartasDeJson(string ruta)
    {
        try
        {
            if (!File.Exists(ruta)) return null;
            var txt = File.ReadAllText(ruta);
            using var doc = System.Text.Json.JsonDocument.Parse(txt);
            var root = doc.RootElement;

            if (!root.TryGetProperty("cartas", out var arrCartas)) return null;

            var lista = new List<string>();
            foreach (var carta in arrCartas.EnumerateArray())
            {
                if (carta.ValueKind == System.Text.Json.JsonValueKind.String)
                {
                    lista.Add(carta.GetString() ?? "");
                }
                else if (carta.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    // Preferir campo "imagen" (ruta), sino "nombre"
                    if (carta.TryGetProperty("imagen", out var img))
                        lista.Add(img.GetString() ?? "");
                    else if (carta.TryGetProperty("nombre", out var nom))
                        lista.Add(nom.GetString() ?? "");
                }
            }
            return lista.Count > 0 ? lista.ToArray() : null;
        }
        catch { return null; }
    }

    
    private readonly List<Panel> _tarjetas = new();

    private void PoblarTarjetas()
    {
        flpTablas.SuspendLayout();
        flpTablas.Controls.Clear();
        _tarjetas.Clear();

        for (int i = 0; i < _entradas.Length; i++)
        {
            var tarjeta = CrearTarjeta(i);
            _tarjetas.Add(tarjeta);
            flpTablas.Controls.Add(tarjeta);
        }

        flpTablas.ResumeLayout();
        AjustarAnchoTarjetas();

        if (_entradas.Length > 0)
            SeleccionarTarjeta(0);
    }

    private Panel CrearTarjeta(int idx)
    {
        var entrada = _entradas[idx];

        var card = new Panel
        {
            BackColor = Paleta.Superficie,
            BorderStyle = BorderStyle.None,
            Cursor = Cursors.Hand,
            Padding = new Padding(12),
            Margin = new Padding(4, 4, 4, 6),
            Height = 220   // altura inicial; se ajusta si hay cartas
        };

        
        card.Paint += (s, e) => PintarTarjeta(s as Panel, e, idx);
        card.Click += (_, _) => SeleccionarTarjeta(idx);

       
        var pnlTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 38,
            BackColor = Color.Transparent
        };
        pnlTop.Click += (_, _) => SeleccionarTarjeta(idx);

        var lblDesc = new Label
        {
            Dock = DockStyle.Fill,
            Text = entrada.Descripcion,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            ForeColor = Paleta.TextoPrimario,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(4, 0, 0, 0)
        };
        lblDesc.Click += (_, _) => SeleccionarTarjeta(idx);
        pnlTop.Controls.Add(lblDesc);
        card.Controls.Add(pnlTop);

        var sep = new Panel
        {
            Dock = DockStyle.Top,
            Height = 2,
            BackColor = Paleta.Amarillo,
            Margin = new Padding(0, 2, 0, 4)
        };
        card.Controls.Add(sep);

        
        var pnlCartas = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = false,
            BackColor = Color.Transparent,
            Padding = new Padding(2)
        };
        pnlCartas.Click += (_, _) => SeleccionarTarjeta(idx);

        if (entrada.Cartas != null && entrada.Cartas.Length > 0)
        {
            int maxMostrar = Math.Min(entrada.Cartas.Length, 16);
            for (int c = 0; c < maxMostrar; c++)
            {
                var miniatura = CrearMiniatura(entrada.Cartas[c], idx);
                pnlCartas.Controls.Add(miniatura);
            }

            
            if (entrada.Cartas.Length > maxMostrar)
            {
                var lblMas = new Label
                {
                    Text = $"+{entrada.Cartas.Length - maxMostrar} más",
                    Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                    ForeColor = Paleta.TextoSecund,
                    AutoSize = true,
                    Padding = new Padding(4, 8, 0, 0)
                };
                lblMas.Click += (_, _) => SeleccionarTarjeta(idx);
                pnlCartas.Controls.Add(lblMas);
            }

            card.Height = 240;
        }
        else
        {
            
            var lblSinDatos = new Label
            {
                Dock = DockStyle.Fill,
                Text = "🃏  (sin preview disponible)",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Paleta.TextoSecund,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblSinDatos.Click += (_, _) => SeleccionarTarjeta(idx);
            pnlCartas.Controls.Add(lblSinDatos);
            card.Height = 100;
        }

        card.Controls.Add(pnlCartas);

        return card;
    }

   
    private Control CrearMiniatura(string cartaRef, int idxTabla)
    {
        const int W = 60, H = 80;

        var pnl = new Panel
        {
            Size = new Size(W, H),
            Margin = new Padding(3),
            BackColor = Color.White,
            BorderStyle = BorderStyle.None,
            Cursor = Cursors.Hand
        };
        pnl.Click += (_, _) => SeleccionarTarjeta(idxTabla);

       
        Image? img = CargarImagen(cartaRef);

        if (img != null)
        {
            var pic = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };
            pic.Click += (_, _) => SeleccionarTarjeta(idxTabla);
            pnl.Controls.Add(pic);
        }
        else
        {
            var lbl = new Label
            {
                Dock = DockStyle.Fill,
                Text = AcortarNombre(cartaRef),
                Font = new Font("Segoe UI", 7f),
                ForeColor = Paleta.TextoPrimario,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(2)
            };
            lbl.Click += (_, _) => SeleccionarTarjeta(idxTabla);
            pnl.Controls.Add(lbl);

            pnl.BackColor = Color.FromArgb(250, 240, 220);
        }

        pnl.Paint += (s, e) =>
        {
            var p = s as Panel;
            if (p == null) return;
            using var pen = new System.Drawing.Pen(Paleta.Amarillo, 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
        };

        return pnl;
    }

    private void SeleccionarTarjeta(int idx)
    {
        _indiceSeleccionado = idx;
        btnSeleccionar.Enabled = true;

        // Repintar todas las tarjetas para actualizar borde de selección
        foreach (var t in _tarjetas)
            t.Invalidate();
    }

    private void PintarTarjeta(Panel? card, PaintEventArgs e, int idx)
    {
        if (card == null) return;
        bool sel = (idx == _indiceSeleccionado);

        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

       
        using var brushFondo = new SolidBrush(sel ? Color.FromArgb(255, 252, 240) : Paleta.Superficie);
        e.Graphics.FillRectangle(brushFondo, 0, 0, card.Width, card.Height);

        var colorBorde = sel ? Paleta.SeleccionBorde : Color.FromArgb(220, 200, 170);
        float grosor = sel ? 2.5f : 1f;
        using var pen = new System.Drawing.Pen(colorBorde, grosor);
        e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);

       
        if (sel)
        {
            using var brushFranja = new SolidBrush(Paleta.Rojo);
            e.Graphics.FillRectangle(brushFranja, 0, 0, 5, card.Height);
        }
    }

  
    private void AjustarAnchoTarjetas()
    {
        int ancho = flpTablas.ClientSize.Width - flpTablas.Padding.Horizontal - 12;
        foreach (var t in _tarjetas)
            t.Width = ancho;
    }

    private static Image? CargarImagen(string referencia)
    {
        if (string.IsNullOrWhiteSpace(referencia)) return null;
        try
        {
            if (File.Exists(referencia))
                return Image.FromFile(referencia);
        }
        catch { /* imagen no encontrada o inválida */ }
        return null;
    }

    private static string AcortarNombre(string nombre)
    {
        var n = Path.GetFileNameWithoutExtension(nombre);
        if (n.Length > 12) n = n[..12] + "…";
        return n;
    }

    private void ConfirmarSeleccion()
    {
        if (_indiceSeleccionado < 0 || _indiceSeleccionado >= _entradas.Length)
        {
            MessageBox.Show("Seleccioná una tabla de la lista.", "Ninguna tabla elegida",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        RutaSeleccionada = _entradas[_indiceSeleccionado].Ruta;
        DialogResult = DialogResult.OK;
        Close();
    }
}