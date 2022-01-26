using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace NBodySim
{
    internal class ParticleHandler
    {
        static List<Particle> particles = new List<Particle>();
        Random rnd = new Random();

        public ParticleHandler()
        {

        }

        public void AddParticle(float m, float r, Color c, Vector2f p)
        {
            particles.Add(new Particle(m, r, c, p, particles.Count));
        }

        public void GenParticles(float nbr, float m, float r, Color c, Vector2f gp, float gr)
        {
            float angle;
            float hyp;
            float dx;
            float dy;

            for(int i = 0; i < nbr; i++)
            {
                angle = (float)rnd.NextDouble() * ((float)Math.PI * 2);
                hyp = (float)rnd.NextDouble() * gr;
                dx = (float)Math.Cos(angle) * hyp;
                dy = (float)Math.Sin(angle) * hyp;
                particles.Add(new Particle(m, r, c, gp + new Vector2f(dx, dy), particles.Count));
            }
        }

        public void RndVel(int min, int max, float heading, int idmin, int idmax)
        {
            for (int i = idmin; i <= idmax; i++)
            {
                float vx = (float)Math.Cos(heading) * rnd.Next(min, max);
                float vy = (float)Math.Sin(heading) * rnd.Next(min, max);
                particles[i].Velocity = new Vector2f(vx, vy);
            }
        }

        public List<Particle> Particles
        {
            get { return particles; }
            set { particles = value; }
        }
    }
}
