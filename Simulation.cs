using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SpaceSim.BarnesHut;

namespace SpaceSim
{
    internal class Simulation
    {
        RenderWindow _window;
        List<IObject> _obj = new List<IObject>();
        Clock _clock;
        float _elapsedTime;

        //Test
        QuadTree _qt;
        ParticleSystem _ps = new ParticleSystem();

        //État du clique gauche de la souris
        bool leftClick = false;

        public Simulation(uint w, uint h, string t)
        {
            _window = new RenderWindow(new VideoMode(w, h), t);
            _clock = new Clock();
        }

        public void Run()
        {
            Load();
            Console.CursorVisible = false;

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
            Random rnd = new Random();
            _qt = new QuadTree(new Rectangle(_window.Size.X/2, _window.Size.Y / 2, _window.Size.X / 2, _window.Size.Y / 2));
            _obj.Add(new Camera(_window.Size.X, _window.Size.Y, 500));
            _ps.GenBodies(11, 1000, 20000, new Vector2f(400, 300), 400000);
            _ps.RndVel(600000, 600000,0);
            Particle pMassive = new Particle(_ps.GetTotalMass()*999, 400000, Color.Blue, new Vector2f(400, 300 + 400000 * 4));
            _ps.AddBody(pMassive);
            foreach (Particle p in _ps.Bodies)
            {
                _qt.Insert(p);
            }
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            ////Crée une particule au clique de souris
            //float mx = Mouse.GetPosition(_window).X;
            //float my = Mouse.GetPosition(_window).Y;
            //float tx = mx / _window.Size.X;
            //float ty = my / _window.Size.Y;
            //float nx = tx * (_obj[0] as Camera).View.Size.X;
            //float ny = ty * (_obj[0] as Camera).View.Size.Y;
            //float ox = (_obj[0] as Camera).View.Center.X - (_obj[0] as Camera).View.Size.X / 2;
            //float oy = (_obj[0] as Camera).View.Center.Y - (_obj[0] as Camera).View.Size.Y / 2;
            //Console.WriteLine("{0:0.00}               ", ox+nx);
            //Console.WriteLine("{0:0.00}               ", oy+ny);
            //if (Mouse.IsButtonPressed(Mouse.Button.Left))
            //{
            //    if (!leftClick)
            //    {
            //        Particle p = new Particle(100,2,Color.White,new Vector2f(ox+nx, oy+ny));
            //        _qt.Insert(p);
            //    }
            //    leftClick = true;
            //}
            //else
            //{
            //    leftClick = false;
            //}
            if (Keyboard.IsKeyPressed(Keyboard.Key.Q))
            {
                Console.Clear();
            }
            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;

            //Trouve la taille totale du rectangle
            foreach(Particle p in _ps.Bodies)
            {
                float x = p.Position.X;
                float y = p.Position.Y;
                if(x < minX)
                {
                    minX = x - 1;
                }
                if(x > maxX)
                {
                    maxX = x + 1;
                }
                if(y < minY)
                {
                    minY = y - 1;
                }
                if(y > maxY)
                {
                    maxY = y + 1;
                }
            }

            float w = (maxX - minX) / 2;
            float h = (maxY - minY) / 2;
            _qt = new QuadTree(new Rectangle(w + minX, h + minY, w, h));
            foreach (Particle p in _ps.Bodies)
            {
                _qt.Insert(p);
            }
            _ps.QuadTree = _qt;
            _ps.Update(_elapsedTime);
            foreach (IObject obj in _obj)
            {
                obj.Update(_elapsedTime);
            }
        }

        /// <summary>
        /// Dessine la simulation sur une image
        /// </summary>
        void Render()
        {
            _ps.Render(_window);
            _qt.Draw(_window);

            foreach (IObject obj in _obj)
            {
                obj.Render(_window);
            }
        }
    }
}
