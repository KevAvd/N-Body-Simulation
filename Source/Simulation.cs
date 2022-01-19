using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
using SFML.Window;

namespace SpaceSim
{
    internal class Simulation
    {
        RenderWindow _window;
        List<IObject> _obj = new List<IObject>();
        Clock _clock;
        float _elapsedTime;

        public Simulation(uint w, uint h, string t)
        {
            _window = new RenderWindow(new VideoMode(w, h), t);
            _clock = new Clock();
        }

        public void Run()
        {
            Load();

            while (_window.IsOpen)
            {
                //Affiche le nombre d'image par seconde
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("FPS: {0:0.00}", 1 / _elapsedTime);
                Console.WriteLine("{0}          ", (_obj[0] as Camera).View.Size);
                Console.WriteLine("{0}          ", (_obj[0] as Camera).View.Center);
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
            _obj.Add(new Camera(_window.Size.X, _window.Size.Y, 500));
            ParticleSystem ps = new ParticleSystem();
            ps.GenBodies(500,10,200, new Vector2f(0f, 0f), 16000);
            ps.RndVel(8000, 8000, (float)Math.PI);
            Particle p = new Particle(ps.GetTotalMass()* 100, 4000, Color.Red, new Vector2f(0, 8000*5));
            ps.AddBody(p);
            _obj.Add(ps);
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            foreach(IObject obj in _obj)
            {
                obj.Update(_elapsedTime);
            }
        }

        /// <summary>
        /// Dessine la simulation sur une image
        /// </summary>
        void Render()
        {
            foreach (IObject obj in _obj)
            {
                obj.Render(_window);
            }
        }
    }
}
