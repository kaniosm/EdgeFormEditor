using EdgeFormEditor.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace EdgeFormEditor
{
    public partial class frmMain : Form
    {
        private readonly string _webDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Microsoft",
            "Edge",
            "User Data",
            "Default",
            "Web Data");

        private readonly HashSet<string> _pendingDeleteKeys = new(StringComparer.Ordinal);
        private List<AutofillRow> _currentItems = [];
        private string _sortColumn = nameof(AutofillRow.Name);
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        public frmMain()
        {
            InitializeComponent();
        }

        private async void frmMain_Load(object? sender, EventArgs e)
        {
            var proceed = await WarnAndForceCloseEdgeAsync();
            if (!proceed)
            {
                Close();
                return;
            }

            await LoadAutofillAsync();
            UpdateSaveButtonState();
        }

        private async Task<bool> WarnAndForceCloseEdgeAsync()
        {
            var result = MessageBox.Show(
                $"This app needs to close Microsoft Edge to edit autofill data.{Environment.NewLine}{Environment.NewLine}All Edge windows/processes will be force-closed now (taskkill /f /im msedge.exe).{Environment.NewLine}{Environment.NewLine}Continue?",
                "Close Edge Required",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning);

            if (result != DialogResult.OK)
            {
                return false;
            }

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = "/f /im msedge.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using var process = Process.Start(startInfo);
                if (process is not null)
                {
                    await process.WaitForExitAsync();
                }

                await Task.Delay(300);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to close Microsoft Edge automatically.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                    "Close Edge Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        private async void FilterTextChanged(object? sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            await LoadAutofillAsync();
        }

        private async Task LoadAutofillAsync()
        {
            if (!File.Exists(_webDataPath))
            {
                MessageBox.Show(
                    $"Could not find the Edge Web Data database at:{Environment.NewLine}{_webDataPath}",
                    "Database Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                UseWaitCursor = true;
                Enabled = false;

                var options = new DbContextOptionsBuilder<WebDataDbContext>()
                    .UseSqlite($"Data Source={_webDataPath};Mode=ReadOnly")
                    .Options;

                await using var dbContext = new WebDataDbContext(options);
                var query = dbContext.Autofills.AsNoTracking();

                var nameFilter = txtNameFilter.Text.Trim();
                if (!string.IsNullOrWhiteSpace(nameFilter))
                {
                    var loweredName = nameFilter.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(loweredName));
                }

                var valueFilter = txtValueFilter.Text.Trim();
                if (!string.IsNullOrWhiteSpace(valueFilter))
                {
                    var loweredValue = valueFilter.ToLower();
                    query = query.Where(x => x.ValueLower.Contains(loweredValue));
                }

                _currentItems = query.ToList()
                    .Select(x => new AutofillRow
                    {
                        Name = x.Name,
                        Value = x.Value,
                        Count = x.Count,
                        DateCreated = x.DateCreated,
                        DateLastUsed = x.DateLastUsed,
                        IsPendingDelete = _pendingDeleteKeys.Contains(CreateRowKey(x.Name, x.Value))
                    })
                    .ToList();

                BindGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load autofill entries.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void dgvAutofill_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

            var column = dgvAutofill.Columns[e.ColumnIndex];
            if (string.IsNullOrWhiteSpace(column.DataPropertyName))
            {
                return;
            }

            if (_sortColumn == column.DataPropertyName)
            {
                _sortDirection = _sortDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column.DataPropertyName;
                _sortDirection = ListSortDirection.Ascending;
            }

            BindGrid();
        }

        private void btnDeleteSelected_Click(object? sender, EventArgs e)
        {
            MarkSelectedRowsForDeletion();
        }

        private void dgvAutofill_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
            {
                return;
            }

            MarkSelectedRowsForDeletion();
            e.Handled = true;
        }

        private async void btnSaveChanges_Click(object? sender, EventArgs e)
        {
            await SaveChangesAsync();
        }

        private async Task SaveChangesAsync()
        {
            if (_pendingDeleteKeys.Count == 0)
            {
                return;
            }

            try
            {
                UseWaitCursor = true;
                Enabled = false;

                EnsureDatabaseIsWritable();

                var pendingDeletes = _pendingDeleteKeys
                    .Select(ParseRowKey)
                    .ToList();

                var options = new DbContextOptionsBuilder<WebDataDbContext>()
                    .UseSqlite($"Data Source={_webDataPath};Mode=ReadWrite")
                    .Options;

                await using var dbContext = new WebDataDbContext(options);

                foreach (var pending in pendingDeletes)
                {
                    var entity = await dbContext.Autofills.FindAsync(pending.Name, pending.Value);
                    if (entity is not null)
                    {
                        dbContext.Autofills.Remove(entity);
                    }
                }

                await dbContext.SaveChangesAsync();

                _pendingDeleteKeys.Clear();
                await LoadAutofillAsync();
                UpdateSaveButtonState();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 8)
            {
                MessageBox.Show(
                    $"Database is read-only. Close Microsoft Edge completely and try again.{Environment.NewLine}{Environment.NewLine}{_webDataPath}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to save changes.{Environment.NewLine}{Environment.NewLine}{ex.InnerException?.Message ?? ex.Message}",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void EnsureDatabaseIsWritable()
        {
            var fileInfo = new FileInfo(_webDataPath);
            if (fileInfo.IsReadOnly)
            {
                fileInfo.IsReadOnly = false;
            }
        }

        private void MarkSelectedRowsForDeletion()
        {
            if (dgvAutofill.SelectedRows.Count == 0)
            {
                return;
            }

            foreach (DataGridViewRow selectedRow in dgvAutofill.SelectedRows)
            {
                if (selectedRow.DataBoundItem is not AutofillRow item)
                {
                    continue;
                }

                item.IsPendingDelete = true;
                _pendingDeleteKeys.Add(CreateRowKey(item.Name, item.Value));
            }

            dgvAutofill.Invalidate();
            UpdateSaveButtonState();
        }

        private void BindGrid()
        {
            IEnumerable<AutofillRow> sortedItems = _currentItems;

            sortedItems = (_sortColumn, _sortDirection) switch
            {
                (nameof(AutofillRow.Name), ListSortDirection.Ascending) => sortedItems.OrderBy(x => x.Name),
                (nameof(AutofillRow.Name), ListSortDirection.Descending) => sortedItems.OrderByDescending(x => x.Name),
                (nameof(AutofillRow.Value), ListSortDirection.Ascending) => sortedItems.OrderBy(x => x.Value),
                (nameof(AutofillRow.Value), ListSortDirection.Descending) => sortedItems.OrderByDescending(x => x.Value),
                (nameof(AutofillRow.Count), ListSortDirection.Ascending) => sortedItems.OrderBy(x => x.Count),
                (nameof(AutofillRow.Count), ListSortDirection.Descending) => sortedItems.OrderByDescending(x => x.Count),
                (nameof(AutofillRow.DateCreated), ListSortDirection.Ascending) => sortedItems.OrderBy(x => x.DateCreated),
                (nameof(AutofillRow.DateCreated), ListSortDirection.Descending) => sortedItems.OrderByDescending(x => x.DateCreated),
                (nameof(AutofillRow.DateLastUsed), ListSortDirection.Ascending) => sortedItems.OrderBy(x => x.DateLastUsed),
                (nameof(AutofillRow.DateLastUsed), ListSortDirection.Descending) => sortedItems.OrderByDescending(x => x.DateLastUsed),
                _ => sortedItems.OrderBy(x => x.Name).ThenBy(x => x.Value)
            };

            dgvAutofill.DataSource = sortedItems.ToList();
            UpdateSortGlyph();
        }

        private void UpdateSortGlyph()
        {
            foreach (DataGridViewColumn column in dgvAutofill.Columns)
            {
                column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (string.IsNullOrWhiteSpace(_sortColumn) || !dgvAutofill.Columns.Contains(_sortColumn))
            {
                return;
            }

            dgvAutofill.Columns[_sortColumn].HeaderCell.SortGlyphDirection = _sortDirection == ListSortDirection.Ascending
                ? SortOrder.Ascending
                : SortOrder.Descending;
        }

        private void dgvAutofill_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var row = dgvAutofill.Rows[e.RowIndex];
            if (row.DataBoundItem is not AutofillRow item)
            {
                return;
            }

            if (item.IsPendingDelete)
            {
                row.DefaultCellStyle.BackColor = Color.MistyRose;
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
                row.DefaultCellStyle.SelectionBackColor = Color.LightCoral;
                row.DefaultCellStyle.SelectionForeColor = Color.White;
                return;
            }

            row.DefaultCellStyle.BackColor = dgvAutofill.DefaultCellStyle.BackColor;
            row.DefaultCellStyle.ForeColor = dgvAutofill.DefaultCellStyle.ForeColor;
            row.DefaultCellStyle.SelectionBackColor = dgvAutofill.DefaultCellStyle.SelectionBackColor;
            row.DefaultCellStyle.SelectionForeColor = dgvAutofill.DefaultCellStyle.SelectionForeColor;
        }

        private void UpdateSaveButtonState()
        {
            btnSaveChanges.Enabled = _pendingDeleteKeys.Count > 0;
        }

        private static string CreateRowKey(string name, string value) => $"{name}\u001F{value}";

        private static (string Name, string Value) ParseRowKey(string key)
        {
            var parts = key.Split('\u001F', 2);
            if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }

            return (parts[0], string.Empty);
        }

        private sealed class AutofillRow
        {
            public string Name { get; set; } = string.Empty;

            public string Value { get; set; } = string.Empty;

            public int? Count { get; set; }

            public int? DateCreated { get; set; }

            public int? DateLastUsed { get; set; }

            [NotMapped]
            public bool IsPendingDelete { get; set; }
        }
    }
}
