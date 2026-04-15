using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CalculatorWPF
{
    public partial class MainWindow : Window
    {
        private double currentValue = 0;
        private string currentInput = "";
        private string currentOperation = "";

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
            this.Focus();
        }

        // Ввід чисел
        private void HandleNumberInput(string value)
        {
            if (value == "." && currentInput.Contains(".")) return;

            currentInput += value;
            Display.Text = currentInput;
        }

        // Ввід з кнопок
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            HandleNumberInput(btn.Content.ToString());
        }

        // Вибір операції 
        private void HandleOperation(string op)
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                currentValue = double.Parse(currentInput);
                currentInput = "";
            }

            currentOperation = op;
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            HandleOperation(btn.Content.ToString());
        }
    }
}