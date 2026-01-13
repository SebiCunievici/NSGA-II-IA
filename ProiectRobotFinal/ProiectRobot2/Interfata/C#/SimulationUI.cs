using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace EvolutionaryAlgorithm
{
    public class SimulationForm : Form
    {
        private Chromosome _pilot;
        private List<float> _obstacles;
        private float _robotX = 0;
        private Timer _timer;
        private int _step = 0;
        private bool _isCrashed = false;

        public SimulationForm(Chromosome pilot)
        {
            this.Text = "Simulare NSGA-II: Evolutie/Rezistena Robot";
            this.Size = new Size(800, 400);
            this.DoubleBuffered = true; // Previne palpairea imaginii
            _pilot = pilot;

            // Generam un traseu de obstacole (X-uri)
            _obstacles = new List<float> { 100,120,140, 300, 400, 430, 600, 750 };

            _timer = new Timer { Interval = 50 }; // 20 FPS
            _timer.Tick += (s, e) => UpdateSimulation();
            _timer.Start();
        }

        private void UpdateSimulation()
        {


            if (_isCrashed) return; // Oprim totul dacă s-a izbit


            float nextObstacle = _obstacles.Find(o => o > _robotX);
            float dist = nextObstacle - _robotX;

            // SINCRONIZARE: Folosim aceeasi formula ca in RobotEvolution.cs
            // Genes[0] = W1 (Accelerație), Genes[1] = W2 (Frânare)
            float velocity = (float)((_pilot.Genes[0] * dist) - (_pilot.Genes[1] / dist));

            // Limităm viteza pentru a nu merge cu spatele sau prea repede (ca în fitness)
            velocity = Math.Max(0.5f, Math.Min(8.0f, velocity));

            _robotX += velocity;

            // Resetare circuit
            if (_robotX > 800) _robotX = 0;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (_isCrashed) g.Clear(Color.DarkRed);

             g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Desenam drumul
            g.DrawLine(Pens.Gray, 0, 200, 800, 200);

            // Desenam obstacolele
            foreach (var obs in _obstacles)
                g.FillRectangle(Brushes.Red, obs, 180, 10, 40);

            // Desenam robotul
            g.FillEllipse(Brushes.Blue, _robotX, 185, 30, 30);
            g.DrawString("ROBOT", SystemFonts.DefaultFont, Brushes.White, _robotX + 2, 195);

            // Afisam datele in timp real
            g.DrawString($"Gene: W1={_pilot.Genes[0]:F2}", SystemFonts.DefaultFont, Brushes.Black, 10, 10);
            g.DrawString($"Pozitie: {_robotX:F1}", SystemFonts.DefaultFont, Brushes.Black, 10, 30);

            if (_isCrashed)
            {
                g.DrawString("!!! COLLISION !!!", new Font("Arial", 20, FontStyle.Bold),
                             Brushes.Yellow, 250, 100);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SimulationForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "SimulationForm";
            this.Load += new System.EventHandler(this.SimulationForm_Load);
            this.ResumeLayout(false);

        }

        private void SimulationForm_Load(object sender, EventArgs e)
        {

        }
    }
}