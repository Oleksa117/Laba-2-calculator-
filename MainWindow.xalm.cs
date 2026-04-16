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


        // Клавіатура
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Цифри
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = (e.Key - Key.D0).ToString();
                HandleNumberInput(digit);
            }

            // NumPad
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = (e.Key - Key.NumPad0).ToString();
                HandleNumberInput(digit);
            }

            // Крапка
            if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                HandleNumberInput(".");
            }

            // Операції
            if (e.Key == Key.Add || e.Key == Key.OemPlus)
                HandleOperation("+");

            if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                HandleOperation("-");

            if (e.Key == Key.Multiply)
                HandleOperation("×");

            if (e.Key == Key.Divide)
                HandleOperation("÷");

            // Enter (=)
            if (e.Key == Key.Enter)
                Equals_Click(null, null);

            // Backspace (видаляє останній символ)
            if (e.Key == Key.Back && currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                Display.Text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
            }

            // Escape (повне очищення)
            if (e.Key == Key.Escape)
                Clear_Click(null, null);
        }

        // Clear (C)
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            currentValue = 0;
            currentInput = "";
            currentOperation = "";

            Display.Text = "0";
        }

        private Stack<Command> history = new Stack<Command>();

        // =
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentInput)) return;

            double operand = double.Parse(currentInput);
            Command cmd = null;

            switch (currentOperation)
            {
                case "+":
                    cmd = new AddCommand(currentValue, operand);
                    break;
                case "-":
                    cmd = new SubCommand(currentValue, operand);
                    break;
                case "×":
                    cmd = new MulCommand(currentValue, operand);
                    break;
                case "÷":
                    if (operand == 0)
                    {
                        MessageBox.Show("Ділення на нуль!");
                        return;
                    }
                    cmd = new DivCommand(currentValue, operand);
                    break;
            }

            if (cmd != null)
            {
                cmd.Execute();
                history.Push(cmd);

                currentValue = cmd.Result;
                Display.Text = currentValue.ToString();
                currentInput = "";
            }
        }

        // Undo (CE)
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (history.Count > 0)
            {
                var cmd = history.Pop();
                currentValue = cmd.Previous;

                Display.Text = currentValue.ToString();
                currentInput = "";
            }
        }

        // Command Pattern
        public abstract class Command
        {
            public double Previous { get; protected set; }
            public double Result { get; protected set; }
            public abstract void Execute();
        }

        public class AddCommand : Command
        {
            private double a, b;
            public AddCommand(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            public override void Execute()
            {
                Previous = a;
                Result = a + b;
            }
        }

        public class SubCommand : Command
        {
            private double a, b;
            public SubCommand(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            public override void Execute()
            {
                Previous = a;
                Result = a - b;
            }
        }

        public class MulCommand : Command
        {
            private double a, b;
            public MulCommand(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            public override void Execute()
            {
                Previous = a;
                Result = a * b;
            }
        }

        public class DivCommand : Command
        {
            private double a, b;
            public DivCommand(double a, double b)
            {
                this.a = a;
                this.b = b;
            }
            public override void Execute()
            {
                Previous = a;
                Result = a / b;
            }
        }
    }
}