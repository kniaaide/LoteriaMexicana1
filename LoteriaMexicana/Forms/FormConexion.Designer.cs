namespace LoteriaMexicana.Forms;

partial class FormConexion
{
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
        this.components     = new System.ComponentModel.Container();
        this.pnlHeader      = new System.Windows.Forms.Panel();
        this.pnlBandaDorada = new System.Windows.Forms.Panel();
        this.lblTitulo      = new System.Windows.Forms.Label();
        this.lblSubtitulo   = new System.Windows.Forms.Label();
        this.pnlContenido   = new System.Windows.Forms.Panel();
        this.lblNombreTxt   = new System.Windows.Forms.Label();
        this.txtNombre      = new System.Windows.Forms.TextBox();
        this.btnCrear       = new System.Windows.Forms.Button();
        this.lblSeparador   = new System.Windows.Forms.Label();
        this.lblIpTxt       = new System.Windows.Forms.Label();
        this.txtIp          = new System.Windows.Forms.TextBox();
        this.btnUnirse      = new System.Windows.Forms.Button();
        this._lblIpLocal    = new System.Windows.Forms.Label();
        this.lblEstado      = new System.Windows.Forms.Label();

        this.pnlHeader.SuspendLayout();
        this.pnlContenido.SuspendLayout();
        this.SuspendLayout();

        // ── pnlHeader ─────────────────────────────────────────────────────────
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(206, 17, 38);
        this.pnlHeader.Dock     = System.Windows.Forms.DockStyle.Top;
        this.pnlHeader.Height   = 110;
        this.pnlHeader.Controls.AddRange(new System.Windows.Forms.Control[]
            { this.lblTitulo, this.lblSubtitulo });

        // ── lblTitulo ─────────────────────────────────────────────────────────
        this.lblTitulo.Dock      = System.Windows.Forms.DockStyle.Fill;
        this.lblTitulo.Text      = "¡LOTERÍA!";
        this.lblTitulo.Font      = new System.Drawing.Font("Georgia", 32F,
            System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic);
        this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(240, 185, 11);
        this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // ── lblSubtitulo ──────────────────────────────────────────────────────
        this.lblSubtitulo.Dock      = System.Windows.Forms.DockStyle.Bottom;
        this.lblSubtitulo.Height    = 28;
        this.lblSubtitulo.Text      = "Mexicana";
        this.lblSubtitulo.Font      = new System.Drawing.Font("Segoe UI", 13F,
            System.Drawing.FontStyle.Italic);
        this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(255, 220, 180);
        this.lblSubtitulo.TextAlign = System.Drawing.ContentAlignment.TopCenter;

        // ── pnlBandaDorada ────────────────────────────────────────────────────
        this.pnlBandaDorada.BackColor = System.Drawing.Color.FromArgb(240, 185, 11);
        this.pnlBandaDorada.Dock      = System.Windows.Forms.DockStyle.Top;
        this.pnlBandaDorada.Height    = 6;

        // ── pnlContenido ──────────────────────────────────────────────────────
        // Sin Padding — los controles usan Location absoluta dentro del panel
        this.pnlContenido.BackColor = System.Drawing.Color.FromArgb(255, 250, 235);
        this.pnlContenido.Dock      = System.Windows.Forms.DockStyle.Fill;
        this.pnlContenido.Controls.AddRange(new System.Windows.Forms.Control[]
        {
            this.lblNombreTxt,
            this.txtNombre,
            this.btnCrear,
            this.lblSeparador,
            this.lblIpTxt,
            this.txtIp,
            this.btnUnirse,
            this._lblIpLocal,
            this.lblEstado
        });

        // ── lblNombreTxt ──────────────────────────────────────────────────────
        this.lblNombreTxt.AutoSize  = false;
        this.lblNombreTxt.Location  = new System.Drawing.Point(36, 20);
        this.lblNombreTxt.Size      = new System.Drawing.Size(380, 22);
        this.lblNombreTxt.Text      = "Tu nombre:";
        this.lblNombreTxt.Font      = new System.Drawing.Font("Segoe UI", 9.5F,
            System.Drawing.FontStyle.Bold);
        this.lblNombreTxt.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

        // ── txtNombre ─────────────────────────────────────────────────────────
        this.txtNombre.Location        = new System.Drawing.Point(36, 46);
        this.txtNombre.Size            = new System.Drawing.Size(380, 36);
        this.txtNombre.Font            = new System.Drawing.Font("Segoe UI", 11F);
        this.txtNombre.PlaceholderText = "Ej: Juan";
        this.txtNombre.BorderStyle     = System.Windows.Forms.BorderStyle.FixedSingle;
        this.txtNombre.BackColor       = System.Drawing.Color.White;
        this.txtNombre.TabIndex        = 0;

        // ── btnCrear ──────────────────────────────────────────────────────────
        this.btnCrear.Location                      = new System.Drawing.Point(36, 96);
        this.btnCrear.Size                          = new System.Drawing.Size(380, 48);
        this.btnCrear.Text                          = "🎤  Crear sala (soy el Gritón)";
        this.btnCrear.Font                          = new System.Drawing.Font("Segoe UI", 11F,
            System.Drawing.FontStyle.Bold);
        this.btnCrear.BackColor                     = System.Drawing.Color.FromArgb(0, 104, 56);
        this.btnCrear.ForeColor                     = System.Drawing.Color.White;
        this.btnCrear.FlatStyle                     = System.Windows.Forms.FlatStyle.Flat;
        this.btnCrear.FlatAppearance.BorderSize     = 0;
        this.btnCrear.Cursor                        = System.Windows.Forms.Cursors.Hand;
        this.btnCrear.TabIndex                      = 1;
        this.btnCrear.Click += new System.EventHandler(this.btnCrear_Click);

