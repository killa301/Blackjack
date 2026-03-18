namespace blackjack
{
    partial class Form1
    {
        /// <summary>
        private List<Control> controlsToResize;
        private float originalFormWidth;
        private float originalFormHeight;
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            originalFormWidth = this.Width;
            originalFormHeight = this.Height;
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (originalFormWidth == 0 || originalFormHeight == 0) return;

            float widthRatio = (float)this.Width / originalFormWidth;
            float heightRatio = (float)this.Height / originalFormHeight;

            foreach (Control c in controlsToResize)
            {
                int newX = (int)(c.Location.X * widthRatio);
                int newY = (int)(c.Location.Y * heightRatio);
                int newWidth = (int)(c.Width * widthRatio);
                int newHeight = (int)(c.Height * heightRatio);

                c.Location = new Point(newX, newY);
                c.Size = new Size(newWidth, newHeight);
            }

            originalFormWidth = this.Width;
            originalFormHeight = this.Height;
        }
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }
}
