namespace Train.Trains
{
    partial class SetLocForm
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
            this.tbKilo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTrainLength = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbMeter = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbKilo
            // 
            this.tbKilo.Location = new System.Drawing.Point(52, 131);
            this.tbKilo.Name = "tbKilo";
            this.tbKilo.Size = new System.Drawing.Size(30, 21);
            this.tbKilo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "列车左端位置：";
            // 
            // lblTrainLength
            // 
            this.lblTrainLength.AutoSize = true;
            this.lblTrainLength.Location = new System.Drawing.Point(35, 71);
            this.lblTrainLength.Name = "lblTrainLength";
            this.lblTrainLength.Size = new System.Drawing.Size(65, 12);
            this.lblTrainLength.TabIndex = 1;
            this.lblTrainLength.Text = "列车长度：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "K";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(92, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "+";
            // 
            // tbMeter
            // 
            this.tbMeter.Location = new System.Drawing.Point(118, 131);
            this.tbMeter.Name = "tbMeter";
            this.tbMeter.Size = new System.Drawing.Size(42, 21);
            this.tbMeter.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(49, 187);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // SetLocForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblTrainLength);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbMeter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbKilo);
            this.MaximizeBox = false;
            this.Name = "SetLocForm";
            this.Text = "SetLocForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbKilo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTrainLength;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbMeter;
        private System.Windows.Forms.Button btnOK;
    }
}