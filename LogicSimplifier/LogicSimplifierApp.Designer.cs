
namespace LogicSimplifier
{
    partial class LogicSimplifierApp
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
            this.components = new System.ComponentModel.Container();
            this.settingsCheckList = new System.Windows.Forms.CheckedListBox();
            this.globalSettingsToggle = new System.Windows.Forms.CheckBox();
            this.waypointComboBox = new System.Windows.Forms.ComboBox();
            this.labelWaypointBox = new System.Windows.Forms.Label();
            this.buttonCompute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.labelLastWaypoint = new System.Windows.Forms.Label();
            this.labelUpdateStack = new System.Windows.Forms.Label();
            this.labelAbsLogicStatements = new System.Windows.Forms.Label();
            this.labelRelLogicStatements = new System.Windows.Forms.Label();
            this.labelUpdateDepth = new System.Windows.Forms.Label();
            this.labelTimeElapsed = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // settingsCheckList
            // 
            this.settingsCheckList.CheckOnClick = true;
            this.settingsCheckList.Enabled = false;
            this.settingsCheckList.FormattingEnabled = true;
            this.settingsCheckList.Location = new System.Drawing.Point(6, 102);
            this.settingsCheckList.Name = "settingsCheckList";
            this.settingsCheckList.Size = new System.Drawing.Size(150, 140);
            this.settingsCheckList.TabIndex = 0;
            // 
            // globalSettingsToggle
            // 
            this.globalSettingsToggle.AutoSize = true;
            this.globalSettingsToggle.Location = new System.Drawing.Point(9, 75);
            this.globalSettingsToggle.Name = "globalSettingsToggle";
            this.globalSettingsToggle.Size = new System.Drawing.Size(207, 21);
            this.globalSettingsToggle.TabIndex = 1;
            this.globalSettingsToggle.Text = "Set settings flags for output.";
            this.globalSettingsToggle.UseVisualStyleBackColor = true;
            this.globalSettingsToggle.CheckedChanged += new System.EventHandler(this.globalSettingsToggle_CheckedChanged);
            // 
            // waypointComboBox
            // 
            this.waypointComboBox.FormattingEnabled = true;
            this.waypointComboBox.Location = new System.Drawing.Point(6, 38);
            this.waypointComboBox.Name = "waypointComboBox";
            this.waypointComboBox.Size = new System.Drawing.Size(210, 24);
            this.waypointComboBox.TabIndex = 2;
            // 
            // labelWaypointBox
            // 
            this.labelWaypointBox.AutoSize = true;
            this.labelWaypointBox.Location = new System.Drawing.Point(6, 18);
            this.labelWaypointBox.Name = "labelWaypointBox";
            this.labelWaypointBox.Size = new System.Drawing.Size(116, 17);
            this.labelWaypointBox.TabIndex = 3;
            this.labelWaypointBox.Text = "Starting waypoint";
            // 
            // buttonCompute
            // 
            this.buttonCompute.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCompute.Location = new System.Drawing.Point(6, 406);
            this.buttonCompute.Name = "buttonCompute";
            this.buttonCompute.Size = new System.Drawing.Size(224, 38);
            this.buttonCompute.TabIndex = 4;
            this.buttonCompute.Text = "Compute new logic";
            this.buttonCompute.UseVisualStyleBackColor = true;
            this.buttonCompute.Click += new System.EventHandler(this.buttonCompute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelWaypointBox);
            this.groupBox1.Controls.Add(this.buttonCompute);
            this.groupBox1.Controls.Add(this.waypointComboBox);
            this.groupBox1.Controls.Add(this.settingsCheckList);
            this.groupBox1.Controls.Add(this.globalSettingsToggle);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 450);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setup";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelLastWaypoint);
            this.groupBox2.Controls.Add(this.labelUpdateStack);
            this.groupBox2.Controls.Add(this.labelAbsLogicStatements);
            this.groupBox2.Controls.Add(this.labelRelLogicStatements);
            this.groupBox2.Controls.Add(this.labelUpdateDepth);
            this.groupBox2.Controls.Add(this.labelTimeElapsed);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(236, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(564, 96);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Info";
            // 
            // labelLastWaypoint
            // 
            this.labelLastWaypoint.AutoSize = true;
            this.labelLastWaypoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelLastWaypoint.Location = new System.Drawing.Point(235, 68);
            this.labelLastWaypoint.Name = "labelLastWaypoint";
            this.labelLastWaypoint.Size = new System.Drawing.Size(86, 17);
            this.labelLastWaypoint.TabIndex = 5;
            this.labelLastWaypoint.Text = "Most recent:";
            // 
            // labelUpdateStack
            // 
            this.labelUpdateStack.AutoSize = true;
            this.labelUpdateStack.Location = new System.Drawing.Point(7, 68);
            this.labelUpdateStack.Name = "labelUpdateStack";
            this.labelUpdateStack.Size = new System.Drawing.Size(95, 17);
            this.labelUpdateStack.TabIndex = 4;
            this.labelUpdateStack.Text = "Update stack:";
            // 
            // labelAbsLogicStatements
            // 
            this.labelAbsLogicStatements.AutoSize = true;
            this.labelAbsLogicStatements.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelAbsLogicStatements.Location = new System.Drawing.Point(235, 45);
            this.labelAbsLogicStatements.Name = "labelAbsLogicStatements";
            this.labelAbsLogicStatements.Size = new System.Drawing.Size(173, 17);
            this.labelAbsLogicStatements.TabIndex = 3;
            this.labelAbsLogicStatements.Text = "Absolute logic statements:";
            // 
            // labelRelLogicStatements
            // 
            this.labelRelLogicStatements.AutoSize = true;
            this.labelRelLogicStatements.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelRelLogicStatements.Location = new System.Drawing.Point(235, 22);
            this.labelRelLogicStatements.Name = "labelRelLogicStatements";
            this.labelRelLogicStatements.Size = new System.Drawing.Size(169, 17);
            this.labelRelLogicStatements.TabIndex = 2;
            this.labelRelLogicStatements.Text = "Relative logic statements:";
            // 
            // labelUpdateDepth
            // 
            this.labelUpdateDepth.AutoSize = true;
            this.labelUpdateDepth.Location = new System.Drawing.Point(7, 45);
            this.labelUpdateDepth.Name = "labelUpdateDepth";
            this.labelUpdateDepth.Size = new System.Drawing.Size(98, 17);
            this.labelUpdateDepth.TabIndex = 1;
            this.labelUpdateDepth.Text = "Update depth:";
            // 
            // labelTimeElapsed
            // 
            this.labelTimeElapsed.AutoSize = true;
            this.labelTimeElapsed.Location = new System.Drawing.Point(7, 22);
            this.labelTimeElapsed.Name = "labelTimeElapsed";
            this.labelTimeElapsed.Size = new System.Drawing.Size(97, 17);
            this.labelTimeElapsed.TabIndex = 0;
            this.labelTimeElapsed.Text = "Time elapsed:";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LogicSimplifierApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "LogicSimplifierApp";
            this.Text = "LogicSimplifier";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox settingsCheckList;
        private System.Windows.Forms.CheckBox globalSettingsToggle;
        private System.Windows.Forms.ComboBox waypointComboBox;
        private System.Windows.Forms.Label labelWaypointBox;
        private System.Windows.Forms.Button buttonCompute;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label labelLastWaypoint;
        private System.Windows.Forms.Label labelUpdateStack;
        private System.Windows.Forms.Label labelAbsLogicStatements;
        private System.Windows.Forms.Label labelRelLogicStatements;
        private System.Windows.Forms.Label labelUpdateDepth;
        private System.Windows.Forms.Label labelTimeElapsed;
        private System.Windows.Forms.Timer timer1;
    }
}

