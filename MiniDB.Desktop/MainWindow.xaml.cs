using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using MiniDB.Core.Models;
using MiniDB.Core.Services;

using MiniDatabase = MiniDB.Core.Models.Database;
using MiniTable = MiniDB.Core.Models.Table;

namespace MiniDB.Desktop;

public partial class MainWindow : Window
{
    private MiniDatabase? _database;

    private readonly DatabaseSerializer _serializer = new();
    private readonly Validator _validator = new();
    private readonly ColumnSchemaService _columnSchemaService = new();

    private string? _currentFilePath;

    private int _selectedColumnIndex = -1;

    public MainWindow()
    {
        InitializeComponent();
        UpdateInterface();
    }

    private MiniTable? SelectedTable
    {
        get
        {
            if (_database == null ||
                TablesListBox.SelectedItem == null)
            {
                return null;
            }

            string tableName =
                TablesListBox.SelectedItem.ToString()!;

            return _database.GetTable(tableName);
        }
    }

    private void TableDataGrid_CurrentCellChanged(
    object? sender,
    EventArgs e)
    {
        if (TableDataGrid.CurrentCell.Column != null)
        {
            _selectedColumnIndex =
                TableDataGrid.CurrentCell.Column.DisplayIndex;
        }
    }

    // =============================
    // DATABASE
    // =============================

    private void NewDatabase_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new TextInputWindow(
            "Нова база даних",
            "Введіть назву бази даних:");

        if (dialog.ShowDialog() != true)
            return;

        string name = dialog.Result.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(
                "Назва бази даних не може бути порожньою.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        _database = new MiniDatabase(name);
        _currentFilePath = null;

        UpdateInterface();
    }

    private void OpenDatabase_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Відкрити базу даних",
            Filter = "MiniDB JSON (*.json)|*.json|Усі файли (*.*)|*.*"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            _database = _serializer.Load(dialog.FileName);
            _currentFilePath = dialog.FileName;

