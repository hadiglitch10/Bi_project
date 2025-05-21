using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageTemplate
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            pictureBox2.Click += PictureBox2_Click;
            button1.Click += button1_Click;
        }

        RGBPixel[,] ImageMatrix;
        private int[,] segmentMap;
        private HashSet<int> mergedSegments = new HashSet<int>();
        private Bitmap originalSegmentedBitmap;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            REGION_FINDING.buildTheGraph(ImageMatrix);
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            double sigma = double.Parse(txtGaussSigma.Text);
            int maskSize = (int)nudMaskSize.Value;
            ImageMatrix = ImageOperations.GaussianFilter1D(ImageMatrix, maskSize, sigma);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);  

            REGION_FINDING.StartRegionFinding(50);
            label7.Text = REGION_FINDING.dsu.numof_sets.ToString();
            
            segmentMap = REGION_FINDING.GetSegmentMap(ImageMatrix);
            originalSegmentedBitmap = REGION_FINDING.VisualizeSegments(ImageMatrix);
            pictureBox2.Image = originalSegmentedBitmap;
        }


        private void PictureBox2_Click(object sender, EventArgs e)
        {
            if (segmentMap == null || ImageMatrix == null) return;

            MouseEventArgs me = (MouseEventArgs)e;
            Point clickLocation = new Point(me.X, me.Y);
            int selectedRegionId = segmentMap[clickLocation.Y, clickLocation.X];

            bool isCtrlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            
            if (isCtrlPressed)
            {
                ToggleSegmentSelection(selectedRegionId);
            }
            else
            {
                ResetAndSelectSegment(selectedRegionId);
            }

            UpdateSegmentationDisplay();
        }

        private void ToggleSegmentSelection(int segmentId)
        {
            if (mergedSegments.Contains(segmentId))
            {
                mergedSegments.Remove(segmentId);
            }
            else
            {
                mergedSegments.Add(segmentId);
            }
        }

        private void ResetAndSelectSegment(int segmentId)
        {
            mergedSegments.Clear();
            mergedSegments.Add(segmentId);
        }

        private void UpdateSegmentationDisplay()
        {
            pictureBox2.Image = originalSegmentedBitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateSegmentSelection()) return;

            Bitmap resultImage = CreateSegmentedImage();
            DisplayResultImage(resultImage);
        }

        private bool ValidateSegmentSelection()
        {
            if (mergedSegments.Count == 0)
            {
                MessageBox.Show("Please select at least one region to display!", 
                    "Selection Required", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                return false;
            }
            return true;
        }

        private Bitmap CreateSegmentedImage()
        {
            int imageWidth = ImageMatrix.GetLength(1);
            int imageHeight = ImageMatrix.GetLength(0);
            Bitmap resultImage = new Bitmap(imageWidth, imageHeight);

            for (int row = 0; row < imageHeight; row++)
            {
                for (int col = 0; col < imageWidth; col++)
                {
                    Color pixelColor = DeterminePixelColor(row, col);
                    resultImage.SetPixel(col, row, pixelColor);
                }
            }

            return resultImage;
        }

        private Color DeterminePixelColor(int row, int col)
        {
            int currentSegment = segmentMap[row, col];
            
            if (mergedSegments.Contains(currentSegment))
            {
                return Color.FromArgb(
                    ImageMatrix[row, col].red,
                    ImageMatrix[row, col].green,
                    ImageMatrix[row, col].blue
                );
            }
            
            return Color.White;
        }

        private void DisplayResultImage(Bitmap resultImage)
        {
            pictureBox2.Image = resultImage;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}