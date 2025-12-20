using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GymManager.WPF.Behaviors;

/// <summary>
/// Behavior para permitir solo entrada numerica en TextBox
/// Uso: behaviors:NumericOnlyBehavior.IsEnabled="True"
/// </summary>
public static class NumericOnlyBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(NumericOnlyBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj) =>
        (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value) =>
        obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += OnPreviewTextInput;
                textBox.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(textBox, OnPasting);
            }
            else
            {
                textBox.PreviewTextInput -= OnPreviewTextInput;
                textBox.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(textBox, OnPasting);
            }
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Solo permitir digitos
        e.Handled = !IsNumeric(e.Text);
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Permitir teclas de control (Backspace, Delete, Tab, etc.)
        if (e.Key == Key.Space)
        {
            e.Handled = true;
        }
    }

    private static void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!IsNumeric(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static bool IsNumeric(string text) =>
        Regex.IsMatch(text, @"^[0-9]+$");
}

/// <summary>
/// Behavior para permitir numeros y caracteres especiales de telefono
/// Uso: behaviors:PhoneNumberBehavior.IsEnabled="True"
/// </summary>
public static class PhoneNumberBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(PhoneNumberBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj) =>
        (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value) =>
        obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.PreviewTextInput += OnPreviewTextInput;
                DataObject.AddPastingHandler(textBox, OnPasting);
            }
            else
            {
                textBox.PreviewTextInput -= OnPreviewTextInput;
                DataObject.RemovePastingHandler(textBox, OnPasting);
            }
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Permitir digitos, +, -, (, ), espacios
        e.Handled = !IsValidPhoneChar(e.Text);
    }

    private static void OnPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!IsValidPhoneString(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static bool IsValidPhoneChar(string text) =>
        Regex.IsMatch(text, @"^[0-9+\-\(\)\s]+$");

    private static bool IsValidPhoneString(string text) =>
        Regex.IsMatch(text, @"^[0-9+\-\(\)\s]+$");
}

/// <summary>
/// Behavior para limpiar espacios dobles y trim automatico
/// Uso: behaviors:TextCleanupBehavior.IsEnabled="True"
/// </summary>
public static class TextCleanupBehavior
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(TextCleanupBehavior),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj) =>
        (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value) =>
        obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            if ((bool)e.NewValue)
            {
                textBox.LostFocus += OnLostFocus;
            }
            else
            {
                textBox.LostFocus -= OnLostFocus;
            }
        }
    }

    private static void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                // Eliminar espacios dobles y trim
                var cleaned = Regex.Replace(text.Trim(), @"\s+", " ");
                if (text != cleaned)
                {
                    textBox.Text = cleaned;
                }
            }
        }
    }
}