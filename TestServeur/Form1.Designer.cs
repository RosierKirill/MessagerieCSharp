namespace TestServeur
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.startButton = new System.Windows.Forms.Button();
            this.listBoxEvents = new System.Windows.Forms.ListBox();
            this.serveur1 = new NetworkTools.Controls.Serveur();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(27, 24);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // listBoxEvents
            // 
            this.listBoxEvents.FormattingEnabled = true;
            this.listBoxEvents.ItemHeight = 16;
            this.listBoxEvents.Location = new System.Drawing.Point(27, 84);
            this.listBoxEvents.Name = "listBoxEvents";
            this.listBoxEvents.Size = new System.Drawing.Size(288, 116);
            this.listBoxEvents.TabIndex = 2;
            // 
            // serveur1
            // 
            this.serveur1.Location = new System.Drawing.Point(321, 50);
            this.serveur1.Name = "serveur1";
            this.serveur1.Size = new System.Drawing.Size(150, 150);
            this.serveur1.TabIndex = 0;
            this.serveur1.Visible = false;
            this.serveur1.NetworkClientAccept += new System.EventHandler<NetworkTools.Controls.NetworkServerEventArgs>(this.serveur1_NetworkClientAccept);
            this.serveur1.NetworkClientClose += new System.EventHandler<NetworkTools.Controls.NetworkServerEventArgs>(this.serveur1_NetworkClientClose);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 225);
            this.Controls.Add(this.listBoxEvents);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.serveur1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private NetworkTools.Controls.Serveur serveur1;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.ListBox listBoxEvents;
    }
}

