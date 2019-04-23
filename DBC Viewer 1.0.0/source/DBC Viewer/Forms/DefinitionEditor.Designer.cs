namespace DBCViewer
{
    partial class DefinitionEditor
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
            this.editorDataGridView = new System.Windows.Forms.DataGridView();
            this.IndexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IsIndexColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ArraySizeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FormatColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.doneButton = new System.Windows.Forms.Button();
            this.buildTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.editorDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // editorDataGridView
            // 
            this.editorDataGridView.AllowDrop = true;
            this.editorDataGridView.AllowUserToResizeRows = false;
            this.editorDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.editorDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.editorDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndexColumn,
            this.NameColumn,
            this.TypeColumn,
            this.IsIndexColumn,
            this.ArraySizeColumn,
            this.FormatColumn});
            this.editorDataGridView.Location = new System.Drawing.Point(12, 12);
            this.editorDataGridView.MultiSelect = false;
            this.editorDataGridView.Name = "editorDataGridView";
            this.editorDataGridView.Size = new System.Drawing.Size(682, 429);
            this.editorDataGridView.TabIndex = 0;
            this.editorDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.editorDataGridView_CellValueChanged);
            this.editorDataGridView.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.editorDataGridView_DefaultValuesNeeded);
            this.editorDataGridView.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.editorDataGridView_UserAddedRow);
            this.editorDataGridView.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.editorDataGridView_UserDeletedRow);
            this.editorDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.editorDataGridView_DragDrop);
            this.editorDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.editorDataGridView_DragEnter);
            this.editorDataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.editorDataGridView_MouseDown);
            // 
            // IndexColumn
            // 
            this.IndexColumn.DataPropertyName = "Index";
            this.IndexColumn.HeaderText = "Index";
            this.IndexColumn.Name = "IndexColumn";
            this.IndexColumn.ReadOnly = true;
            this.IndexColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // NameColumn
            // 
            this.NameColumn.DataPropertyName = "Name";
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // TypeColumn
            // 
            this.TypeColumn.DataPropertyName = "Type";
            this.TypeColumn.HeaderText = "Type";
            this.TypeColumn.Items.AddRange(new object[] {
            "long",
            "ulong",
            "int",
            "uint",
            "short",
            "ushort",
            "sbyte",
            "byte",
            "float",
            "double",
            "string"});
            this.TypeColumn.MaxDropDownItems = 11;
            this.TypeColumn.Name = "TypeColumn";
            // 
            // IsIndexColumn
            // 
            this.IsIndexColumn.DataPropertyName = "IsIndex";
            this.IsIndexColumn.HeaderText = "IsIndex";
            this.IsIndexColumn.Name = "IsIndexColumn";
            // 
            // ArraySizeColumn
            // 
            this.ArraySizeColumn.DataPropertyName = "ArraySize";
            this.ArraySizeColumn.HeaderText = "Array Size";
            this.ArraySizeColumn.Name = "ArraySizeColumn";
            this.ArraySizeColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FormatColumn
            // 
            this.FormatColumn.DataPropertyName = "Format";
            this.FormatColumn.HeaderText = "Format";
            this.FormatColumn.Name = "FormatColumn";
            this.FormatColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.doneButton.Location = new System.Drawing.Point(12, 447);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 1;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // buildTextBox
            // 
            this.buildTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buildTextBox.Location = new System.Drawing.Point(132, 449);
            this.buildTextBox.Name = "buildTextBox";
            this.buildTextBox.Size = new System.Drawing.Size(100, 20);
            this.buildTextBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 452);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Build:";
            // 
            // DefinitionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 482);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buildTextBox);
            this.Controls.Add(this.doneButton);
            this.Controls.Add(this.editorDataGridView);
            this.Name = "DefinitionEditor";
            this.Text = "Definition Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DefinitionEditorNew_FormClosing);
            this.Load += new System.EventHandler(this.DefinitionEditorNew_Load);
            ((System.ComponentModel.ISupportInitialize)(this.editorDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView editorDataGridView;
        private System.Windows.Forms.Button doneButton;
        private System.Windows.Forms.TextBox buildTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsIndexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ArraySizeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn FormatColumn;
    }
}