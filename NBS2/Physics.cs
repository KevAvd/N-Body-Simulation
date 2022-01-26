using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NBodySim
{
    internal class Physics
    {
        QuadTree qt;
        List<Particle> particles = new List<Particle>();

        public void GenBodies(uint nbrOfBodies, int minMass, int maxMass, Vector2f center, int radius)
        {
            Random rnd = new Random();
            //Génère n nombre de corps céléste
            for (int i = 0; i < nbrOfBodies; i++)
            {
                //Propriétés du corps
                Vector2f position = new Vector2f();
                float mass = rnd.Next(minMass, maxMass);
                Color color = new Color((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                float d;

                //Génère une position à l'intérieur d'un cércle
                do
                {
                    //Génère une position aléatoirement
                    position = new Vector2f(rnd.Next((int)center.X - radius, (int)center.X + radius), rnd.Next((int)center.Y - radius, (int)center.Y + radius));

                    //Obtient la distance entre le centre du cercle de génération et la position du corps
                    float dx = position.X - center.X;
                    float dy = position.Y - center.Y;
                    d = (float)Math.Sqrt(dx * dx + dy * dy);

                    //Regénère la position du corps s'il sort du cercle
                } while (d > radius);

                //Ajoute le corps généré dans le système céléste
                particles.Add(new Particle(mass, 100, color, position, i));
            }
        }

        public void RndVel(int min, int max, float heading)
        {
            Random rnd = new Random();

            for (int i = 0; i < particles.Count; i++)
            {
                Vector2f vel = particles[i].Velocity;
                float speed = rnd.Next(min, max);
                vel.X += (float)Math.Cos(heading) * speed;
                vel.Y += (float)Math.Sin(heading) * speed;
                particles[i].Velocity = vel;
            }
        }
        public void RndVel(int min, int max, float heading, float heading2)
        {
            Random rnd = new Random();

            for (int i = 0; i < particles.Count; i++)
            {
                Vector2f vel = particles[i].Velocity;
                float speed = rnd.Next(min, max);
                if (i > particles.Count / 2)
                {
                    vel.X += (float)Math.Cos(heading2) * speed;
                    vel.Y += (float)Math.Sin(heading2) * speed;
                }
                else
                {
                    vel.X += (float)Math.Cos(heading) * speed;
                    vel.Y += (float)Math.Sin(heading) * speed;
                }
                particles[i].Velocity = vel;
            }
        }

        public void AddParticle(float m, float r, Color c, Vector2f p)
        {
            particles.Add(new Particle(m, r, c, p, particles.Count));
        }

        public void CreateQuadTree()
        {
            //Position du rectangle
            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;
            //Position des particules
            float x;
            float y;

            //Trouve la taille du rectangle qui contient toutes les particules
            foreach (Particle p in particles)
            {
                x = p.Position.X;
                y = p.Position.Y;
                if (x - 10 < minX)
                {
                    minX = x - 10;
                }
                if (x + 10 > maxX)
                {
                    maxX = x + 10;
                }
                if (y - 10 < minY)
                {
                    minY = y - 10;
                }
                if (y + 10 > maxY)
                {
                    maxY = y + 10;
                }
            }

            //Calcule la longueur du rectangle
            float w = (maxX - minX) / 2;
            //Calcule la hauteur du rectangle
            float h = (maxY - minY) / 2;

            //Transforme le rectangle en carré
            w = w < h ? h : w;
            h = w;

            //Crée un quadTree à partire du rectangle contenant toutes les particules
            qt = new QuadTree(new Rectangle(w + minX, h + minY, w, h));
            //Insère toutes les particules dans le quadTree
            foreach(Particle p in particles)
            {
                qt.Insert(p);
            }
        }

        public void CalcForce(float accuracy, float smoothValue)
        {
            foreach (Particle p in particles)
            {
                qt.CalcAccel(p, accuracy, smoothValue);
            }
        }

        public void Update(float dt)
        {
            foreach(Particle p in particles)
            {
                p.Update(dt);
            }
        }

        public void DrawQT(RenderWindow w)
        {
            qt.Draw(w);
        }

        public void Render(RenderWindow w)
        {
            foreach (Particle p in particles)
            {
                p.Render(w);
            }
        }

        public List<Particle> Particles
        {
            get { return particles; }
            set { particles = value; }
        }
    }
}
