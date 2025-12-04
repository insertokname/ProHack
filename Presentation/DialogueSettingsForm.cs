using Infrastructure.Memory;

namespace Presentation
{
    public partial class DialogueSettingsForm : Form
    {
        private readonly PROMemoryManager _proMemoryManager;
        public DialogueSettingsForm(PROMemoryManager memoryManager)
        {
            _proMemoryManager = memoryManager;
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateCurValue();
            _proMemoryManager.TextSpeed = IntToFloatTextSpeed(trackBar1.Value);
        }

        private void UpdateCurValue()
        {
            label1.Text = $"Current Value: {IntToFloatTextSpeed(trackBar1.Value)}";
        }

        private void DialogueSettings_Load(object sender, EventArgs e)
        {
            trackBar1.Value = FloatToIntTextSpeed(_proMemoryManager.TextSpeed);
            UpdateCurValue();
        }

        private static int FloatToIntTextSpeed(float value)
            => (int)(value * 100);

        private static float IntToFloatTextSpeed(int value)
            => value / 100.0f;
    }
}
