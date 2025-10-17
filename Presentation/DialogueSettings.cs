using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Presentation
{
    public partial class DialogueSettings : Form
    {
        private readonly MemoryManager _memoryManager;
        public DialogueSettings(MemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateCurValue();
            _memoryManager.TextSpeed = IntToFloatTextSpeed(trackBar1.Value);
        }

        private void UpdateCurValue()
        {
            label1.Text = $"Current Value: {IntToFloatTextSpeed(trackBar1.Value)}";
        }

        private void DialogueSettings_Load(object sender, EventArgs e)
        {
            trackBar1.Value = FloatToIntTextSpeed(_memoryManager.TextSpeed);
            UpdateCurValue();
        }

        private static int FloatToIntTextSpeed(float value)
            => (int)(value * 100);

        private static float IntToFloatTextSpeed(int value)
            => value / 100.0f;
    }
}
