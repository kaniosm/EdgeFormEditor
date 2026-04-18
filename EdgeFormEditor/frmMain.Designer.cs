namespace EdgeFormEditor
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblNameFilter;
        private System.Windows.Forms.TextBox txtNameFilter;
        private System.Windows.Forms.Label lblValueFilter;
        private System.Windows.Forms.TextBox txtValueFilter;
        private System.Windows.Forms.DataGridView dgvAutofill;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.Button btnSaveChanges;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblNameFilter = new Label();
            txtNameFilter = new TextBox();
            lblValueFilter = new Label();
            txtValueFilter = new TextBox();
            dgvAutofill = new DataGridView();
            btnDeleteSelected = new Button();
            btnSaveChanges = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvAutofill).BeginInit();
            SuspendLayout();
            
            lblNameFilter.AutoSize = true;
            lblNameFilter.Location = new Point(12, 15);
            lblNameFilter.Name = "lblNameFilter";
            lblNameFilter.Size = new Size(42, 15);
            lblNameFilter.Text = "Name:";
            
            txtNameFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtNameFilter.Location = new Point(60, 12);
            txtNameFilter.Name = "txtNameFilter";
            txtNameFilter.Size = new Size(300, 23);
            txtNameFilter.TextChanged += FilterTextChanged;
            
            lblValueFilter.AutoSize = true;
            lblValueFilter.Location = new Point(380, 15);
            lblValueFilter.Name = "lblValueFilter";
            lblValueFilter.Size = new Size(38, 15);
            lblValueFilter.Text = "Value:";
            
            txtValueFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtValueFilter.Location = new Point(424, 12);
            txtValueFilter.Name = "txtValueFilter";
            txtValueFilter.Size = new Size(170, 23);
            txtValueFilter.TextChanged += FilterTextChanged;
            
            btnDeleteSelected.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteSelected.Location = new Point(600, 11);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(90, 25);
            btnDeleteSelected.Text = "Delete Row";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            btnDeleteSelected.Click += btnDeleteSelected_Click;
            
            btnSaveChanges.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSaveChanges.Enabled = false;
            btnSaveChanges.Location = new Point(698, 11);
            btnSaveChanges.Name = "btnSaveChanges";
            btnSaveChanges.Size = new Size(90, 25);
            btnSaveChanges.Text = "Save";
            btnSaveChanges.UseVisualStyleBackColor = true;
            btnSaveChanges.Click += btnSaveChanges_Click;
            
            dgvAutofill.AllowUserToAddRows = false;
            dgvAutofill.AllowUserToDeleteRows = false;
            dgvAutofill.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvAutofill.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAutofill.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAutofill.Location = new Point(12, 50);
            dgvAutofill.MultiSelect = false;
            dgvAutofill.Name = "dgvAutofill";
            dgvAutofill.ReadOnly = true;
            dgvAutofill.RowHeadersVisible = false;
            dgvAutofill.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAutofill.Size = new Size(776, 388);
            dgvAutofill.ColumnHeaderMouseClick += dgvAutofill_ColumnHeaderMouseClick;
            dgvAutofill.RowPrePaint += dgvAutofill_RowPrePaint;
            dgvAutofill.KeyDown += dgvAutofill_KeyDown;
            
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnSaveChanges);
            Controls.Add(btnDeleteSelected);
            Controls.Add(dgvAutofill);
            Controls.Add(txtValueFilter);
            Controls.Add(lblValueFilter);
            Controls.Add(txtNameFilter);
            Controls.Add(lblNameFilter);
            MinimumSize = new Size(816, 489);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Edge Autofill Viewer";
            Load += frmMain_Load;
            ((System.ComponentModel.ISupportInitialize)dgvAutofill).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
