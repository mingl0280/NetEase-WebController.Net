namespace NUCForeground
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn = new System.Windows.Forms.Button();
            this.tTopMostSetup = new System.Windows.Forms.Timer(this.components);
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoop = new System.Windows.Forms.Button();
            this.btnNEM = new System.Windows.Forms.Button();
            this.btnWechat = new System.Windows.Forms.Button();
            this.btnLrc = new System.Windows.Forms.Button();
            this.btnShutdown = new System.Windows.Forms.Button();
            this.btnHibernate = new System.Windows.Forms.Button();
            this.btnVolUp = new System.Windows.Forms.Button();
            this.btnVolDown = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.btnPrevMusic = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn
            // 
            this.btn.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btn.BackgroundImage = global::NUCForeground.Properties.Resources.windows;
            this.btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btn.FlatAppearance.BorderSize = 0;
            this.btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn.Location = new System.Drawing.Point(454, 571);
            this.btn.Name = "btn";
            this.btn.Size = new System.Drawing.Size(160, 160);
            this.btn.TabIndex = 24;
            this.btn.UseVisualStyleBackColor = false;
            this.btn.Click += new System.EventHandler(this.btn_Click);
            // 
            // tTopMostSetup
            // 
            this.tTopMostSetup.Interval = 5000;
            this.tTopMostSetup.Tick += new System.EventHandler(this.tTopMostSetup_Tick);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnExit.BackgroundImage = global::NUCForeground.Properties.Resources.Exit;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(1151, 631);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 100);
            this.btnExit.TabIndex = 26;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.button10_Click);
            // 
            // btnLoop
            // 
            this.btnLoop.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLoop.BackgroundImage = global::NUCForeground.Properties.Resources.ListNoLoop;
            this.btnLoop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLoop.FlatAppearance.BorderSize = 0;
            this.btnLoop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoop.Location = new System.Drawing.Point(669, 571);
            this.btnLoop.Name = "btnLoop";
            this.btnLoop.Size = new System.Drawing.Size(160, 160);
            this.btnLoop.TabIndex = 25;
            this.btnLoop.UseVisualStyleBackColor = false;
            this.btnLoop.Click += new System.EventHandler(this.btnLoop_Click);
            // 
            // btnNEM
            // 
            this.btnNEM.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNEM.BackgroundImage = global::NUCForeground.Properties.Resources.NEM;
            this.btnNEM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNEM.FlatAppearance.BorderSize = 0;
            this.btnNEM.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNEM.Location = new System.Drawing.Point(239, 571);
            this.btnNEM.Name = "btnNEM";
            this.btnNEM.Size = new System.Drawing.Size(160, 160);
            this.btnNEM.TabIndex = 23;
            this.btnNEM.UseVisualStyleBackColor = false;
            this.btnNEM.Click += new System.EventHandler(this.btnNEM_Click);
            // 
            // btnWechat
            // 
            this.btnWechat.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnWechat.BackgroundImage = global::NUCForeground.Properties.Resources.WeChat;
            this.btnWechat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnWechat.FlatAppearance.BorderSize = 0;
            this.btnWechat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnWechat.Location = new System.Drawing.Point(24, 571);
            this.btnWechat.Name = "btnWechat";
            this.btnWechat.Size = new System.Drawing.Size(160, 160);
            this.btnWechat.TabIndex = 22;
            this.btnWechat.UseVisualStyleBackColor = false;
            this.btnWechat.Click += new System.EventHandler(this.btnWechat_Click);
            // 
            // btnLrc
            // 
            this.btnLrc.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLrc.BackgroundImage = global::NUCForeground.Properties.Resources.Lyric;
            this.btnLrc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLrc.FlatAppearance.BorderSize = 0;
            this.btnLrc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLrc.Location = new System.Drawing.Point(884, 571);
            this.btnLrc.Name = "btnLrc";
            this.btnLrc.Size = new System.Drawing.Size(160, 160);
            this.btnLrc.TabIndex = 21;
            this.btnLrc.UseVisualStyleBackColor = false;
            this.btnLrc.Click += new System.EventHandler(this.btnLrc_Click);
            // 
            // btnShutdown
            // 
            this.btnShutdown.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnShutdown.BackgroundImage = global::NUCForeground.Properties.Resources.Power;
            this.btnShutdown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnShutdown.FlatAppearance.BorderSize = 0;
            this.btnShutdown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShutdown.Location = new System.Drawing.Point(1151, 475);
            this.btnShutdown.Name = "btnShutdown";
            this.btnShutdown.Size = new System.Drawing.Size(100, 100);
            this.btnShutdown.TabIndex = 20;
            this.btnShutdown.UseVisualStyleBackColor = false;
            this.btnShutdown.Click += new System.EventHandler(this.btnShutdown_Click);
            // 
            // btnHibernate
            // 
            this.btnHibernate.BackColor = System.Drawing.Color.Transparent;
            this.btnHibernate.BackgroundImage = global::NUCForeground.Properties.Resources.Hibernate;
            this.btnHibernate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnHibernate.FlatAppearance.BorderSize = 0;
            this.btnHibernate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHibernate.Location = new System.Drawing.Point(1151, 319);
            this.btnHibernate.Name = "btnHibernate";
            this.btnHibernate.Size = new System.Drawing.Size(100, 100);
            this.btnHibernate.TabIndex = 19;
            this.btnHibernate.UseVisualStyleBackColor = false;
            this.btnHibernate.Click += new System.EventHandler(this.btnHibernate_Click);
            // 
            // btnVolUp
            // 
            this.btnVolUp.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnVolUp.BackgroundImage = global::NUCForeground.Properties.Resources.VolUp;
            this.btnVolUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnVolUp.FlatAppearance.BorderSize = 0;
            this.btnVolUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolUp.Location = new System.Drawing.Point(884, 377);
            this.btnVolUp.Name = "btnVolUp";
            this.btnVolUp.Size = new System.Drawing.Size(160, 160);
            this.btnVolUp.TabIndex = 18;
            this.btnVolUp.UseVisualStyleBackColor = false;
            this.btnVolUp.Click += new System.EventHandler(this.btnVolUp_Click);
            // 
            // btnVolDown
            // 
            this.btnVolDown.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnVolDown.BackgroundImage = global::NUCForeground.Properties.Resources.VolDown;
            this.btnVolDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnVolDown.FlatAppearance.BorderSize = 0;
            this.btnVolDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolDown.Location = new System.Drawing.Point(669, 377);
            this.btnVolDown.Name = "btnVolDown";
            this.btnVolDown.Size = new System.Drawing.Size(160, 160);
            this.btnVolDown.TabIndex = 17;
            this.btnVolDown.UseVisualStyleBackColor = false;
            this.btnVolDown.Click += new System.EventHandler(this.btnVolDown_Click);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnNext.BackgroundImage = global::NUCForeground.Properties.Resources.Next;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Location = new System.Drawing.Point(454, 377);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(160, 160);
            this.btnNext.TabIndex = 16;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnPlayPause.BackgroundImage = global::NUCForeground.Properties.Resources.Play;
            this.btnPlayPause.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPlayPause.FlatAppearance.BorderSize = 0;
            this.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayPause.Location = new System.Drawing.Point(239, 377);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(160, 160);
            this.btnPlayPause.TabIndex = 15;
            this.btnPlayPause.UseVisualStyleBackColor = false;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // btnPrevMusic
            // 
            this.btnPrevMusic.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnPrevMusic.BackgroundImage = global::NUCForeground.Properties.Resources.Prev;
            this.btnPrevMusic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPrevMusic.FlatAppearance.BorderSize = 0;
            this.btnPrevMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrevMusic.Location = new System.Drawing.Point(24, 377);
            this.btnPrevMusic.Name = "btnPrevMusic";
            this.btnPrevMusic.Size = new System.Drawing.Size(160, 160);
            this.btnPrevMusic.TabIndex = 14;
            this.btnPrevMusic.UseVisualStyleBackColor = false;
            this.btnPrevMusic.Click += new System.EventHandler(this.btnPrevMusic_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 768);
            this.ControlBox = false;
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLoop);
            this.Controls.Add(this.btn);
            this.Controls.Add(this.btnNEM);
            this.Controls.Add(this.btnWechat);
            this.Controls.Add(this.btnLrc);
            this.Controls.Add(this.btnShutdown);
            this.Controls.Add(this.btnHibernate);
            this.Controls.Add(this.btnVolUp);
            this.Controls.Add(this.btnVolDown);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPlayPause);
            this.Controls.Add(this.btnPrevMusic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.Activated += new System.EventHandler(this.FrmMain_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLoop;
        private System.Windows.Forms.Button btn;
        private System.Windows.Forms.Button btnNEM;
        private System.Windows.Forms.Button btnWechat;
        private System.Windows.Forms.Button btnLrc;
        private System.Windows.Forms.Button btnShutdown;
        private System.Windows.Forms.Button btnHibernate;
        private System.Windows.Forms.Button btnVolUp;
        private System.Windows.Forms.Button btnVolDown;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.Button btnPrevMusic;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer tTopMostSetup;
    }
}

