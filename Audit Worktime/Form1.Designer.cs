namespace Audit_Worktime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtHoursDay = new System.Windows.Forms.TextBox();
            this.btnGetDayHours = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.btnGetMonthHours = new System.Windows.Forms.Button();
            this.txtHoursMonth = new System.Windows.Forms.TextBox();
            this.backgroundWorkerParseEvents = new System.ComponentModel.BackgroundWorker();
            this.progressBarEvents = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtHoursDay
            // 
            this.txtHoursDay.Location = new System.Drawing.Point(119, 6);
            this.txtHoursDay.Name = "txtHoursDay";
            this.txtHoursDay.ReadOnly = true;
            this.txtHoursDay.Size = new System.Drawing.Size(47, 20);
            this.txtHoursDay.TabIndex = 0;
            this.txtHoursDay.Text = "0";
            this.txtHoursDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnGetDayHours
            // 
            this.btnGetDayHours.Location = new System.Drawing.Point(66, 32);
            this.btnGetDayHours.Name = "btnGetDayHours";
            this.btnGetDayHours.Size = new System.Drawing.Size(100, 23);
            this.btnGetDayHours.TabIndex = 1;
            this.btnGetDayHours.Text = "Get Day worktime";
            this.btnGetDayHours.UseVisualStyleBackColor = true;
            this.btnGetDayHours.Click += new System.EventHandler(this.btnGetDayHours_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Selected day hours:";
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.FirstDayOfWeek = System.Windows.Forms.Day.Monday;
            this.monthCalendar1.Location = new System.Drawing.Point(178, 6);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.TabIndex = 3;
            this.monthCalendar1.TodayDate = new System.DateTime(((long)(0)));
            this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
            // 
            // btnGetMonthHours
            // 
            this.btnGetMonthHours.Location = new System.Drawing.Point(50, 203);
            this.btnGetMonthHours.Name = "btnGetMonthHours";
            this.btnGetMonthHours.Size = new System.Drawing.Size(116, 23);
            this.btnGetMonthHours.TabIndex = 4;
            this.btnGetMonthHours.Text = "Get month worktime";
            this.btnGetMonthHours.UseVisualStyleBackColor = true;
            this.btnGetMonthHours.Click += new System.EventHandler(this.btnGetMonthHours_Click);
            // 
            // txtHoursMonth
            // 
            this.txtHoursMonth.Location = new System.Drawing.Point(13, 59);
            this.txtHoursMonth.Multiline = true;
            this.txtHoursMonth.Name = "txtHoursMonth";
            this.txtHoursMonth.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtHoursMonth.Size = new System.Drawing.Size(153, 138);
            this.txtHoursMonth.TabIndex = 5;
            this.txtHoursMonth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // backgroundWorkerParseEvents
            // 
            this.backgroundWorkerParseEvents.WorkerReportsProgress = true;
            this.backgroundWorkerParseEvents.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerParseEvents_DoWork);
            this.backgroundWorkerParseEvents.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerParseEvents_ProgressChanged);
            this.backgroundWorkerParseEvents.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerParseEvents_RunWorkerCompleted);
            // 
            // progressBarEvents
            // 
            this.progressBarEvents.Location = new System.Drawing.Point(178, 203);
            this.progressBarEvents.Name = "progressBarEvents";
            this.progressBarEvents.Size = new System.Drawing.Size(164, 23);
            this.progressBarEvents.Step = 1;
            this.progressBarEvents.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 237);
            this.Controls.Add(this.progressBarEvents);
            this.Controls.Add(this.txtHoursMonth);
            this.Controls.Add(this.btnGetMonthHours);
            this.Controls.Add(this.monthCalendar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnGetDayHours);
            this.Controls.Add(this.txtHoursDay);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audit Worktime v1.2";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHoursDay;
        private System.Windows.Forms.Button btnGetDayHours;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.Button btnGetMonthHours;
        private System.Windows.Forms.TextBox txtHoursMonth;
        private System.ComponentModel.BackgroundWorker backgroundWorkerParseEvents;
        private System.Windows.Forms.ProgressBar progressBarEvents;
    }
}

