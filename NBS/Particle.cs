using SFML.Graphics;
using SFML.System;
using System;

namespace NBodySim
{
    internal class Particle
    {
        int _id;
        float _mass;
        float _radius;
        Color _color;
        Vector2f _position;
        Vector2f _velocity;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="m"> Masse </param>
        /// <param name="r"> Rayon </param>
        /// <param name="c"> Couleur </param>
        public Particle(float m, float r, Color c, Vector2f p, int id)
        {
            _id = id;
            _mass = m;
            _radius = r;
            _color = c;
            _position = p;
            _velocity = new Vector2f(0, 0);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="m"> Masse </param>
        /// <param name="r"> Rayon </param>
        /// <param name="c"> Couleur </param>
        /// <param name="v"> Vélocité </param>
        public Particle(float m, float r, Color c, Vector2f p, Vector2f v)
        {
            _mass = m;
            _radius = r;
            _color = c;
            _position = p;
            _velocity = v;
        }

        public void Update(float dt)
        {
            _position += _velocity * dt;
        }
        public void Render(RenderWindow w)
        {
            CircleShape shape = new CircleShape(_radius);
            shape.Position = new Vector2f(_position.X - _radius, _position.Y - _radius);
            shape.FillColor = _color;
            w.Draw(shape);
        }

        public float Mass
        {
            get { return _mass; }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    _mass = value;
                }
            }
        }

        public float Radius
        {
            get { return _radius; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    _radius = value;
                }
            }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public Vector2f Position
        {
            get { return _position; }
            set {_position = value; }
        }

        public Vector2f Velocity
        {
            get { return _velocity; }
            set {_velocity = value; }
        }

        public int ID
        {
            get { return _id; }
        }
    }
}
