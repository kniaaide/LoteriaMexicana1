namespace LoteriaMexicana.Forms;

partial class FormJuegoRed
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnCrearTabla = new Button();
        pnlTopBar = new Panel();
        lblAppTitle = new Label();
        lblUsuario = new Label();
        pnlBandaDorada = new Panel();
        tblMain = new TableLayoutPanel();
        pnlIzquierdo = new Panel();
        pnlConfigHost = new Panel();
        lblSalaInfo = new Label();
        btnIniciar = new Button();
        btnCrearTabla = new Button();
        pnlCartaCard = new Panel();
        pnlCartaBanda = new Panel();
        lblCartaCardTit = new Label();
        picCarta = new PictureBox();
        lblNombreCarta = new Label();
        lblFrase = new Label();
        btnCantarCarta = new Button();
        chkTts = new CheckBox();
        lblFichasTxt = new Label();
        lblFichaHint = new Label();
        flpFichas = new FlowLayoutPanel();
        lblHistorialTxt = new Label();
        flpHistorial = new FlowLayoutPanel();
        pnlDerecho = new Panel();
        lblMensaje = new Label();
        tblSplit = new TableLayoutPanel();
        pnlGrilla = new Panel();
        btnLoteria = new Button();
        btnNuevaTabla = new Button();
        btnGuardarTabla = new Button();
        btnCargarTabla = new Button();
        btnReiniciarPartida = new Button();
        grilla = new TableLayoutPanel();
        lblPatrones = new Label();
        lblContadorCartas = new Label();
        pnlJugadores = new Panel();
        pnlChat = new Panel();
        rtbChat = new RichTextBox();
        pnlChatInput = new Panel();
        txtMensaje = new TextBox();
        btnEnviar = new Button();
        lblChatTitulo = new Label();
        pnlChatBanda = new Panel();
        lstJugadores = new ListBox();
        pnlSepJug = new Panel();
        lblConectados = new Label();
        lblSalaTitulo = new Label();
        bandaHost = new Panel();
        lblHostTit = new Label();

        pnlTopBar.SuspendLayout();
        tblMain.SuspendLayout();
        pnlIzquierdo.SuspendLayout();
        pnlConfigHost.SuspendLayout();
        pnlCartaCard.SuspendLayout();
        pnlConfigHost.Controls.Add(lblSalaInfo);
        pnlConfigHost.Controls.Add(btnIniciar);
        pnlConfigHost.Controls.Add(btnCrearTabla); 
        ((System.ComponentModel.ISupportInitialize)picCarta).BeginInit();
        pnlDerecho.SuspendLayout();
        tblSplit.SuspendLayout();
        pnlGrilla.SuspendLayout();
        pnlJugadores.SuspendLayout();
        pnlChat.SuspendLayout();
        pnlChatInput.SuspendLayout();
        SuspendLayout();

        // ── pnlTopBar ─────────────────────────────────────────────────────────
        pnlTopBar.BackColor = Color.FromArgb(206, 17, 38);
        pnlTopBar.Controls.Add(lblAppTitle);
        pnlTopBar.Controls.Add(lblUsuario);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Size = new Size(1463, 56);
        pnlTopBar.Paint += pnlTopBar_Paint;

        lblAppTitle.Dock = DockStyle.Left;
        lblAppTitle.Font = new Font("Georgia", 14F, FontStyle.Bold | FontStyle.Italic);
        lblAppTitle.ForeColor = Color.FromArgb(240, 185, 11);
        lblAppTitle.Padding = new Padding(14, 0, 0, 0);
        lblAppTitle.Size = new Size(355, 56);
        lblAppTitle.Text = "🃏  LOTERÍA MEXICANA";
        lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;

        lblUsuario.Dock = DockStyle.Right;
        lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsuario.ForeColor = Color.White;
        lblUsuario.Padding = new Padding(0, 0, 16, 0);
        lblUsuario.Size = new Size(297, 56);
        lblUsuario.TextAlign = ContentAlignment.MiddleRight;

        // ── pnlBandaDorada ────────────────────────────────────────────────────
        pnlBandaDorada.BackColor = Color.FromArgb(240, 185, 11);
        pnlBandaDorada.Dock = DockStyle.Top;
        pnlBandaDorada.Size = new Size(1463, 7);

        // ── tblMain ───────────────────────────────────────────────────────────
        tblMain.BackColor = Color.Transparent;
        tblMain.ColumnCount = 2;
        tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 366F));
        tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblMain.Controls.Add(pnlIzquierdo, 0, 0);
        tblMain.Controls.Add(pnlDerecho, 1, 0);
        tblMain.Dock = DockStyle.Fill;
        tblMain.Padding = new Padding(9, 11, 9, 11);
        tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblMain.Size = new Size(1463, 992);

        // ── pnlIzquierdo ──────────────────────────────────────────────────────
        pnlIzquierdo.BackColor = Color.FromArgb(255, 250, 235);
        pnlIzquierdo.Controls.Add(pnlConfigHost);
        pnlIzquierdo.Controls.Add(pnlCartaCard);
        pnlIzquierdo.Controls.Add(chkTts);
        pnlIzquierdo.Controls.Add(lblFichasTxt);
        pnlIzquierdo.Controls.Add(lblFichaHint);
        pnlIzquierdo.Controls.Add(flpFichas);
        pnlIzquierdo.Controls.Add(lblHistorialTxt);
        pnlIzquierdo.Controls.Add(flpHistorial);
        pnlIzquierdo.Dock = DockStyle.Fill;
        pnlIzquierdo.Margin = new Padding(0, 0, 7, 0);
        pnlIzquierdo.Padding = new Padding(16, 19, 16, 19);
        pnlIzquierdo.Size = new Size(359, 970);

        // ── pnlConfigHost  (solo lblSalaInfo + btnIniciar) ────────────────────
        pnlConfigHost.AutoSize = true;
        pnlConfigHost.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        pnlConfigHost.BackColor = Color.White;
        pnlConfigHost.Controls.Add(lblSalaInfo);
        pnlConfigHost.Controls.Add(btnIniciar);
        pnlConfigHost.Location = new Point(16, 19);
        pnlConfigHost.MinimumSize = new Size(327, 0);
        pnlConfigHost.Padding = new Padding(10);

        // lblSalaInfo
        lblSalaInfo.AutoSize = true;
        lblSalaInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblSalaInfo.ForeColor = Color.FromArgb(0, 104, 56);
        lblSalaInfo.Location = new Point(10, 10);
        lblSalaInfo.Margin = new Padding(0, 0, 0, 8);
        lblSalaInfo.Size = new Size(307, 19);

        // btnIniciar
        btnIniciar.BackColor = Color.FromArgb(0, 104, 56);
        btnIniciar.Cursor = Cursors.Hand;
        btnIniciar.FlatAppearance.BorderSize = 0;
        btnIniciar.FlatStyle = FlatStyle.Flat;
        btnIniciar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnIniciar.ForeColor = Color.White;
        btnIniciar.Location = new Point(10, 37);
        btnIniciar.Size = new Size(307, 40);
        btnIniciar.Text = "▶  Iniciar Partida";
        btnIniciar.UseVisualStyleBackColor = false;

        // ── pnlCartaCard ──────────────────────────────────────────────────────
        // Sube para ocupar el espacio que liberó la config eliminada
        pnlCartaCard.BackColor = Color.White;
        pnlCartaCard.Controls.Add(pnlCartaBanda);
        pnlCartaCard.Controls.Add(lblCartaCardTit);
        pnlCartaCard.Controls.Add(picCarta);
        pnlCartaCard.Controls.Add(lblNombreCarta);
        pnlCartaCard.Controls.Add(lblFrase);
        pnlCartaCard.Controls.Add(btnCantarCarta);
        pnlCartaCard.Location = new Point(16, 100);   // ← subió (antes 240)
        pnlCartaCard.Size = new Size(327, 352);

        pnlCartaBanda.BackColor = Color.FromArgb(240, 185, 11);
        pnlCartaBanda.Location = new Point(0, 0);
        pnlCartaBanda.Size = new Size(5, 352);

        lblCartaCardTit.AutoSize = true;
        lblCartaCardTit.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblCartaCardTit.ForeColor = Color.FromArgb(80, 80, 80);
        lblCartaCardTit.Location = new Point(11, 11);
        lblCartaCardTit.Text = "🎴  Carta Actual";

        picCarta.BackColor = Color.White;
        picCarta.Location = new Point(11, 40);
        picCarta.Size = new Size(151, 160);
        picCarta.SizeMode = PictureBoxSizeMode.Zoom;

        lblNombreCarta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblNombreCarta.ForeColor = Color.FromArgb(206, 17, 38);
        lblNombreCarta.Location = new Point(11, 204);
        lblNombreCarta.Size = new Size(304, 32);
        lblNombreCarta.Text = "— Ninguna —";

        lblFrase.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblFrase.ForeColor = Color.FromArgb(100, 100, 100);
        lblFrase.Location = new Point(3, 236);
        lblFrase.Size = new Size(304, 53);
        lblFrase.Click += lblFrase_Click;

        btnCantarCarta.BackColor = Color.FromArgb(206, 17, 38);
        btnCantarCarta.Cursor = Cursors.Hand;
        btnCantarCarta.Enabled = false;
        btnCantarCarta.FlatAppearance.BorderSize = 0;
        btnCantarCarta.FlatStyle = FlatStyle.Flat;
        btnCantarCarta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCantarCarta.ForeColor = Color.White;
        btnCantarCarta.Location = new Point(11, 293);
        btnCantarCarta.Size = new Size(304, 45);
        btnCantarCarta.Text = "🎤  Cantar Carta";
        btnCantarCarta.UseVisualStyleBackColor = false;

        // ── Fichas y controles inferiores  (ajustados al nuevo Y base) ────────
        chkTts.AutoSize = true;
        chkTts.Checked = true;
        chkTts.CheckState = CheckState.Checked;
        chkTts.Font = new Font("Segoe UI", 9F);
        chkTts.ForeColor = Color.FromArgb(200, 200, 200);
        chkTts.Location = new Point(16, 468);   // ← subió (antes 608)
        chkTts.Text = "🔊 Voz activa";

        lblFichasTxt.AutoSize = true;
        lblFichasTxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblFichasTxt.ForeColor = Color.FromArgb(180, 180, 180);
        lblFichasTxt.Location = new Point(16, 496);  // ← subió (antes 636)
        lblFichasTxt.Text = "🎯  Fichas (elige y haz click en casilla):";

        lblFichaHint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblFichaHint.ForeColor = Color.FromArgb(160, 160, 160);
        lblFichaHint.Location = new Point(16, 516);  // ← subió (antes 656)
        lblFichaHint.Size = new Size(327, 20);
        lblFichaHint.Text = "① Elige ficha  ② Click en casilla";
        lblFichaHint.TextAlign = ContentAlignment.MiddleLeft;

        flpFichas.BackColor = Color.White;
        flpFichas.BorderStyle = BorderStyle.FixedSingle;
        flpFichas.Location = new Point(19, 540);     // ← subió (antes 680)
        flpFichas.Padding = new Padding(7, 8, 7, 8);
        flpFichas.Size = new Size(327, 114);
        flpFichas.WrapContents = false;
        flpFichas.Paint += flpFichas_Paint;

        lblHistorialTxt.AutoSize = true;
        lblHistorialTxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHistorialTxt.ForeColor = Color.FromArgb(180, 180, 180);
        lblHistorialTxt.Location = new Point(16, 658); // ← subió (antes 798)
        lblHistorialTxt.Text = "📜  Historial:";

        flpHistorial.AutoScroll = true;
        flpHistorial.BackColor = Color.White;
        flpHistorial.BorderStyle = BorderStyle.FixedSingle;
        flpHistorial.Location = new Point(16, 682);   // ← subió (antes 822)
        flpHistorial.Size = new Size(327, 120);

        // ── pnlDerecho ────────────────────────────────────────────────────────
        pnlDerecho.BackColor = Color.FromArgb(255, 250, 235);
        pnlDerecho.Controls.Add(lblMensaje);
        pnlDerecho.Controls.Add(tblSplit);
        pnlDerecho.Dock = DockStyle.Fill;
        pnlDerecho.Margin = new Padding(7, 0, 0, 0);
        pnlDerecho.Padding = new Padding(16, 19, 16, 19);
        pnlDerecho.Size = new Size(1072, 970);

        lblMensaje.BackColor = Color.Transparent;
        lblMensaje.Dock = DockStyle.Top;
        lblMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblMensaje.ForeColor = Color.FromArgb(100, 100, 100);
        lblMensaje.Size = new Size(1040, 37);
        lblMensaje.TextAlign = ContentAlignment.MiddleCenter;

        tblSplit.BackColor = Color.Transparent;
        tblSplit.ColumnCount = 2;
        tblSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        tblSplit.Controls.Add(pnlGrilla, 0, 0);
        tblSplit.Controls.Add(pnlJugadores, 1, 0);
        tblSplit.Dock = DockStyle.Fill;
        tblSplit.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblSplit.Size = new Size(1040, 932);

        // ── pnlGrilla ─────────────────────────────────────────────────────────
        pnlGrilla.AutoScroll = true;
        pnlGrilla.BackColor = Color.FromArgb(245, 245, 240);
        pnlGrilla.Controls.Add(btnLoteria);
        pnlGrilla.Controls.Add(btnNuevaTabla);
        pnlGrilla.Controls.Add(btnGuardarTabla);
        pnlGrilla.Controls.Add(btnCargarTabla);
        pnlGrilla.Controls.Add(btnReiniciarPartida);
        pnlGrilla.Controls.Add(lblPatrones);
        pnlGrilla.Controls.Add(lblContadorCartas);
        pnlGrilla.Controls.Add(grilla);
        pnlGrilla.Dock = DockStyle.Fill;
        pnlGrilla.Margin = new Padding(3, 4, 3, 4);
        pnlGrilla.Size = new Size(834, 924);

        btnLoteria.BackColor = Color.FromArgb(206, 17, 38);
        btnLoteria.Cursor = Cursors.Hand;
        btnLoteria.Enabled = false;
        btnLoteria.FlatAppearance.BorderSize = 0;
        btnLoteria.FlatStyle = FlatStyle.Flat;
        btnLoteria.Font = new Font("Georgia", 16F, FontStyle.Bold | FontStyle.Italic);
        btnLoteria.ForeColor = Color.White;
        btnLoteria.Location = new Point(-19, 0);
        btnLoteria.Size = new Size(1072, 48);
        btnLoteria.Text = "🏆  ¡LOTERÍA!";
        btnLoteria.UseVisualStyleBackColor = false;

        lblPatrones.AutoSize = false;
        lblPatrones.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblPatrones.ForeColor = Color.FromArgb(255, 190, 50);
        lblPatrones.Location = new Point(3, 52);
        lblPatrones.Size = new Size(830, 22);
        lblPatrones.TextAlign = ContentAlignment.MiddleLeft;

        lblContadorCartas.AutoSize = false;
        lblContadorCartas.Font = new Font("Segoe UI", 8.5F);
        lblContadorCartas.ForeColor = Color.FromArgb(150, 150, 170);
        lblContadorCartas.Location = new Point(3, 74);
        lblContadorCartas.Size = new Size(830, 20);
        lblContadorCartas.Text = "🃏 Cartas: 0/54  (54 restantes)";
        lblContadorCartas.TextAlign = ContentAlignment.MiddleLeft;

        btnNuevaTabla.BackColor = Color.FromArgb(206, 17, 38);
        btnNuevaTabla.Cursor = Cursors.Hand;
        btnNuevaTabla.Enabled = false;
        btnNuevaTabla.FlatAppearance.BorderSize = 0;
        btnNuevaTabla.FlatStyle = FlatStyle.Flat;
        btnNuevaTabla.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnNuevaTabla.ForeColor = Color.White;
        btnNuevaTabla.Location = new Point(3, 98);
        btnNuevaTabla.Size = new Size(130, 38);
        btnNuevaTabla.Text = "🔀  Nueva Tabla";
        btnNuevaTabla.UseVisualStyleBackColor = false;

        btnGuardarTabla.BackColor = Color.FromArgb(0, 104, 56);
        btnGuardarTabla.Cursor = Cursors.Hand;
        btnGuardarTabla.Enabled = false;
        btnGuardarTabla.FlatAppearance.BorderSize = 0;
        btnGuardarTabla.FlatStyle = FlatStyle.Flat;
        btnGuardarTabla.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnGuardarTabla.ForeColor = Color.White;
        btnGuardarTabla.Location = new Point(139, 98);
        btnGuardarTabla.Size = new Size(130, 38);
        btnGuardarTabla.Text = "💾  Guardar Tabla";
        btnGuardarTabla.UseVisualStyleBackColor = false;

        btnCargarTabla.BackColor = Color.FromArgb(80, 50, 120);
        btnCargarTabla.Cursor = Cursors.Hand;
        btnCargarTabla.Enabled = true;
        btnCargarTabla.FlatAppearance.BorderSize = 0;
        btnCargarTabla.FlatStyle = FlatStyle.Flat;
        btnCargarTabla.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnCargarTabla.ForeColor = Color.White;
        btnCargarTabla.Location = new Point(275, 98);
        btnCargarTabla.Size = new Size(130, 38);
        btnCargarTabla.Text = "📂  Cargar Tabla";
        btnCargarTabla.UseVisualStyleBackColor = false;

        btnCrearTabla.BackColor = Color.FromArgb(80, 50, 120);
        btnCrearTabla.Cursor = Cursors.Hand;
        btnCrearTabla.Enabled = true;
        btnCrearTabla.FlatAppearance.BorderSize = 0;
        btnCrearTabla.FlatStyle = FlatStyle.Flat;
        btnCrearTabla.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCrearTabla.ForeColor = Color.White;
        btnCrearTabla.Location = new Point(10, 85);   // debajo de btnIniciar (Y=37+40+8)
        btnCrearTabla.Size = new Size(307, 40);
        btnCrearTabla.Text = "🃏  Crear Tabla Personalizada";
        btnCrearTabla.UseVisualStyleBackColor = false;


        btnReiniciarPartida.BackColor = Color.FromArgb(180, 100, 0);
        btnReiniciarPartida.Cursor = Cursors.Hand;
        btnReiniciarPartida.Enabled = false;
        btnReiniciarPartida.FlatAppearance.BorderSize = 0;
        btnReiniciarPartida.FlatStyle = FlatStyle.Flat;
        btnReiniciarPartida.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnReiniciarPartida.ForeColor = Color.White;
        btnReiniciarPartida.Location = new Point(411, 98);
        btnReiniciarPartida.Size = new Size(150, 38);
        btnReiniciarPartida.Text = "🔄  Reiniciar Partida";
        btnReiniciarPartida.UseVisualStyleBackColor = false;

        grilla.AutoSize = true;
        grilla.BackColor = Color.Transparent;
        grilla.ColumnCount = 5;
        for (int i = 0; i < 5; i++)
            grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.Location = new Point(143, 160);
        grilla.Margin = new Padding(0);
        grilla.RowCount = 4;
        for (int i = 0; i < 4; i++)
            grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
        grilla.Size = new Size(550, 512);

        // ── pnlJugadores ──────────────────────────────────────────────────────
        pnlJugadores.BackColor = Color.White;
        pnlJugadores.Controls.Add(pnlChat);
        pnlJugadores.Controls.Add(lstJugadores);
        pnlJugadores.Controls.Add(pnlSepJug);
        pnlJugadores.Controls.Add(lblConectados);
        pnlJugadores.Controls.Add(lblSalaTitulo);
        pnlJugadores.Dock = DockStyle.Fill;
        pnlJugadores.Margin = new Padding(3, 4, 3, 4);
        pnlJugadores.Padding = new Padding(11, 13, 11, 13);
        pnlJugadores.Size = new Size(194, 924);

        pnlChat.BackColor = Color.White;
        pnlChat.Controls.Add(rtbChat);
        pnlChat.Controls.Add(pnlChatInput);
        pnlChat.Controls.Add(lblChatTitulo);
        pnlChat.Controls.Add(pnlChatBanda);
        pnlChat.Location = new Point(11, 608);
        pnlChat.Size = new Size(172, 290);

        rtbChat.BackColor = Color.White;
        rtbChat.BorderStyle = BorderStyle.None;
        rtbChat.Dock = DockStyle.Fill;
        rtbChat.Font = new Font("Segoe UI", 9F);
        rtbChat.Location = new Point(0, 29);
        rtbChat.ReadOnly = true;
        rtbChat.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbChat.Size = new Size(172, 227);

        pnlChatInput.BackColor = Color.White;
        pnlChatInput.Controls.Add(txtMensaje);
        pnlChatInput.Controls.Add(btnEnviar);
        pnlChatInput.Dock = DockStyle.Bottom;
        pnlChatInput.Location = new Point(0, 256);
        pnlChatInput.Size = new Size(172, 34);

        txtMensaje.BorderStyle = BorderStyle.FixedSingle;
        txtMensaje.Dock = DockStyle.Fill;
        txtMensaje.Font = new Font("Segoe UI", 9F);
        txtMensaje.PlaceholderText = "Mensaje...";
        txtMensaje.Size = new Size(112, 27);

        btnEnviar.BackColor = Color.FromArgb(0, 104, 56);
        btnEnviar.Cursor = Cursors.Hand;
        btnEnviar.Dock = DockStyle.Right;
        btnEnviar.FlatAppearance.BorderSize = 0;
        btnEnviar.FlatStyle = FlatStyle.Flat;
        btnEnviar.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnEnviar.ForeColor = Color.White;
        btnEnviar.Size = new Size(60, 34);
        btnEnviar.Text = "Enviar";
        btnEnviar.UseVisualStyleBackColor = false;

        lblChatTitulo.Dock = DockStyle.Top;
        lblChatTitulo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblChatTitulo.ForeColor = Color.FromArgb(80, 80, 80);
        lblChatTitulo.Padding = new Padding(6, 0, 0, 0);
        lblChatTitulo.Size = new Size(172, 26);
        lblChatTitulo.Text = "💬  Chat";
        lblChatTitulo.TextAlign = ContentAlignment.MiddleLeft;
        lblChatTitulo.Click += lblChatTitulo_Click;

        pnlChatBanda.BackColor = Color.FromArgb(240, 185, 11);
        pnlChatBanda.Dock = DockStyle.Top;
        pnlChatBanda.Size = new Size(172, 3);

        lstJugadores.BackColor = Color.White;
        lstJugadores.BorderStyle = BorderStyle.None;
        lstJugadores.DrawMode = DrawMode.OwnerDrawFixed;
        lstJugadores.Font = new Font("Segoe UI", 9.5F);
        lstJugadores.ItemHeight = 30;
        lstJugadores.Location = new Point(11, 126);
        lstJugadores.Size = new Size(169, 480);
        lstJugadores.DrawItem += lstJugadores_DrawItem;

        pnlSepJug.BackColor = Color.FromArgb(240, 185, 11);
        pnlSepJug.Dock = DockStyle.Top;
        pnlSepJug.Size = new Size(172, 3);

        lblConectados.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblConectados.ForeColor = Color.DimGray;
        lblConectados.Location = new Point(11, 81);
        lblConectados.Size = new Size(114, 19);
        lblConectados.Text = "Conectados: 0";
        lblConectados.TextAlign = ContentAlignment.MiddleLeft;

        lblSalaTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblSalaTitulo.ForeColor = Color.FromArgb(0, 104, 56);
        lblSalaTitulo.Location = new Point(11, 33);
        lblSalaTitulo.Size = new Size(114, 58);
        lblSalaTitulo.Text = "👥  Sala";
        lblSalaTitulo.TextAlign = ContentAlignment.MiddleLeft;

        // ── fantasmas ocultos (compatibilidad) ────────────────────────────────
        bandaHost.Location = new Point(0, 0);
        bandaHost.Size = new Size(0, 0);
        bandaHost.Visible = false;

        lblHostTit.Location = new Point(0, 0);
        lblHostTit.Size = new Size(0, 0);
        lblHostTit.Visible = false;

        // ── FormJuegoRed ──────────────────────────────────────────────────────
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(0, 104, 56);
        ClientSize = new Size(1463, 1055);
        Controls.Add(tblMain);
        Controls.Add(pnlBandaDorada);
        Controls.Add(pnlTopBar);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1255, 851);
        Text = "Lotería Mexicana";
        Load += FormJuegoRed_Load;

        pnlTopBar.ResumeLayout(false);
        tblMain.ResumeLayout(false);
        pnlIzquierdo.ResumeLayout(false);
        pnlIzquierdo.PerformLayout();
        pnlConfigHost.ResumeLayout(false);
        pnlConfigHost.PerformLayout();
        pnlCartaCard.ResumeLayout(false);
        pnlCartaCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picCarta).EndInit();
        pnlDerecho.ResumeLayout(false);
        tblSplit.ResumeLayout(false);
        pnlGrilla.ResumeLayout(false);
        pnlGrilla.PerformLayout();
        pnlJugadores.ResumeLayout(false);
        pnlChat.ResumeLayout(false);
        pnlChatInput.ResumeLayout(false);
        pnlChatInput.PerformLayout();
        ResumeLayout(false);
    }



    // ── Declaraciones de campo ────────────────────────────────────────────────
    private Panel pnlTopBar;
    private Label lblAppTitle;
    private Label lblUsuario;
    private Panel pnlBandaDorada;
    private TableLayoutPanel tblMain;
    private Panel pnlIzquierdo;
    private Panel pnlConfigHost;
    private Label lblSalaInfo;
    private Button btnIniciar;
    private Panel pnlCartaCard;
    private Panel pnlCartaBanda;
    private Label lblCartaCardTit;
    private PictureBox picCarta;
    private Label lblNombreCarta;
    private Label lblFrase;
    private Button btnCantarCarta;
    private CheckBox chkTts;
    private Label lblFichasTxt;
    private Label lblFichaHint;
    private FlowLayoutPanel flpFichas;
    private Label lblHistorialTxt;
    private FlowLayoutPanel flpHistorial;
    private Panel pnlDerecho;
    private Label lblMensaje;
    private TableLayoutPanel tblSplit;
    private Panel pnlGrilla;
    private TableLayoutPanel grilla;
    private Button btnLoteria;
    private Button btnNuevaTabla;
    private Button btnGuardarTabla;
    private Button btnCargarTabla;
    private Button btnCrearTabla;
    private Button btnReiniciarPartida;
    private Label lblPatrones;
    private Label lblContadorCartas;
    private Panel pnlJugadores;
    private Label lblSalaTitulo;
    private Label lblConectados;
    private Panel pnlSepJug;
    private ListBox lstJugadores;
    private Panel pnlChat;
    private RichTextBox rtbChat;
    private Panel pnlChatInput;
    private TextBox txtMensaje;
    private Button btnEnviar;
    private Label lblChatTitulo;
    private Panel pnlChatBanda;
    private Panel bandaHost;
    private Label lblHostTit;
}