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
        QuadTree _qt;
        List<Point> _points = new List<Point>();

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
            Random rnd = new Random();
            _qt = new QuadTree(new Rectangle(_window.Size.X/2, _window.Size.Y / 2, _window.Size.X / 2, _window.Size.Y / 2), 2);
            _obj.Add(new Camera(_window.Size.X, _window.Size.Y, 500));
            //int nbrOfPoints = 1000;
            //for (int i = 0; i < nbrOfPoints; i++)
            //{
            //    Point p = new Point(rnd.Next((int)_window.Size.X), rnd.Next((int)_window.Size.Y), 0);
            //    _points.Add(p);
            //    _qt.Insert(p);
            //}
        }

        /// <summary>
        /// Met à jour la simulation
        /// </summary>
        void Update()
        {
            float mx = Mouse.GetPosition(_window).X;
            float my = Mouse.GetPosition(_window).Y;
            float tx = mx / _window.Size.X;
            float ty = my / _window.Size.Y;
            float nx = tx * (_obj[0] as Camera).View.Size.X;
            float ny = ty * (_obj[0] as Camera).View.Size.Y;
            float ox = (_obj[0] as Camera).View.Center.X - (_obj[0] as Camera).View.Size.X / 2;
            float oy = (_obj[0] as Camera).View.Center.Y - (_obj[0] as Camera).View.Size.Y / 2;
            Console.WriteLine("{0:0.00}               ", ox+nx);
            Console.WriteLine("{0:0.00}               ", oy+ny);
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                if (!leftClick)
                {
                    Point p = new Point(ox+nx, oy+ny, 0);
                    _points.Add(p);
                    _qt.Insert(p);
                }
                leftClick = true;
            }
            else
            {
                leftClick = false;
            }

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
            foreach(Point p in _points)
            {
                p.Draw(_window);
            }
            _qt.Draw(_window);
            foreach (IObject obj in _obj)
            {
                obj.Render(_window);
            }
        }
    }
}