        // ── lblSeparador ──────────────────────────────────────────────────────
        this.lblSeparador.AutoSize  = false;
        this.lblSeparador.Location  = new System.Drawing.Point(36, 158);
        this.lblSeparador.Size      = new System.Drawing.Size(380, 20);
        this.lblSeparador.Text      = "─────────  o  ─────────";
        this.lblSeparador.Font      = new System.Drawing.Font("Segoe UI", 9F);
        this.lblSeparador.ForeColor = System.Drawing.Color.FromArgb(180, 180, 180);
        this.lblSeparador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // ── lblIpTxt ──────────────────────────────────────────────────────────
        this.lblIpTxt.AutoSize  = false;
        this.lblIpTxt.Location  = new System.Drawing.Point(36, 188);
        this.lblIpTxt.Size      = new System.Drawing.Size(380, 22);
        this.lblIpTxt.Text      = "IP del Gritón:";
        this.lblIpTxt.Font      = new System.Drawing.Font("Segoe UI", 9.5F,
            System.Drawing.FontStyle.Bold);
        this.lblIpTxt.ForeColor = System.Drawing.Color.FromArgb(60, 60, 60);

        // ── txtIp ─────────────────────────────────────────────────────────────
        this.txtIp.Location        = new System.Drawing.Point(36, 214);
        this.txtIp.Size            = new System.Drawing.Size(380, 36);
        this.txtIp.Font            = new System.Drawing.Font("Segoe UI", 11F);
        this.txtIp.PlaceholderText = "Ej: 192.168.1.10";
        this.txtIp.BorderStyle     = System.Windows.Forms.BorderStyle.FixedSingle;
        this.txtIp.BackColor       = System.Drawing.Color.White;
        this.txtIp.TabIndex        = 2;

        // ── btnUnirse ─────────────────────────────────────────────────────────
        this.btnUnirse.Location                     = new System.Drawing.Point(36, 264);
        this.btnUnirse.Size                         = new System.Drawing.Size(380, 48);
        this.btnUnirse.Text                         = "🃏  Unirme a la sala";
        this.btnUnirse.Font                         = new System.Drawing.Font("Segoe UI", 11F,
            System.Drawing.FontStyle.Bold);
        this.btnUnirse.BackColor                    = System.Drawing.Color.FromArgb(206, 17, 38);
        this.btnUnirse.ForeColor                    = System.Drawing.Color.White;
        this.btnUnirse.FlatStyle                    = System.Windows.Forms.FlatStyle.Flat;
        this.btnUnirse.FlatAppearance.BorderSize    = 0;
        this.btnUnirse.Cursor                       = System.Windows.Forms.Cursors.Hand;
        this.btnUnirse.TabIndex                     = 3;
        this.btnUnirse.Click += new System.EventHandler(this.btnUnirse_Click);

        // ── _lblIpLocal ───────────────────────────────────────────────────────
        this._lblIpLocal.AutoSize  = false;
        this._lblIpLocal.Location  = new System.Drawing.Point(36, 326);
        this._lblIpLocal.Size      = new System.Drawing.Size(380, 22);
        this._lblIpLocal.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
        this._lblIpLocal.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);

        // ── lblEstado ─────────────────────────────────────────────────────────
        this.lblEstado.AutoSize  = false;
        this.lblEstado.Location  = new System.Drawing.Point(36, 352);
        this.lblEstado.Size      = new System.Drawing.Size(380, 22);
        this.lblEstado.Font      = new System.Drawing.Font("Segoe UI", 8.5F,
            System.Drawing.FontStyle.Italic);
        this.lblEstado.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);

        // ── FormConexion ──────────────────────────────────────────────────────
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize          = new System.Drawing.Size(480, 560);
        this.MinimumSize         = new System.Drawing.Size(480, 560);
        this.FormBorderStyle     = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox         = false;
        this.StartPosition       = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text                = "Lotería Mexicana";
        this.BackColor           = System.Drawing.Color.FromArgb(0, 104, 56);
        this.Font                = new System.Drawing.Font("Segoe UI", 10F);

        this.Controls.Add(this.pnlContenido);
        this.Controls.Add(this.pnlBandaDorada);
        this.Controls.Add(this.pnlHeader);

        this.pnlHeader.ResumeLayout(false);
        this.pnlContenido.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel   pnlHeader;
    private System.Windows.Forms.Panel   pnlBandaDorada;
    private System.Windows.Forms.Label   lblTitulo;
    private System.Windows.Forms.Label   lblSubtitulo;
    private System.Windows.Forms.Panel   pnlContenido;
    private System.Windows.Forms.Label   lblNombreTxt;
    private System.Windows.Forms.TextBox txtNombre;
    private System.Windows.Forms.Button  btnCrear;
    private System.Windows.Forms.Label   lblSeparador;
    private System.Windows.Forms.Label   lblIpTxt;
    private System.Windows.Forms.TextBox txtIp;
    private System.Windows.Forms.Button  btnUnirse;
    private System.Windows.Forms.Label   _lblIpLocal;
    private System.Windows.Forms.Label   lblEstado;
}
