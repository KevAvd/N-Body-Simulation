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
        ParticleHandler ph = new ParticleHandler();

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

                //Met à jour les entrées
                InputHandler.UpdateOld();
                InputHandler.Update();

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

                //Quitte le jeu
                if (InputHandler.IsKeyClicked(Keyboard.Key.F4))
                {
                    _window.Close();
                }
            }
        }

        /// <summary>
        /// Charge tout les éléments nécessaire à la simulation
        /// </summary>
        void Load()
        {
            ph.GenParticles(1000, 1, 100, Color.Blue, new Vector2f(100, 100), 10000);
            int id = ph.Particles.Count - 1;
            ph.RndVel(1000, 1000, 0, 0, id);
            ph.GenParticles(1000, 1, 100, Color.Blue, new Vector2f(100, 100), 10000);
            ph.RndVel(1000, 1000, (float)Math.PI, id, ph.Particles.Count - 1);
            physics.Particles = ph.Particles;
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            camera.Update(_elapsedTime);
            physics.CreateQuadTree();
            physics.CalcForce(2f, 9999);
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
