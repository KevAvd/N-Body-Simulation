using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace NBodySim
{
    class Camera
    {
        View _view;
        float _speed;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="w"> Largeur </param>
        /// <param name="h"> Hauteur </param>
        /// <param name="s"> Vitesse </param>
        public Camera(uint w, uint h, float s)
        {
            _view = new View(new Vector2f(0, 0), new Vector2f(w, h));
            _speed = s;
        }

        public void Update(float dt)
        {
            //Récupère les propriétés de la vue
            Vector2f p = _view.Center;
            Vector2f size = _view.Size;

            //Gère les entrées
            if (InputHandler.IsKeyPressed(Keyboard.Key.W))
            {
                p.Y -= _speed * dt;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.A))
            {
                p.X -= _speed * dt;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.S))
            {
                p.Y += _speed * dt;
            }
            if (InputHandler.IsKeyPressed(Keyboard.Key.D))
            {
                p.X += _speed * dt;
            }
            if (InputHandler.IsKeyClicked(Keyboard.Key.C))
            {
                size.X = 2 * size.X;
                size.Y = 2 * size.Y;
                _speed *= 2;
            }
            if (InputHandler.IsKeyClicked(Keyboard.Key.V))
            {
                size.X = size.X / 2;
                size.Y = size.Y / 2;
                _speed /= 2;
            }
            

            _view.Center = p;
            _view.Size = size;
        }
        public void Render(RenderWindow w)
        {
            w.SetView(_view);
        }

        /// <summary>
        /// Vue de la caméra
        /// </summary>
        public View View
        {
            get { return _view; }
            set { _view = value; }
        }

        /// <summary>
        /// Vitesse de déplacement de la caméra
        /// </summary>
        public float Speed
        {
            get { return _speed; }
            set
            {
                if (value <= 0) { throw new Exception("La vitesse de la caméra ne peut pas être inférieure ou égale à zéro"); }
                else { _speed = value; }
            }
        }
    }
}
