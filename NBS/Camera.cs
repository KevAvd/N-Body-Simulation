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
        bool _cClick = false;
        bool _vClick = false;

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
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                p.Y -= _speed * dt;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                p.X -= _speed * dt;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                p.Y += _speed * dt;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                p.X += _speed * dt;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.C))
            {
                if (!_cClick)
                {
                    size.X = 2 * size.X;
                    size.Y = 2 * size.Y;
                    _speed *= 2;
                    _cClick = true;
                }
            }
            else
            {
                _cClick = false;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.V))
            {
                if (!_vClick)
                {
                    size.X = size.X / 2;
                    size.Y = size.Y / 2;
                    _speed /= 2;
                    _vClick = true;
                }
            }
            else
            {
                _vClick = false;
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
