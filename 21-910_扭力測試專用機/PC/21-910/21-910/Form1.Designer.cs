﻿namespace _21_910
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lbConnectStatus908 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbConnectStatus909 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbConnectStatus910 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.pbSaveData = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbConnectStatus908
            // 
            this.lbConnectStatus908.AutoSize = true;
            this.lbConnectStatus908.BackColor = System.Drawing.Color.Red;
            this.lbConnectStatus908.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbConnectStatus908.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConnectStatus908.Location = new System.Drawing.Point(6, 43);
            this.lbConnectStatus908.Name = "lbConnectStatus908";
            this.lbConnectStatus908.Size = new System.Drawing.Size(174, 37);
            this.lbConnectStatus908.TabIndex = 0;
            this.lbConnectStatus908.Text = "未連線";
            this.lbConnectStatus908.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "有負載測試機";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.Location = new System.Drawing.Point(189, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 37);
            this.label2.TabIndex = 2;
            this.label2.Text = "無負載測試機";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbConnectStatus909
            // 
            this.lbConnectStatus909.AutoSize = true;
            this.lbConnectStatus909.BackColor = System.Drawing.Color.Red;
            this.lbConnectStatus909.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbConnectStatus909.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConnectStatus909.Location = new System.Drawing.Point(189, 43);
            this.lbConnectStatus909.Name = "lbConnectStatus909";
            this.lbConnectStatus909.Size = new System.Drawing.Size(174, 37);
            this.lbConnectStatus909.TabIndex = 4;
            this.lbConnectStatus909.Text = "未連線";
            this.lbConnectStatus909.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.lbConnectStatus909, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbConnectStatus910, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lbConnectStatus908, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33778F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33778F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(554, 83);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // lbConnectStatus910
            // 
            this.lbConnectStatus910.AutoSize = true;
            this.lbConnectStatus910.BackColor = System.Drawing.Color.Red;
            this.lbConnectStatus910.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbConnectStatus910.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lbConnectStatus910.Location = new System.Drawing.Point(372, 43);
            this.lbConnectStatus910.Name = "lbConnectStatus910";
            this.lbConnectStatus910.Size = new System.Drawing.Size(176, 37);
            this.lbConnectStatus910.TabIndex = 6;
            this.lbConnectStatus910.Text = "未連線";
            this.lbConnectStatus910.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("新細明體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label3.Location = new System.Drawing.Point(372, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 37);
            this.label3.TabIndex = 4;
            this.label3.Text = "扭力測試機";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(12, 111);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 22);
            this.dateTimePicker1.TabIndex = 7;
            // 
            // pbSaveData
            // 
            this.pbSaveData.Location = new System.Drawing.Point(218, 111);
            this.pbSaveData.Name = "pbSaveData";
            this.pbSaveData.Size = new System.Drawing.Size(75, 23);
            this.pbSaveData.TabIndex = 8;
            this.pbSaveData.Text = "存檔";
            this.pbSaveData.UseVisualStyleBackColor = true;
            this.pbSaveData.Click += new System.EventHandler(this.pbSaveData_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(253, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "或可手動挑選日期存檔,位置:D:\\DailyReport\\之下";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(310, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "每日關閉程式時自動存檔";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 148);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pbSaveData);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "台灣保來得";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbConnectStatus908;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbConnectStatus909;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbConnectStatus910;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button pbSaveData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}

