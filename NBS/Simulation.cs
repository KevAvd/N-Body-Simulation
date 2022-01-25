using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace NBodySim
{
    internal class Simulation
    {
        RenderWindow _window;
        Clock _clock;
        float _elapsedTime;
        Camera camera;
        QuadTree qt;
        List<Particle> particles = new List<Particle>();

        //État du clique gauche de la souris
        bool leftClick = false;

        public Simulation(uint w, uint h, string t)
        {
            _window = new RenderWindow(new VideoMode(w, h), t);
            camera = new Camera(w, h, 500);
            _clock = new Clock();
        }

        public void Run()
        {
            Load();
            int cursorTop = Console.CursorTop;
            while (_window.IsOpen)
            {
                //Obtient le temps écoulé depuis la dernière image
                _elapsedTime = _clock.ElapsedTime.AsSeconds();

                //Redémarre la montre 
                _clock.Restart();

                //Gère les événements de la fenêtre
                _window.DispatchEvents();

                //Efface l'image précédente
                _window.Clear(Color.Black);

                //Met à jour la simulation
                Update();

                //Dessine les objets
                Render();

                //Affiche la simulation
                _window.Display();
            }
        }

        /// <summary>
        /// Charge tout les éléments nécessaire à la simulation
        /// </summary>
        void Load()
        {
            Console.CursorVisible = false;

            Random rnd = new Random();

            uint nbrOfBodies = 10000;
            int minMass = 100;
            int maxMass = 100;
            Vector2f center = new Vector2f(0, 0);
            int radius = 10000;
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
            int min = 5500;
            int max = 5500;
            float heading = (float)Math.PI / 4f;
            for (int i = 0; i < particles.Count; i++)
            {
                Vector2f vel = particles[i].Velocity;
                float speed = rnd.Next(min, max);
                //heading = (float)rnd.NextDouble() * (float)Math.PI * 2.0f;
                if (i > particles.Count / 2)
                {
                    vel.X = (float)Math.Cos(heading + Math.PI / 2f) * speed;
                    vel.Y = (float)Math.Sin(heading + Math.PI / 2f) * speed;
                }
                else
                {
                    vel.X = (float)Math.Cos(heading) * speed;
                    vel.Y = (float)Math.Sin(heading) * speed;
                }
                particles[i].Velocity = vel;
            }

            particles.Add(new Particle(99999999999, 1000, Color.Yellow, new Vector2f(0, 10000 * 4f), 10000));
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            camera.Update(_elapsedTime);

            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;

            //Trouve la taille totale du rectangle
            foreach (Particle p in particles)
            {
                float x = p.Position.X;
                float y = p.Position.Y;
                if (x < minX)
                {
                    minX = x - 1000;
                }
                if (x > maxX)
                {
                    maxX = x + 1000;
                }
                if (y < minY)
                {
                    minY = y - 1000;
                }
                if (y > maxY)
                {
                    maxY = y + 1000;
                }
            }

            float w = (maxX - minX) / 2;
            float h = (maxY - minY) / 2;

            w = w < h ? h : w;
            h = w;

            qt = new QuadTree(new Rectangle(w + minX, h + minY, w, h));
            foreach (Particle p in particles)
            {
                qt.Insert(p);
            }

            foreach(Particle p in particles)
            {
                qt.CalcAccel(p, 2f, 99);
            }

            foreach (Particle p in particles)
            {
                p.Update(_elapsedTime);
            }
        }

        /// <summary>
        /// Dessine la simulation sur une image
        /// </summary>
        void Render()
        {
            camera.Render(_window);
            foreach(Particle p in particles)
            {
                p.Render(_window);
            }
            //qt.Draw(_window);
        }
    }
}