            UpdateInterface();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося відкрити базу даних.\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void SaveDatabase_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_database == null)
        {
            MessageBox.Show(
                "Спочатку створіть або відкрийте базу даних.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        string? path = _currentFilePath;

        if (path == null)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Зберегти базу даних",
                Filter = "MiniDB JSON (*.json)|*.json",
                FileName = $"{_database.Name}.json"
            };

            if (dialog.ShowDialog() != true)
                return;

            path = dialog.FileName;
        }

        try
        {
            _serializer.Save(_database, path);
            _currentFilePath = path;

            MessageBox.Show(
                "Базу даних успішно збережено.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка збереження:\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // =============================
    // TABLES
    // =============================

    private void CreateTable_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (_database == null)
        {
            MessageBox.Show(
                "Спочатку створіть базу даних.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var dialog = new CreateTableWindow();

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            _database.CreateTable(dialog.ResultTable);

            UpdateTablesList();

            TablesListBox.SelectedItem =
                dialog.ResultTable.Name;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private void DeleteTable_Click(
        object sender,
        RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null || _database == null)
        {
            MessageBox.Show(
                "Оберіть таблицю.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var result = MessageBox.Show(
            $"Видалити таблицю '{table.Name}'?",
            "Підтвердження",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        _database.DeleteTable(table.Name);

        UpdateTablesList();
        ClearTableView();
    }

    private void TablesListBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        RefreshTableView();
    }

    // =============================
    // ROW OPERATIONS
    // =============================

    private void AddRow_Click(
    object sender,
    RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null)
        {
            MessageBox.Show(
                "Спочатку оберіть таблицю.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var dialog = new RowEditorWindow(table)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            if (!_validator.ValidateRow(
                    dialog.ResultRow,
                    table))
            {
                MessageBox.Show(
                    "Рядок містить некоректні дані.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            table.AddRow(dialog.ResultRow);

            RefreshTableView();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void EditRow_Click(
    object sender,
    RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null)
        {
            MessageBox.Show(
                "Спочатку оберіть таблицю.");

            return;
        }

        int index = TableDataGrid.SelectedIndex;

        if (index < 0 ||
            index >= table.Rows.Count)
        {
            MessageBox.Show(
                "Оберіть рядок для редагування.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var currentRow = table.Rows[index];

        var dialog =
            new RowEditorWindow(
                table,
                currentRow)
            {
                Owner = this
            };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            table.EditRow(
                index,
                dialog.ResultRow);

            RefreshTableView();

            TableDataGrid.SelectedIndex = index;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void DeleteRow_Click(
    object sender,
    RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null)
        {
            MessageBox.Show(
                "Спочатку оберіть таблицю.");

            return;
        }

        int index = TableDataGrid.SelectedIndex;

        if (index < 0 ||
            index >= table.Rows.Count)
        {
            MessageBox.Show(
                "Оберіть рядок для видалення.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var result = MessageBox.Show(
            "Видалити вибраний рядок?",
            "Підтвердження",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        table.DeleteRow(index);

        RefreshTableView();
    }

    // =============================
    // INDIVIDUAL OPERATION
    // =============================

    private void RenameColumn_Click(
    object sender,
    RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null)
        {
            MessageBox.Show(
                "Спочатку оберіть таблицю.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (_selectedColumnIndex < 0 ||
            _selectedColumnIndex >= table.Columns.Count)
        {
            MessageBox.Show(
                "Клацніть по будь-якій комірці потрібної колонки.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        var column =
            table.Columns[_selectedColumnIndex];

        var dialog = new TextInputWindow(
            "Перейменування колонки",
            $"Нова назва для '{column.Name}':",
            column.Name)
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        string newName =
            dialog.Result.Trim();

        try
        {
            _columnSchemaService.RenameColumn(
                table,
                column.Name,
                newName);

            RefreshTableView();

            _selectedColumnIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
    private void MoveColumn_Click(
    object sender,
    RoutedEventArgs e)
    {
        var table = SelectedTable;

        if (table == null)
        {
            MessageBox.Show(
                "Спочатку оберіть таблицю.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        if (_selectedColumnIndex < 0 ||
            _selectedColumnIndex >= table.Columns.Count)
        {
            MessageBox.Show(
                "Клацніть по будь-якій комірці потрібної колонки.",
                "MiniDB",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        int oldIndex = _selectedColumnIndex;

        var column =
            table.Columns[oldIndex];

        var dialog = new TextInputWindow(
            "Перестановка колонки",
            $"Нова позиція для '{column.Name}' " +
            $"(від 1 до {table.Columns.Count}):",
            (oldIndex + 1).ToString())
        {
            Owner = this
        };

        if (dialog.ShowDialog() != true)
            return;

        if (!int.TryParse(
                dialog.Result,
                out int newPosition))
        {
            MessageBox.Show(
                "Позиція повинна бути цілим числом.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (newPosition < 1 ||
            newPosition > table.Columns.Count)
        {
            MessageBox.Show(
                $"Введіть число від 1 до {table.Columns.Count}.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        int newIndex =
            newPosition - 1;

        try
        {
            _columnSchemaService.MoveColumn(
                table,
                oldIndex,
                newIndex);

            RefreshTableView();

            _selectedColumnIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    // =============================
    // UI
    // =============================

    private void UpdateInterface()
    {
        if (_database == null)
        {
            DatabaseNameText.Text = "База не створена";

            TablesListBox.ItemsSource = null;

            ClearTableView();

            return;
        }

        DatabaseNameText.Text =
            $"База: {_database.Name}";

        UpdateTablesList();
    }

    private void UpdateTablesList()
    {
        if (_database == null)
        {
            TablesListBox.ItemsSource = null;
            return;
        }

        string? selected =
            TablesListBox.SelectedItem?.ToString();

        TablesListBox.ItemsSource = null;

        TablesListBox.ItemsSource =
            _database.Tables
                .Select(t => t.Name)
                .ToList();

        if (selected != null &&
            _database.GetTable(selected) != null)
        {
            TablesListBox.SelectedItem = selected;
        }
    }

    private void RefreshTableView()
    {
        var table = SelectedTable;

        if (table == null)
        {
            ClearTableView();
            return;
        }

        CurrentTableText.Text = table.Name;

        var dataTable = new DataTable();

        foreach (var column in table.Columns)
        {
            dataTable.Columns.Add(
                column.Name,
                typeof(string));
        }

        foreach (var row in table.Rows)
        {
            var dataRow = dataTable.NewRow();

            for (int i = 0;
                 i < table.Columns.Count;
                 i++)
            {
                dataRow[i] =
                    i < row.Values.Count
                        ? row.Values[i]
                        : string.Empty;
            }

            dataTable.Rows.Add(dataRow);
        }

        TableDataGrid.ItemsSource =
            dataTable.DefaultView;
    }

    private void ClearTableView()
    {
        CurrentTableText.Text =
            "Оберіть таблицю";

        TableDataGrid.ItemsSource = null;
    }
}