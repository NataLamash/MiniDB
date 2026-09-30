using MiniDB.Core.DataTypes;
using MiniDB.Core.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace MiniDB.Desktop;

public partial class CreateTableWindow : Window
{
    private readonly ObservableCollection<ColumnDraft>
        _columns = new();

    public Table ResultTable { get; private set; } =
        new();

    public CreateTableWindow()
    {
        InitializeComponent();

        ColumnsDataGrid.ItemsSource = _columns;

        TypeColumn.ItemsSource =
            DataTypeFactory.SupportedTypes;

        _columns.Add(
            new ColumnDraft
            {
                Name = "Id",
                TypeName = "integer"
            });
    }

    private void AddColumn_Click(
        object sender,
        RoutedEventArgs e)
    {
        _columns.Add(
            new ColumnDraft
            {
                Name = "",
                TypeName = "string"
            });
    }

    private void DeleteColumn_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (ColumnsDataGrid.SelectedItem
            is ColumnDraft column)
        {
            _columns.Remove(column);
        }
    }

    private void Create_Click(
        object sender,
        RoutedEventArgs e)
    {
        ColumnsDataGrid.CommitEdit(
            DataGridEditingUnit.Cell,
            true);

        ColumnsDataGrid.CommitEdit(
            DataGridEditingUnit.Row,
            true);

        string tableName =
            TableNameTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(tableName))
        {
            MessageBox.Show(
                "Введіть назву таблиці.");

            return;
        }

        if (_columns.Count == 0)
        {
            MessageBox.Show(
                "Таблиця повинна містити хоча б одну колонку.");

            return;
        }

        if (_columns.Any(c =>
                string.IsNullOrWhiteSpace(c.Name)))
        {
            MessageBox.Show(
                "Усі колонки повинні мати назви.");

            return;
        }

        bool duplicateNames =
            _columns
                .GroupBy(
                    c => c.Name.Trim(),
                    StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1);

        if (duplicateNames)
        {
            MessageBox.Show(
                "Назви колонок повинні бути унікальними.");

            return;
        }

        var table = new Table(tableName);

        foreach (var draft in _columns)
        {
            table.AddColumn(
                new Column(
                    draft.Name.Trim(),
                    draft.TypeName));
        }

        ResultTable = table;

        DialogResult = true;
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}

public class ColumnDraft
{
    public string Name { get; set; } =
        string.Empty;

    public string TypeName { get; set; } =
        "string";
}