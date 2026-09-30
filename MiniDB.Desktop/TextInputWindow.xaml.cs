using System.Windows;

namespace MiniDB.Desktop;

public partial class TextInputWindow : Window
{
    public string Result =>
        InputTextBox.Text;

    public TextInputWindow(
        string title,
        string prompt,
        string initialValue = "")
    {
        InitializeComponent();

        Title = title;
        PromptText.Text = prompt;
        InputTextBox.Text = initialValue;

        Loaded += (_, _) =>
        {
            InputTextBox.Focus();
            InputTextBox.SelectAll();
        };
    }

    private void Ok_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
    }
}