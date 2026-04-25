using Avalonia.Controls;
using Avalonia.Input;

namespace ControlStyles
{
    public class UiIntegerTextBox : UiTextBox
    {
        protected override Type StyleKeyOverride => typeof(TextBox);

        protected override void OnTextInput(TextInputEventArgs e)
        {
            // Se il testo in entrata contiene caratteri NON numerici, blocca l'evento
            if (!string.IsNullOrEmpty(e.Text) && !e.Text.All(char.IsDigit))
            {
                e.Handled = true;
                return;
            }

            base.OnTextInput(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Blocca la barra spaziatrice (che il TextInput a volte non intercetta)
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }

            // Esegue la logica della classe base (incluso lo spostamento focus su Invio)
            base.OnKeyDown(e);
        }

        
    }
}
