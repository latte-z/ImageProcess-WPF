namespace ImageProcess.Emgu
{
    partial class DisLeftPaint
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
            this.paintPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.paintPic)).BeginInit();
            this.SuspendLayout();
            // 
            // paintPic
            // 
            this.paintPic.Location = new System.Drawing.Point(0, 0);
            this.paintPic.Name = "paintPic";
            this.paintPic.Size = new System.Drawing.Size(456, 456);
            this.paintPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.paintPic.TabIndex = 0;
            this.paintPic.TabStop = false;
            this.paintPic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.paintPic_MouseDown);
            this.paintPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.paintPic_MouseMove);
            this.paintPic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.paintPic_MouseUp);
            // 
            // DisLeftPaint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 456);
            this.Controls.Add(this.paintPic);
            this.Name = "DisLeftPaint";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DisLeftPaint";
            ((System.ComponentModel.ISupportInitialize)(this.paintPic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox paintPic;
    }
}