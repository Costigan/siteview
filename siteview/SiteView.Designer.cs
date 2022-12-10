namespace siteview
{
    partial class SiteView
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.siteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlsPanelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.legendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.plotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.test1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblLatLon = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlTime = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.tbCurrentTime = new System.Windows.Forms.TextBox();
            this.cbTimeSpan = new System.Windows.Forms.ComboBox();
            this.cbUpdateContinuously = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dtStartTime = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.tbSelectTime = new System.Windows.Forms.TrackBar();
            this.pnlPlot = new System.Windows.Forms.Panel();
            this.tabRight = new System.Windows.Forms.TabControl();
            this.tabProperties = new System.Windows.Forms.TabPage();
            this.pnlPropertiesHolder = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.tabLeft = new System.Windows.Forms.TabControl();
            this.tabLayers = new System.Windows.Forms.TabPage();
            this.lvLayers = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTransparency = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbTransparency = new System.Windows.Forms.TrackBar();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.tabCenter = new System.Windows.Forms.TabControl();
            this.tabMap = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSelectTime)).BeginInit();
            this.tabRight.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabLeft.SuspendLayout();
            this.tabLayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTransparency)).BeginInit();
            this.tabCenter.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.siteToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1178, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // siteToolStripMenuItem
            // 
            this.siteToolStripMenuItem.Name = "siteToolStripMenuItem";
            this.siteToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.siteToolStripMenuItem.Text = "&Sites";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlsToolStripMenuItem,
            this.controlsPanelToolStripMenuItem,
            this.legendToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.plotToolStripMenuItem,
            this.timeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.viewToolStripMenuItem.Text = "&Panels";
            // 
            // controlsToolStripMenuItem
            // 
            this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
            this.controlsToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.controlsToolStripMenuItem.Text = "Controls";
            // 
            // controlsPanelToolStripMenuItem
            // 
            this.controlsPanelToolStripMenuItem.Checked = true;
            this.controlsPanelToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.controlsPanelToolStripMenuItem.Name = "controlsPanelToolStripMenuItem";
            this.controlsPanelToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.controlsPanelToolStripMenuItem.Text = "Layers";
            this.controlsPanelToolStripMenuItem.Click += new System.EventHandler(this.ControlsPanelToolStripMenuItem_Click);
            // 
            // legendToolStripMenuItem
            // 
            this.legendToolStripMenuItem.Checked = true;
            this.legendToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.legendToolStripMenuItem.Name = "legendToolStripMenuItem";
            this.legendToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.legendToolStripMenuItem.Text = "Legend";
            this.legendToolStripMenuItem.Click += new System.EventHandler(this.LegendToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.PropertiesToolStripMenuItem_Click);
            // 
            // plotToolStripMenuItem
            // 
            this.plotToolStripMenuItem.Name = "plotToolStripMenuItem";
            this.plotToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.plotToolStripMenuItem.Text = "Plot";
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.timeToolStripMenuItem.Text = "Time";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.test1ToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.testToolStripMenuItem.Text = "&Test";
            // 
            // test1ToolStripMenuItem
            // 
            this.test1ToolStripMenuItem.Name = "test1ToolStripMenuItem";
            this.test1ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.test1ToolStripMenuItem.Text = "&Test1";
            this.test1ToolStripMenuItem.Click += new System.EventHandler(this.test1ToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblLatLon});
            this.statusStrip1.Location = new System.Drawing.Point(0, 710);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1178, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblLatLon
            // 
            this.lblLatLon.AutoSize = false;
            this.lblLatLon.Name = "lblLatLon";
            this.lblLatLon.Size = new System.Drawing.Size(600, 17);
            this.lblLatLon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlTime
            // 
            this.pnlTime.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlTime.Controls.Add(this.label9);
            this.pnlTime.Controls.Add(this.tbCurrentTime);
            this.pnlTime.Controls.Add(this.cbTimeSpan);
            this.pnlTime.Controls.Add(this.cbUpdateContinuously);
            this.pnlTime.Controls.Add(this.label7);
            this.pnlTime.Controls.Add(this.label10);
            this.pnlTime.Controls.Add(this.label6);
            this.pnlTime.Controls.Add(this.dtStartTime);
            this.pnlTime.Controls.Add(this.label5);
            this.pnlTime.Controls.Add(this.tbSelectTime);
            this.pnlTime.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlTime.Location = new System.Drawing.Point(0, 656);
            this.pnlTime.Name = "pnlTime";
            this.pnlTime.Size = new System.Drawing.Size(1178, 54);
            this.pnlTime.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.White;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label9.Location = new System.Drawing.Point(671, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(157, 20);
            this.label9.TabIndex = 5;
            // 
            // tbCurrentTime
            // 
            this.tbCurrentTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCurrentTime.Location = new System.Drawing.Point(298, 27);
            this.tbCurrentTime.Name = "tbCurrentTime";
            this.tbCurrentTime.Size = new System.Drawing.Size(140, 20);
            this.tbCurrentTime.TabIndex = 4;
            // 
            // cbTimeSpan
            // 
            this.cbTimeSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTimeSpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbTimeSpan.FormattingEnabled = true;
            this.cbTimeSpan.Items.AddRange(new object[] {
            "1 hour",
            "1 day",
            "1 week",
            "2 weeks",
            "1 Month",
            "2 Months",
            "3 Months",
            "6 Months",
            "1 Year",
            "2 Years"});
            this.cbTimeSpan.Location = new System.Drawing.Point(500, 26);
            this.cbTimeSpan.Name = "cbTimeSpan";
            this.cbTimeSpan.Size = new System.Drawing.Size(117, 21);
            this.cbTimeSpan.TabIndex = 3;
            // 
            // cbUpdateContinuously
            // 
            this.cbUpdateContinuously.AutoSize = true;
            this.cbUpdateContinuously.Checked = true;
            this.cbUpdateContinuously.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUpdateContinuously.Location = new System.Drawing.Point(883, 28);
            this.cbUpdateContinuously.Name = "cbUpdateContinuously";
            this.cbUpdateContinuously.Size = new System.Drawing.Size(124, 17);
            this.cbUpdateContinuously.TabIndex = 3;
            this.cbUpdateContinuously.Text = "Update Continuously";
            this.cbUpdateContinuously.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(444, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Duration:";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(623, 20);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 30);
            this.label10.TabIndex = 2;
            this.label10.Text = "Interval Stop:";
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(4, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 30);
            this.label6.TabIndex = 2;
            this.label6.Text = "Interval Start:";
            // 
            // dtStartTime
            // 
            this.dtStartTime.Location = new System.Drawing.Point(52, 27);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Size = new System.Drawing.Size(197, 20);
            this.dtStartTime.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(254, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 30);
            this.label5.TabIndex = 2;
            this.label5.Text = "Current Time:";
            // 
            // tbSelectTime
            // 
            this.tbSelectTime.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tbSelectTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbSelectTime.LargeChange = 50;
            this.tbSelectTime.Location = new System.Drawing.Point(0, 0);
            this.tbSelectTime.Maximum = 10000;
            this.tbSelectTime.Name = "tbSelectTime";
            this.tbSelectTime.Size = new System.Drawing.Size(1178, 45);
            this.tbSelectTime.TabIndex = 1;
            this.tbSelectTime.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // pnlPlot
            // 
            this.pnlPlot.BackColor = System.Drawing.Color.White;
            this.pnlPlot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlPlot.Location = new System.Drawing.Point(0, 546);
            this.pnlPlot.Name = "pnlPlot";
            this.pnlPlot.Size = new System.Drawing.Size(1178, 110);
            this.pnlPlot.TabIndex = 20;
            // 
            // tabRight
            // 
            this.tabRight.Controls.Add(this.tabProperties);
            this.tabRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabRight.Location = new System.Drawing.Point(978, 73);
            this.tabRight.Name = "tabRight";
            this.tabRight.SelectedIndex = 0;
            this.tabRight.Size = new System.Drawing.Size(200, 470);
            this.tabRight.TabIndex = 22;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.pnlPropertiesHolder);
            this.tabProperties.Location = new System.Drawing.Point(4, 22);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabProperties.Size = new System.Drawing.Size(192, 444);
            this.tabProperties.TabIndex = 0;
            this.tabProperties.Text = "Properties";
            this.tabProperties.UseVisualStyleBackColor = true;
            // 
            // pnlPropertiesHolder
            // 
            this.pnlPropertiesHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPropertiesHolder.Location = new System.Drawing.Point(3, 3);
            this.pnlPropertiesHolder.Name = "pnlPropertiesHolder";
            this.pnlPropertiesHolder.Size = new System.Drawing.Size(186, 438);
            this.pnlPropertiesHolder.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 543);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(1178, 3);
            this.splitter1.TabIndex = 23;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter2.Location = new System.Drawing.Point(975, 73);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 470);
            this.splitter2.TabIndex = 24;
            this.splitter2.TabStop = false;
            // 
            // tabLeft
            // 
            this.tabLeft.Controls.Add(this.tabLayers);
            this.tabLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabLeft.Location = new System.Drawing.Point(0, 73);
            this.tabLeft.Name = "tabLeft";
            this.tabLeft.SelectedIndex = 0;
            this.tabLeft.Size = new System.Drawing.Size(219, 470);
            this.tabLeft.TabIndex = 25;
            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.lvLayers);
            this.tabLayers.Controls.Add(this.tbTransparency);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(211, 444);
            this.tabLayers.TabIndex = 0;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // lvLayers
            // 
            this.lvLayers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvLayers.CheckBoxes = true;
            this.lvLayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colTransparency});
            this.lvLayers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLayers.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvLayers.FullRowSelect = true;
            this.lvLayers.GridLines = true;
            this.lvLayers.HideSelection = false;
            this.lvLayers.Location = new System.Drawing.Point(3, 3);
            this.lvLayers.MultiSelect = false;
            this.lvLayers.Name = "lvLayers";
            this.lvLayers.ShowGroups = false;
            this.lvLayers.Size = new System.Drawing.Size(205, 393);
            this.lvLayers.TabIndex = 3;
            this.lvLayers.UseCompatibleStateImageBehavior = false;
            this.lvLayers.View = System.Windows.Forms.View.Details;
            this.lvLayers.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvLayers_ItemChecked);
            this.lvLayers.SelectedIndexChanged += new System.EventHandler(this.lvLayers_SelectedIndexChanged);
            this.lvLayers.SizeChanged += new System.EventHandler(this.lvLayers_SizeChanged);
            this.lvLayers.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvLayers_KeyDown);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 160;
            // 
            // colTransparency
            // 
            this.colTransparency.Text = " % ";
            this.colTransparency.Width = 40;
            // 
            // tbTransparency
            // 
            this.tbTransparency.BackColor = System.Drawing.Color.White;
            this.tbTransparency.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbTransparency.Location = new System.Drawing.Point(3, 396);
            this.tbTransparency.Maximum = 100;
            this.tbTransparency.Name = "tbTransparency";
            this.tbTransparency.Size = new System.Drawing.Size(205, 45);
            this.tbTransparency.TabIndex = 2;
            this.tbTransparency.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbTransparency.ValueChanged += new System.EventHandler(this.tbTransparency_ValueChanged);
            // 
            // splitter3
            // 
            this.splitter3.Location = new System.Drawing.Point(219, 73);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 470);
            this.splitter3.TabIndex = 26;
            this.splitter3.TabStop = false;
            // 
            // tabCenter
            // 
            this.tabCenter.Controls.Add(this.tabMap);
            this.tabCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCenter.Location = new System.Drawing.Point(222, 73);
            this.tabCenter.Name = "tabCenter";
            this.tabCenter.SelectedIndex = 0;
            this.tabCenter.Size = new System.Drawing.Size(650, 470);
            this.tabCenter.TabIndex = 28;
            // 
            // tabMap
            // 
            this.tabMap.Location = new System.Drawing.Point(4, 22);
            this.tabMap.Name = "tabMap";
            this.tabMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabMap.Size = new System.Drawing.Size(642, 444);
            this.tabMap.TabIndex = 0;
            this.tabMap.Text = "Map";
            this.tabMap.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabControl1.Location = new System.Drawing.Point(875, 73);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(100, 470);
            this.tabControl1.TabIndex = 28;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(92, 444);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Legend";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter4.Location = new System.Drawing.Point(872, 73);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 470);
            this.splitter4.TabIndex = 29;
            this.splitter4.TabStop = false;
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.label1);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 24);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(1178, 49);
            this.pnlControls.TabIndex = 30;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Controls";
            // 
            // SiteView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 732);
            this.Controls.Add(this.tabCenter);
            this.Controls.Add(this.splitter4);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.tabLeft);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.tabRight);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlPlot);
            this.Controls.Add(this.pnlTime);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SiteView";
            this.Text = "Lunar Candidate Landing Site Viewer";
            this.Load += new System.EventHandler(this.SiteView_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlTime.ResumeLayout(false);
            this.pnlTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSelectTime)).EndInit();
            this.tabRight.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabLeft.ResumeLayout(false);
            this.tabLayers.ResumeLayout(false);
            this.tabLayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTransparency)).EndInit();
            this.tabCenter.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblLatLon;
        private System.Windows.Forms.Panel pnlTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbCurrentTime;
        private System.Windows.Forms.ComboBox cbTimeSpan;
        private System.Windows.Forms.CheckBox cbUpdateContinuously;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtStartTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar tbSelectTime;
        private System.Windows.Forms.Panel pnlPlot;
        private System.Windows.Forms.TabControl tabRight;
        private System.Windows.Forms.TabPage tabProperties;
        private System.Windows.Forms.Panel pnlPropertiesHolder;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.TabControl tabLeft;
        private System.Windows.Forms.TabPage tabLayers;
        internal System.Windows.Forms.ListView lvLayers;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colTransparency;
        private System.Windows.Forms.TrackBar tbTransparency;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.TabControl tabCenter;
        private System.Windows.Forms.TabPage tabMap;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsPanelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem legendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem plotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlsToolStripMenuItem;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem siteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem test1ToolStripMenuItem;
    }
}

