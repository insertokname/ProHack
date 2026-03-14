using Infrastructure.Il2Cpp;
using Infrastructure.Il2Cpp.Core;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Presentation
{
    public partial class MapScreenshotForm : Form
    {
        private readonly PROIl2CppManager _proIl2CppManager;
        private Bitmap? _image;
        // No MemoryStream needed — Bitmap is built directly from RGBA bytes via LockBits.

        public MapScreenshotForm(PROIl2CppManager proIl2CppManager)
        {
            InitializeComponent();
            _proIl2CppManager = proIl2CppManager;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void MapScreenshotForm_Load(object sender, EventArgs e)
        {
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_image is null)
                return;

            using var dialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                DefaultExt = "png",
                FileName = "map_screenshot.png",
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _image.Save(dialog.FileName, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to save image:\n{ex.Message}",
                        "Save Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearImage();
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button2.Visible = false;
            button3.Enabled = false;
            button3.Visible = false;
            ClearImage();

            try
            {
                var mode = checkBox1.Checked ? ScreenshotMode.Normal : ScreenshotMode.LowRes;
                var res = await _proIl2CppManager.CaptureMapScreenshotAsync(mode);

                // Build Bitmap directly from raw RGBA bytes — no PNG decode needed.
                // GDI+ Format32bppArgb is stored as B,G,R,A in memory, so swap R↔B first.
                var rawBytes = res.RgbaData;
                for (int i = 0; i < rawBytes.Length; i += 4)
                {
                    (rawBytes[i], rawBytes[i + 2]) = (rawBytes[i + 2], rawBytes[i]);
                }

                var bmp = new Bitmap(res.ImgWidth, res.ImgHeight, PixelFormat.Format32bppArgb);
                var bmpData = bmp.LockBits(
                    new Rectangle(0, 0, res.ImgWidth, res.ImgHeight),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                Marshal.Copy(rawBytes, 0, bmpData.Scan0, rawBytes.Length);
                bmp.UnlockBits(bmpData);

                _image = bmp;
                pictureBox1.Image = _image;

                button2.Enabled = true;
                button2.Visible = true;
                button3.Enabled = true;
                button3.Visible = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"MapScreenshot error: {ex}");
                MessageBox.Show(
                    $"Failed to capture map screenshot:\n\n{ex.Message}",
                    "Screenshot Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private void ClearImage()
        {
            pictureBox1.Image = null;
            _image?.Dispose();
            _image = null;

            button2.Enabled = false;
            button2.Visible = false;
            button3.Enabled = false;
            button3.Visible = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
