namespace LoteriaMexicana.Forms;

partial class FormJuegoRed
{
    /// <summary>Variable del diseñador requerida.</summary>
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Código generado por el Diseñador de Windows Forms

    private void InitializeComponent()
    {
        pnlTopBar = new Panel();
        lblAppTitle = new Label();
        lblUsuario = new Label();
        pnlBandaDorada = new Panel();
        tblMain = new TableLayoutPanel();
        pnlIzquierdo = new Panel();
        pnlConfigHost = new Panel();
        lblSalaInfo = new Label();
        lblFormatoTxt = new Label();
        cmbFormato = new ComboBox();
        btnIniciar = new Button();
        bandaHost = new Panel();
        lblHostTit = new Label();
        pnlCartaCard = new Panel();
        pnlCartaBanda = new Panel();
        lblCartaCardTit = new Label();
        picCarta = new PictureBox();
        lblNombreCarta = new Label();
        lblFrase = new Label();
        btnCantarCarta = new Button();
        chkTts = new CheckBox();
        lblFichasTxt = new Label();
        flpFichas = new FlowLayoutPanel();
        lblHistorialTxt = new Label();
        flpHistorial = new FlowLayoutPanel();
        pnlDerecho = new Panel();
        lblMensaje = new Label();
        tblSplit = new TableLayoutPanel();
        pnlGrilla = new Panel();
        btnLoteria = new Button();
        btnNuevaTabla = new Button();
        grilla = new TableLayoutPanel();
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
        pnlTopBar.SuspendLayout();
        tblMain.SuspendLayout();
        pnlIzquierdo.SuspendLayout();
        pnlConfigHost.SuspendLayout();
        pnlCartaCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picCarta).BeginInit();
        pnlDerecho.SuspendLayout();
        tblSplit.SuspendLayout();
        pnlGrilla.SuspendLayout();
        pnlJugadores.SuspendLayout();
        pnlChat.SuspendLayout();
        pnlChatInput.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTopBar
        // 
        pnlTopBar.BackColor = Color.FromArgb(206, 17, 38);
        pnlTopBar.Controls.Add(lblAppTitle);
        pnlTopBar.Controls.Add(lblUsuario);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Margin = new Padding(3, 4, 3, 4);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1463, 56);
        pnlTopBar.TabIndex = 2;
        pnlTopBar.Paint += pnlTopBar_Paint;
        // 
        // lblAppTitle
        // 
        lblAppTitle.Dock = DockStyle.Left;
        lblAppTitle.Font = new Font("Georgia", 14F, FontStyle.Bold | FontStyle.Italic);
        lblAppTitle.ForeColor = Color.FromArgb(240, 185, 11);
        lblAppTitle.Location = new Point(0, 0);
        lblAppTitle.Name = "lblAppTitle";
        lblAppTitle.Padding = new Padding(14, 0, 0, 0);
        lblAppTitle.Size = new Size(355, 56);
        lblAppTitle.TabIndex = 0;
        lblAppTitle.Text = "🃏  LOTERÍA MEXICANA";
        lblAppTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblUsuario
        // 
        lblUsuario.Dock = DockStyle.Right;
        lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsuario.ForeColor = Color.White;
        lblUsuario.Location = new Point(1166, 0);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Padding = new Padding(0, 0, 16, 0);
        lblUsuario.Size = new Size(297, 56);
        lblUsuario.TabIndex = 1;
        lblUsuario.TextAlign = ContentAlignment.MiddleRight;
        // 
        // pnlBandaDorada
        // 
        pnlBandaDorada.BackColor = Color.FromArgb(240, 185, 11);
        pnlBandaDorada.Dock = DockStyle.Top;
        pnlBandaDorada.Location = new Point(0, 56);
        pnlBandaDorada.Margin = new Padding(3, 4, 3, 4);
        pnlBandaDorada.Name = "pnlBandaDorada";
        pnlBandaDorada.Size = new Size(1463, 7);
        pnlBandaDorada.TabIndex = 1;
        // 
        // tblMain
        // 
        tblMain.BackColor = Color.Transparent;
        tblMain.ColumnCount = 2;
        tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 366F));
        tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblMain.Controls.Add(pnlIzquierdo, 0, 0);
        tblMain.Controls.Add(pnlDerecho, 1, 0);
        tblMain.Dock = DockStyle.Fill;
        tblMain.Location = new Point(0, 63);
        tblMain.Margin = new Padding(3, 4, 3, 4);
        tblMain.Name = "tblMain";
        tblMain.Padding = new Padding(9, 11, 9, 11);
        tblMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tblMain.Size = new Size(1463, 992);
        tblMain.TabIndex = 0;
        // 
        // pnlIzquierdo
        // 
        pnlIzquierdo.BackColor = Color.FromArgb(255, 250, 235);
        pnlIzquierdo.Controls.Add(pnlConfigHost);
        pnlIzquierdo.Controls.Add(pnlCartaCard);
        pnlIzquierdo.Controls.Add(chkTts);
        pnlIzquierdo.Controls.Add(lblFichasTxt);
        pnlIzquierdo.Controls.Add(flpFichas);
        pnlIzquierdo.Controls.Add(lblHistorialTxt);
        pnlIzquierdo.Controls.Add(flpHistorial);
        pnlIzquierdo.Dock = DockStyle.Fill;
        pnlIzquierdo.Location = new Point(9, 11);
        pnlIzquierdo.Margin = new Padding(0, 0, 7, 0);
        pnlIzquierdo.Name = "pnlIzquierdo";
        pnlIzquierdo.Padding = new Padding(16, 19, 16, 19);
        pnlIzquierdo.Size = new Size(359, 970);
        pnlIzquierdo.TabIndex = 0;
        // 
        // pnlConfigHost
        // 
        pnlConfigHost.BackColor = Color.White;
        pnlConfigHost.Controls.Add(lblSalaInfo);
        pnlConfigHost.Controls.Add(lblFormatoTxt);
        pnlConfigHost.Controls.Add(cmbFormato);
        pnlConfigHost.Controls.Add(btnIniciar);
        pnlConfigHost.Controls.Add(bandaHost);
        pnlConfigHost.Controls.Add(lblHostTit);
        pnlConfigHost.Location = new Point(16, 19);
        pnlConfigHost.Margin = new Padding(3, 4, 3, 4);
        pnlConfigHost.Name = "pnlConfigHost";
        pnlConfigHost.Size = new Size(327, 181);
        pnlConfigHost.TabIndex = 0;
        // 
        // lblSalaInfo
        // 
        lblSalaInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblSalaInfo.ForeColor = Color.FromArgb(0, 104, 56);
        lblSalaInfo.Location = new Point(11, 40);
        lblSalaInfo.Name = "lblSalaInfo";
        lblSalaInfo.Size = new Size(304, 27);
        lblSalaInfo.TabIndex = 0;
        // 
        // lblFormatoTxt
        // 
        lblFormatoTxt.AutoSize = true;
        lblFormatoTxt.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblFormatoTxt.ForeColor = Color.FromArgb(80, 80, 80);
        lblFormatoTxt.Location = new Point(11, 72);
        lblFormatoTxt.Name = "lblFormatoTxt";
        lblFormatoTxt.Size = new Size(73, 20);
        lblFormatoTxt.TabIndex = 1;
        lblFormatoTxt.Text = "Formato:";
        // 
        // cmbFormato
        // 
        cmbFormato.BackColor = Color.White;
        cmbFormato.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbFormato.Font = new Font("Segoe UI", 9F);
        cmbFormato.Location = new Point(11, 96);
        cmbFormato.Margin = new Padding(3, 4, 3, 4);
        cmbFormato.Name = "cmbFormato";
        cmbFormato.Size = new Size(228, 28);
        cmbFormato.TabIndex = 0;
        // 
        // btnIniciar
        // 
        btnIniciar.BackColor = Color.FromArgb(0, 104, 56);
        btnIniciar.Cursor = Cursors.Hand;
        btnIniciar.FlatAppearance.BorderSize = 0;
        btnIniciar.FlatStyle = FlatStyle.Flat;
        btnIniciar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnIniciar.ForeColor = Color.White;
        btnIniciar.Location = new Point(11, 136);
        btnIniciar.Margin = new Padding(3, 4, 3, 4);
        btnIniciar.Name = "btnIniciar";
        btnIniciar.Size = new Size(304, 40);
        btnIniciar.TabIndex = 1;
        btnIniciar.Text = "▶  Iniciar Partida";
        btnIniciar.UseVisualStyleBackColor = false;
        // 
        // bandaHost
        // 
        bandaHost.Location = new Point(0, 0);
        bandaHost.Margin = new Padding(3, 4, 3, 4);
        bandaHost.Name = "bandaHost";
        bandaHost.Size = new Size(229, 133);
        bandaHost.TabIndex = 2;
        // 
        // lblHostTit
        // 
        lblHostTit.Location = new Point(0, 0);
        lblHostTit.Name = "lblHostTit";
        lblHostTit.Size = new Size(114, 31);
        lblHostTit.TabIndex = 3;
        // 
        // pnlCartaCard
        // 
        pnlCartaCard.BackColor = Color.White;
        pnlCartaCard.Controls.Add(pnlCartaBanda);
        pnlCartaCard.Controls.Add(lblCartaCardTit);
        pnlCartaCard.Controls.Add(picCarta);
        pnlCartaCard.Controls.Add(lblNombreCarta);
        pnlCartaCard.Controls.Add(lblFrase);
        pnlCartaCard.Controls.Add(btnCantarCarta);
        pnlCartaCard.Location = new Point(16, 213);
        pnlCartaCard.Margin = new Padding(3, 4, 3, 4);
        pnlCartaCard.Name = "pnlCartaCard";
        pnlCartaCard.Size = new Size(327, 352);
        pnlCartaCard.TabIndex = 1;
        // 
        // pnlCartaBanda
        // 
        pnlCartaBanda.BackColor = Color.FromArgb(240, 185, 11);
        pnlCartaBanda.Location = new Point(0, 0);
        pnlCartaBanda.Margin = new Padding(3, 4, 3, 4);
        pnlCartaBanda.Name = "pnlCartaBanda";
        pnlCartaBanda.Size = new Size(5, 440);
        pnlCartaBanda.TabIndex = 0;
        // 
        // lblCartaCardTit
        // 
        lblCartaCardTit.AutoSize = true;
        lblCartaCardTit.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblCartaCardTit.ForeColor = Color.FromArgb(80, 80, 80);
        lblCartaCardTit.Location = new Point(11, 11);
        lblCartaCardTit.Name = "lblCartaCardTit";
        lblCartaCardTit.Size = new Size(125, 20);
        lblCartaCardTit.TabIndex = 1;
        lblCartaCardTit.Text = "🎴  Carta Actual";
        // 
        // picCarta
        // 
        picCarta.BackColor = Color.White;
        picCarta.Location = new Point(11, 40);
        picCarta.Margin = new Padding(3, 4, 3, 4);
        picCarta.Name = "picCarta";
        picCarta.Size = new Size(151, 160);
        picCarta.SizeMode = PictureBoxSizeMode.Zoom;
        picCarta.TabIndex = 2;
        picCarta.TabStop = false;
        // 
        // lblNombreCarta
        // 
        lblNombreCarta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblNombreCarta.ForeColor = Color.FromArgb(206, 17, 38);
        lblNombreCarta.Location = new Point(11, 204);
        lblNombreCarta.Name = "lblNombreCarta";
        lblNombreCarta.Size = new Size(304, 32);
        lblNombreCarta.TabIndex = 3;
        lblNombreCarta.Text = "— Ninguna —";
        // 
        // lblFrase
        // 
        lblFrase.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblFrase.ForeColor = Color.FromArgb(100, 100, 100);
        lblFrase.Location = new Point(3, 236);
        lblFrase.Name = "lblFrase";
        lblFrase.Size = new Size(304, 53);
        lblFrase.TabIndex = 4;
        lblFrase.Click += lblFrase_Click;
        // 
        // btnCantarCarta
        // 
        btnCantarCarta.BackColor = Color.FromArgb(206, 17, 38);
        btnCantarCarta.Cursor = Cursors.Hand;
        btnCantarCarta.Enabled = false;
        btnCantarCarta.FlatAppearance.BorderSize = 0;
        btnCantarCarta.FlatStyle = FlatStyle.Flat;
        btnCantarCarta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCantarCarta.ForeColor = Color.White;
        btnCantarCarta.Location = new Point(11, 293);
        btnCantarCarta.Margin = new Padding(3, 4, 3, 4);
        btnCantarCarta.Name = "btnCantarCarta";
        btnCantarCarta.Size = new Size(304, 45);
        btnCantarCarta.TabIndex = 2;
        btnCantarCarta.Text = "🎤  Cantar Carta";
        btnCantarCarta.UseVisualStyleBackColor = false;
        // 
        // chkTts
        // 
        chkTts.AutoSize = true;
        chkTts.Checked = true;
        chkTts.CheckState = CheckState.Checked;
        chkTts.Font = new Font("Segoe UI", 9F);
        chkTts.ForeColor = Color.FromArgb(200, 200, 200);
        chkTts.Location = new Point(16, 573);
        chkTts.Margin = new Padding(3, 4, 3, 4);
        chkTts.Name = "chkTts";
        chkTts.Size = new Size(123, 24);
        chkTts.TabIndex = 3;
        chkTts.Text = "🔊 Voz activa";
        // 
        // lblFichasTxt
        // 
        lblFichasTxt.AutoSize = true;
        lblFichasTxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblFichasTxt.ForeColor = Color.FromArgb(180, 180, 180);
        lblFichasTxt.Location = new Point(16, 601);
        lblFichasTxt.Name = "lblFichasTxt";
        lblFichasTxt.Size = new Size(224, 20);
        lblFichasTxt.TabIndex = 4;
        lblFichasTxt.Text = "🎯  Fichas (arrastra a la tabla):";
        // 
        // flpFichas
        // 
        flpFichas.BackColor = Color.White;
        flpFichas.BorderStyle = BorderStyle.FixedSingle;
        flpFichas.Location = new Point(19, 625);
        flpFichas.Margin = new Padding(3, 4, 3, 4);
        flpFichas.Name = "flpFichas";
        flpFichas.Padding = new Padding(7, 8, 7, 8);
        flpFichas.Size = new Size(327, 114);
        flpFichas.TabIndex = 5;
        flpFichas.WrapContents = false;
        flpFichas.Paint += flpFichas_Paint;
        // 
        // lblHistorialTxt
        // 
        lblHistorialTxt.AutoSize = true;
        lblHistorialTxt.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblHistorialTxt.ForeColor = Color.FromArgb(180, 180, 180);
        lblHistorialTxt.Location = new Point(16, 743);
        lblHistorialTxt.Name = "lblHistorialTxt";
        lblHistorialTxt.Size = new Size(102, 20);
        lblHistorialTxt.TabIndex = 6;
        lblHistorialTxt.Text = "📜  Historial:";
        // 
        // flpHistorial
        // 
        flpHistorial.AutoScroll = true;
        flpHistorial.BackColor = Color.White;
        flpHistorial.BorderStyle = BorderStyle.FixedSingle;
        flpHistorial.Location = new Point(16, 767);
        flpHistorial.Margin = new Padding(3, 4, 3, 4);
        flpHistorial.Name = "flpHistorial";
        flpHistorial.Size = new Size(327, 173);
        flpHistorial.TabIndex = 7;
        // 
        // pnlDerecho
        // 
        pnlDerecho.BackColor = Color.FromArgb(255, 250, 235);
        pnlDerecho.Controls.Add(lblMensaje);
        pnlDerecho.Controls.Add(tblSplit);
        pnlDerecho.Dock = DockStyle.Fill;
        pnlDerecho.Location = new Point(382, 11);
        pnlDerecho.Margin = new Padding(7, 0, 0, 0);
        pnlDerecho.Name = "pnlDerecho";
        pnlDerecho.Padding = new Padding(16, 19, 16, 19);
        pnlDerecho.Size = new Size(1072, 970);
        pnlDerecho.TabIndex = 1;
        // 
        // lblMensaje
        // 
        lblMensaje.BackColor = Color.Transparent;
        lblMensaje.Dock = DockStyle.Top;
        lblMensaje.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblMensaje.ForeColor = Color.FromArgb(100, 100, 100);
        lblMensaje.Location = new Point(16, 19);
        lblMensaje.Name = "lblMensaje";
        lblMensaje.Size = new Size(1040, 37);
        lblMensaje.TabIndex = 11;
        lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // tblSplit
        // 
        tblSplit.BackColor = Color.Transparent;
        tblSplit.ColumnCount = 2;
        tblSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblSplit.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        tblSplit.Controls.Add(pnlGrilla, 0, 0);
        tblSplit.Controls.Add(pnlJugadores, 1, 0);
        tblSplit.Dock = DockStyle.Fill;
        tblSplit.Location = new Point(16, 19);
        tblSplit.Margin = new Padding(3, 4, 3, 4);
        tblSplit.Name = "tblSplit";
        tblSplit.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        tblSplit.Size = new Size(1040, 932);
        tblSplit.TabIndex = 12;
        // 
        // pnlGrilla
        // 
        pnlGrilla.AutoScroll = true;
        pnlGrilla.BackColor = Color.FromArgb(245, 245, 240);
        pnlGrilla.Controls.Add(btnLoteria);
        pnlGrilla.Controls.Add(btnNuevaTabla);
        pnlGrilla.Controls.Add(grilla);
        pnlGrilla.Dock = DockStyle.Fill;
        pnlGrilla.Location = new Point(3, 4);
        pnlGrilla.Margin = new Padding(3, 4, 3, 4);
        pnlGrilla.Name = "pnlGrilla";
        pnlGrilla.Size = new Size(834, 924);
        pnlGrilla.TabIndex = 0;
        // 
        // btnLoteria
        // 
        btnLoteria.BackColor = Color.FromArgb(206, 17, 38);
        btnLoteria.Cursor = Cursors.Hand;
        btnLoteria.Enabled = false;
        btnLoteria.FlatAppearance.BorderSize = 0;
        btnLoteria.FlatStyle = FlatStyle.Flat;
        btnLoteria.Font = new Font("Georgia", 16F, FontStyle.Bold | FontStyle.Italic);
        btnLoteria.ForeColor = Color.White;
        btnLoteria.Location = new Point(-19, 32);
        btnLoteria.Margin = new Padding(3, 4, 3, 4);
        btnLoteria.Name = "btnLoteria";
        btnLoteria.Size = new Size(1072, 56);
        btnLoteria.TabIndex = 10;
        btnLoteria.Text = "🏆  ¡LOTERÍA!";
        btnLoteria.UseVisualStyleBackColor = false;
        // 
        // btnNuevaTabla
        // 
        btnNuevaTabla.BackColor = Color.FromArgb(206, 17, 38);
        btnNuevaTabla.Cursor = Cursors.Hand;
        btnNuevaTabla.Enabled = false;
        btnNuevaTabla.FlatAppearance.BorderSize = 0;
        btnNuevaTabla.FlatStyle = FlatStyle.Flat;
        btnNuevaTabla.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnNuevaTabla.ForeColor = Color.White;
        btnNuevaTabla.Location = new Point(3, 183);
        btnNuevaTabla.Name = "btnNuevaTabla";
        btnNuevaTabla.Size = new Size(169, 38);
        btnNuevaTabla.TabIndex = 11;
        btnNuevaTabla.Text = "🔀  Nueva Tabla";
        btnNuevaTabla.UseVisualStyleBackColor = false;
        // 
        // grilla
        // 
        grilla.AutoSize = true;
        grilla.BackColor = Color.Transparent;
        grilla.ColumnCount = 5;
        grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
        grilla.Location = new Point(143, 248);
        grilla.Margin = new Padding(0);
        grilla.Name = "grilla";
        grilla.RowCount = 4;
        grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
        grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
        grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
        grilla.RowStyles.Add(new RowStyle(SizeType.Absolute, 128F));
        grilla.Size = new Size(550, 512);
        grilla.TabIndex = 0;
        // 
        // pnlJugadores
        // 
        pnlJugadores.BackColor = Color.White;
        pnlJugadores.Controls.Add(pnlChat);
        pnlJugadores.Controls.Add(lstJugadores);
        pnlJugadores.Controls.Add(pnlSepJug);
        pnlJugadores.Controls.Add(lblConectados);
        pnlJugadores.Controls.Add(lblSalaTitulo);
        pnlJugadores.Dock = DockStyle.Fill;
        pnlJugadores.Location = new Point(843, 4);
        pnlJugadores.Margin = new Padding(3, 4, 3, 4);
        pnlJugadores.Name = "pnlJugadores";
        pnlJugadores.Padding = new Padding(11, 13, 11, 13);
        pnlJugadores.Size = new Size(194, 924);
        pnlJugadores.TabIndex = 1;
        // 
        // pnlChat
        // 
        pnlChat.BackColor = Color.White;
        pnlChat.Controls.Add(rtbChat);
        pnlChat.Controls.Add(pnlChatInput);
        pnlChat.Controls.Add(lblChatTitulo);
        pnlChat.Controls.Add(pnlChatBanda);
        pnlChat.Location = new Point(11, 608);
        pnlChat.Name = "pnlChat";
        pnlChat.Size = new Size(172, 290);
        pnlChat.TabIndex = 10;
        // 
        // rtbChat
        // 
        rtbChat.BackColor = Color.White;
        rtbChat.BorderStyle = BorderStyle.None;
        rtbChat.Dock = DockStyle.Fill;
        rtbChat.Font = new Font("Segoe UI", 9F);
        rtbChat.Location = new Point(0, 29);
        rtbChat.Name = "rtbChat";
        rtbChat.ReadOnly = true;
        rtbChat.ScrollBars = RichTextBoxScrollBars.Vertical;
        rtbChat.Size = new Size(172, 227);
        rtbChat.TabIndex = 0;
        rtbChat.Text = "";
        // 
        // pnlChatInput
        // 
        pnlChatInput.BackColor = Color.White;
        pnlChatInput.Controls.Add(txtMensaje);
        pnlChatInput.Controls.Add(btnEnviar);
        pnlChatInput.Dock = DockStyle.Bottom;
        pnlChatInput.Location = new Point(0, 256);
        pnlChatInput.Name = "pnlChatInput";
        pnlChatInput.Size = new Size(172, 34);
        pnlChatInput.TabIndex = 1;
        // 
        // txtMensaje
        // 
        txtMensaje.BorderStyle = BorderStyle.FixedSingle;
        txtMensaje.Dock = DockStyle.Fill;
        txtMensaje.Font = new Font("Segoe UI", 9F);
        txtMensaje.Location = new Point(0, 0);
        txtMensaje.Name = "txtMensaje";
        txtMensaje.PlaceholderText = "Mensaje...";
        txtMensaje.Size = new Size(112, 27);
        txtMensaje.TabIndex = 0;
        // 
        // btnEnviar
        // 
        btnEnviar.BackColor = Color.FromArgb(0, 104, 56);
        btnEnviar.Cursor = Cursors.Hand;
        btnEnviar.Dock = DockStyle.Right;
        btnEnviar.FlatAppearance.BorderSize = 0;
        btnEnviar.FlatStyle = FlatStyle.Flat;
        btnEnviar.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnEnviar.ForeColor = Color.White;
        btnEnviar.Location = new Point(112, 0);
        btnEnviar.Name = "btnEnviar";
        btnEnviar.Size = new Size(60, 34);
        btnEnviar.TabIndex = 1;
        btnEnviar.Text = "Enviar";
        btnEnviar.UseVisualStyleBackColor = false;
        // 
        // lblChatTitulo
        // 
        lblChatTitulo.Dock = DockStyle.Top;
        lblChatTitulo.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        lblChatTitulo.ForeColor = Color.FromArgb(80, 80, 80);
        lblChatTitulo.Location = new Point(0, 3);
        lblChatTitulo.Name = "lblChatTitulo";
        lblChatTitulo.Padding = new Padding(6, 0, 0, 0);
        lblChatTitulo.Size = new Size(172, 26);
        lblChatTitulo.TabIndex = 2;
        lblChatTitulo.Text = "💬  Chat";
        lblChatTitulo.TextAlign = ContentAlignment.MiddleLeft;
        lblChatTitulo.Click += lblChatTitulo_Click;
        // 
        // pnlChatBanda
        // 
        pnlChatBanda.BackColor = Color.FromArgb(240, 185, 11);
        pnlChatBanda.Dock = DockStyle.Top;
        pnlChatBanda.Location = new Point(0, 0);
        pnlChatBanda.Name = "pnlChatBanda";
        pnlChatBanda.Size = new Size(172, 3);
        pnlChatBanda.TabIndex = 3;
        // 
        // lstJugadores
        // 
        lstJugadores.BackColor = Color.White;
        lstJugadores.BorderStyle = BorderStyle.None;
        lstJugadores.DrawMode = DrawMode.OwnerDrawFixed;
        lstJugadores.Font = new Font("Segoe UI", 9.5F);
        lstJugadores.ItemHeight = 30;
        lstJugadores.Location = new Point(11, 126);
        lstJugadores.Margin = new Padding(3, 4, 3, 4);
        lstJugadores.Name = "lstJugadores";
        lstJugadores.Size = new Size(169, 480);
        lstJugadores.TabIndex = 0;
        lstJugadores.DrawItem += lstJugadores_DrawItem;
        // 
        // pnlSepJug
        // 
        pnlSepJug.BackColor = Color.FromArgb(240, 185, 11);
        pnlSepJug.Dock = DockStyle.Top;
        pnlSepJug.Location = new Point(11, 13);
        pnlSepJug.Margin = new Padding(0, 5, 0, 8);
        pnlSepJug.Name = "pnlSepJug";
        pnlSepJug.Size = new Size(172, 3);
        pnlSepJug.TabIndex = 1;
        // 
        // lblConectados
        // 
        lblConectados.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        lblConectados.ForeColor = Color.DimGray;
        lblConectados.Location = new Point(11, 81);
        lblConectados.Name = "lblConectados";
        lblConectados.Size = new Size(114, 19);
        lblConectados.TabIndex = 2;
        lblConectados.Text = "Conectados: 0";
        lblConectados.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblSalaTitulo
        // 
        lblSalaTitulo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblSalaTitulo.ForeColor = Color.FromArgb(0, 104, 56);
        lblSalaTitulo.Location = new Point(11, 33);
        lblSalaTitulo.Name = "lblSalaTitulo";
        lblSalaTitulo.Size = new Size(114, 58);
        lblSalaTitulo.TabIndex = 3;
        lblSalaTitulo.Text = "👥  Sala";
        lblSalaTitulo.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // FormJuegoRed
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(0, 104, 56);
        ClientSize = new Size(1463, 1055);
        Controls.Add(tblMain);
        Controls.Add(pnlBandaDorada);
        Controls.Add(pnlTopBar);
        Font = new Font("Segoe UI", 9F);
        Margin = new Padding(3, 4, 3, 4);
        MinimumSize = new Size(1255, 851);
        Name = "FormJuegoRed";
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

    #endregion

    // ── Declaración de todos los controles ────────────────────────────────────
    private System.Windows.Forms.Panel            pnlTopBar;
    private System.Windows.Forms.Label            lblAppTitle;
    private System.Windows.Forms.Label            lblUsuario;
    private System.Windows.Forms.Panel            pnlBandaDorada;
    private System.Windows.Forms.TableLayoutPanel tblMain;
    private System.Windows.Forms.Panel            pnlIzquierdo;
    private System.Windows.Forms.Panel            pnlConfigHost;
    private System.Windows.Forms.Label            lblSalaInfo;
    private System.Windows.Forms.Label            lblFormatoTxt;
    private System.Windows.Forms.ComboBox         cmbFormato;
    private System.Windows.Forms.Button           btnIniciar;
    private System.Windows.Forms.Panel            pnlCartaCard;
    private System.Windows.Forms.Label            lblCartaCardTit;
    private System.Windows.Forms.Panel            pnlCartaBanda;
    private System.Windows.Forms.PictureBox       picCarta;
    private System.Windows.Forms.Label            lblNombreCarta;
    private System.Windows.Forms.Label            lblFrase;
    private System.Windows.Forms.Button           btnCantarCarta;
    private System.Windows.Forms.CheckBox         chkTts;
    private System.Windows.Forms.Label            lblFichasTxt;
    private System.Windows.Forms.FlowLayoutPanel  flpFichas;
    private System.Windows.Forms.Label            lblHistorialTxt;
    private System.Windows.Forms.FlowLayoutPanel  flpHistorial;
    private System.Windows.Forms.Panel            pnlDerecho;
    private System.Windows.Forms.Button           btnLoteria;
    private System.Windows.Forms.Label            lblMensaje;
    private System.Windows.Forms.TableLayoutPanel tblSplit;
    private System.Windows.Forms.Panel            pnlGrilla;
    private System.Windows.Forms.TableLayoutPanel grilla;
    private System.Windows.Forms.Panel            pnlJugadores;
    private System.Windows.Forms.Label            lblSalaTitulo;
    private System.Windows.Forms.Label            lblConectados;
    private System.Windows.Forms.Panel            pnlSepJug;
    private System.Windows.Forms.ListBox          lstJugadores;
    private System.Windows.Forms.Button           btnNuevaTabla;
    private Panel bandaHost;
    private Label lblHostTit;
    private System.Windows.Forms.Panel            pnlChat;
    private System.Windows.Forms.RichTextBox      rtbChat;
    private System.Windows.Forms.Panel            pnlChatInput;
    private System.Windows.Forms.TextBox          txtMensaje;
    private System.Windows.Forms.Button           btnEnviar;
    private System.Windows.Forms.Label            lblChatTitulo;
    private System.Windows.Forms.Panel            pnlChatBanda;
}
