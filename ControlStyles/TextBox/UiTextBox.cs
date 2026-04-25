using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace ControlStyles
{
    public class UiTextBox : TextBox
    {
        protected override Type StyleKeyOverride => typeof(TextBox);

        public UiTextBox()
        {
            this.KeyDown += OnKeyDown;
        }

        #region Properties

        public static readonly StyledProperty<bool> MoveToNextOnEnterProperty =
            AvaloniaProperty.Register<UiTextBox, bool>(nameof(MoveToNextOnEnter), defaultValue: true);

        public bool MoveToNextOnEnter
        {
            get => GetValue(MoveToNextOnEnterProperty);
            set => SetValue(MoveToNextOnEnterProperty, value);
        }
        #endregion

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;

                if (MoveToNextOnEnter) MoveFocusToNext();
               
                
            }
        }

        private void MoveFocusToNext()
        {
            var root = TopLevel.GetTopLevel(this) ?? (Visual)Parent;
            if (root == null) return;

            var textBoxes = root.GetVisualDescendants()
                                .OfType<TextBox>()
                                .Where(tb => tb.IsVisible && tb.IsEnabled && tb.Focusable)
                                .OrderBy(tb => tb.TabIndex)
                                .ToList();

            var currentIndex = textBoxes.IndexOf(this);

            if (currentIndex != -1)
            {
                TextBox nextTarget;

                if (currentIndex < textBoxes.Count - 1)
                {
                    // C'è un prossimo TextBox
                    nextTarget = textBoxes[currentIndex + 1];
                }
                else
                {
                    // Siamo all'ultimo, torna al primo
                    nextTarget = textBoxes[0];
                }

                // Sposta il focus e seleziona il testo
                nextTarget.Focus();
                nextTarget.SelectAll();
            }
        }

        private static IEnumerable<UiTextBox> GetAllTextBoxes(IInputElement container)
        {
            if (container is UiTextBox textBox)
                yield return textBox;
            if (container is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    foreach (var childTextBox in GetAllTextBoxes(child))
                        yield return childTextBox;
                }
            }
            else if (container is ContentControl contentControl && contentControl.Content is IInputElement content)
            {
                foreach (var childTextBox in GetAllTextBoxes(content))
                    yield return childTextBox;
            }
            else if (container is Decorator decorator && decorator.Child != null)
            {
                foreach (var childTextBox in GetAllTextBoxes(decorator.Child))
                    yield return childTextBox;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            this.KeyDown -= OnKeyDown;
            base.OnDetachedFromVisualTree(e);
        }
    }
}
