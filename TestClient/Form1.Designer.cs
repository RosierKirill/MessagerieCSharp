namespace TestClient
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
            this.connectionButton = new System.Windows.Forms.Button();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.client1 = new NetworkTools.Controls.Client();
            this.textBoxRecu = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // connectionButton
            // 
            this.connectionButton.Location = new System.Drawing.Point(44, 24);
            this.connectionButton.Name = "connectionButton";
            this.connectionButton.Size = new System.Drawing.Size(98, 30);
            this.connectionButton.TabIndex = 0;
            this.connectionButton.Text = "Connection";
            this.connectionButton.UseVisualStyleBackColor = true;
            this.connectionButton.Click += new System.EventHandler(this.connectionButton_Click);
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(44, 60);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(347, 196);
            this.textBoxMessage.TabIndex = 1;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(397, 233);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 2;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // client1
            // 
            this.client1.Location = new System.Drawing.Point(397, 60);
            this.client1.Name = "client1";
            this.client1.Size = new System.Drawing.Size(150, 150);
            this.client1.TabIndex = 3;
            this.client1.Visible = false;
            this.client1.NetworkClientMessageReceived += new System.EventHandler<NetworkTools.Controls.NetworkClientEventArgs>(this.client1_NetworkClientMessageReceived);
            // 
            // textBoxRecu
            // 
            this.textBoxRecu.Location = new System.Drawing.Point(44, 284);
            this.textBoxRecu.Multiline = true;
            this.textBoxRecu.Name = "textBoxRecu";
            this.textBoxRecu.ReadOnly = true;
            this.textBoxRecu.Size = new System.Drawing.Size(347, 196);
            this.textBoxRecu.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 497);
            this.Controls.Add(this.textBoxRecu);
            this.Controls.Add(this.client1);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.connectionButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectionButton;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.Button sendButton;
        private NetworkTools.Controls.Client client1;
        private System.Windows.Forms.TextBox textBoxRecu;
    }
}

