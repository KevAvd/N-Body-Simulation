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
        Physics physics = new Physics();

        //État du clique gauche de la souris
        bool leftClick = false;

        public Simulation(uint w, uint h, string t)
        {
            _window = new RenderWindow(new VideoMode(w, h), t,Styles.Fullscreen);
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
            physics.GenBodies(2500, 999, 999, new Vector2f(-10000, -10000), 10000);
            physics.GenBodies(2500, 999, 999, new Vector2f(10000, 10000), 10000);
            physics.AddParticle(9999999999, 1000, Color.White, new Vector2f(-10000,-10000));
            physics.AddParticle(9999999999, 1000, Color.Red, new Vector2f(10000,10000));
            physics.Particles[physics.Particles.Count - 2].Velocity = new Vector2f(1000, 0);
            physics.Particles[physics.Particles.Count - 1].Velocity = new Vector2f(-1000, 0);
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            camera.Update(_elapsedTime);
            physics.CreateQuadTree();
            physics.CalcForce(2f,99999999);
            physics.Update(_elapsedTime);
        }

        /// <summary>
        /// Dessine la simulation sur une image
        /// </summary>
        void Render()
        {
            camera.Render(_window);
            physics.Render(_window);
        }
    }
}
