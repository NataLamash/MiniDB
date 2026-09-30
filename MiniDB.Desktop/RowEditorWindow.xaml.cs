using System.Windows;
using System.Windows.Controls;
using MiniDB.Core.Models;
using MiniDB.Core.Services;

namespace MiniDB.Desktop;

public partial class RowEditorWindow : Window
{
    private readonly Table _table;
    private readonly Validator _validator = new();

    private readonly List<TextBox> _textBoxes = new();

    public Row ResultRow { get; private set; } = new();

    public RowEditorWindow(
        Table table,
        Row? existingRow = null)
    {
        InitializeComponent();

        _table = table;

        Title = existingRow == null
            ? "Додавання рядка"
            : "Редагування рядка";

        TitleText.Text = Title;

        CreateFields(existingRow);
    }

    private void CreateFields(Row? existingRow)
    {
        for (int i = 0; i < _table.Columns.Count; i++)
        {
            var column = _table.Columns[i];

            var container = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 15)
            };

            var label = new TextBlock
            {
                Text = $"{column.Name} ({column.TypeName})",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var textBox = new TextBox
            {
                Height = 34,
                FontSize = 15,
                VerticalContentAlignment =
                    VerticalAlignment.Center
            };

            if (existingRow != null &&
                i < existingRow.Values.Count)
            {
                textBox.Text =
                    existingRow.Values[i];
            }

            container.Children.Add(label);
            container.Children.Add(textBox);

            // Підказки для наших спеціальних типів
            if (column.TypeName
                .Equals(
                    "color",
                    StringComparison.OrdinalIgnoreCase))
            {
                container.Children.Add(
                    new TextBlock
                    {
                        Text = "Формат: #RRGGBB, наприклад #FF0000",
                        Foreground =
                            System.Windows.Media.Brushes.Gray,
                        FontSize = 12,
                        Margin = new Thickness(0, 4, 0, 0)
                    });
            }

            if (column.TypeName
                .Equals(
                    "colorInvl",
                    StringComparison.OrdinalIgnoreCase))
            {
                container.Children.Add(
                    new TextBlock
                    {
                        Text =
                            "Формат: #000000..#FFFFFF",
                        Foreground =
                            System.Windows.Media.Brushes.Gray,
                        FontSize = 12,
                        Margin = new Thickness(0, 4, 0, 0)
                    });
            }

            FieldsPanel.Children.Add(container);

            _textBoxes.Add(textBox);
        }
    }

    private void Save_Click(
        object sender,
        RoutedEventArgs e)
    {
        var values =
            _textBoxes
                .Select(t => t.Text.Trim())
                .ToList();

        var row = new Row(values);

        for (int i = 0; i < _table.Columns.Count; i++)
        {
            var column = _table.Columns[i];

            if (!_validator.ValidateValue(
                    row.Values[i],
                    column))
            {
                MessageBox.Show(
                    $"Некоректне значення для поля " +
                    $"'{column.Name}'.\n\n" +
                    $"Тип даних: {column.TypeName}\n" +
                    $"Значення: {row.Values[i]}",
                    "Помилка валідації",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                _textBoxes[i].Focus();
                _textBoxes[i].SelectAll();

                return;
            }
        }

        ResultRow = row;

        DialogResult = true;
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}