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
    internal interface IObject
    {
        /// <summary>
        /// Met à jour l'objet
        /// </summary>
        /// <param name="dt"> Temps écoulé depuis la dernière image (DeltaTime) </param>
        void Update(float dt);
        /// <summary>
        /// Affiche l'objet
        /// </summary>
        /// <param name="w"> Fenêtre sur laquel afficher l'objet </param>
        void Render(RenderWindow w);
    }

    internal class Camera : IObject
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

    internal class ParticleSystem : IObject
    {
        List<Particle> _bodies;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="bodies"> Corps à ajouter dans le système </param>
        public ParticleSystem(params Particle[] bodies)
        {
            _bodies = new List<Particle>();
            AddBody(bodies);
        }

        public float GetTotalMass()
        {
            float totalMass = 0;
            foreach(Particle b in _bodies)
            {
                totalMass += b.Mass;
            }
            return totalMass;
        }

        /// <summary>
        /// Génère aléatoirement des corps positionné à l'intérieur d'un cercle
        /// </summary>
        /// <param name="nbrOfBodies"> Nombre de corps à générer </param>
        /// <param name="minMass"> Mass minimal </param>
        /// <param name="maxMass"> Mass maximal </param>
        /// <param name="center"> Centre du cercle de génération </param>
        /// <param name="radius"> Rayon du cercle de génération </param>
        public void GenBodies(uint nbrOfBodies, int minMass, int maxMass, Vector2f center, int radius)
        {
            Random rnd = new Random();

            //Génère n nombre de corps céléste
            for (int i = 0; i < nbrOfBodies; i++)
            {
                //Propriétés du corps
                Vector2f position = new Vector2f();
                float mass = rnd.Next(minMass,maxMass);
                Color color = new Color((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
                float d;

                //Génère une position à l'intérieur d'un cércle
                do
                {
                    //Génère une position aléatoirement
                    position = new Vector2f(rnd.Next(-radius, radius), rnd.Next(-radius, radius));

                    //Obtient la distance entre le centre du cercle de génération et la position du corps
                    float dx = position.X - center.X;
                    float dy = position.Y - center.Y;
                    d = (float)Math.Sqrt(dx * dx + dy * dy);

                    //Regénère la position du corps s'il sort du cercle
                } while (d > radius);

                //Ajoute le corps généré dans le système céléste
                AddBody(new Particle(mass, mass, color, position));
            }
        }

        /// <summary>
        /// Donne aléatoirment des vélocité au corps du system
        /// </summary>
        /// <param name="min"> Vélocité minimal </param>
        /// <param name="max"> Vélocité maximal </param>
        public void RndVel(int min, int max)
        {
            Random rnd = new Random();
            for(int i = 0; i < _bodies.Count; i++)
            {
                Vector2f vel = _bodies[i].Velocity;
                float speed = rnd.Next(min, max);
                float heading = rnd.NextSingle() * (float)Math.PI * 2.0f;
                vel.X = (float)Math.Cos(heading) * speed;
                vel.Y = (float)Math.Sin(heading) * speed;
                _bodies[i].Velocity = vel;
            }
        }

        public void RndVel(int min, int max, float heading)
        {
            Random rnd = new Random();
            for (int i = 0; i < _bodies.Count; i++)
            {
                Vector2f vel = _bodies[i].Velocity;
                float speed = rnd.Next(min, max);
                vel.X = (float)Math.Cos(heading) * speed;
                vel.Y = (float)Math.Sin(heading) * speed;
                _bodies[i].Velocity = vel;
            }
        }

        public void Update(float dt)
        {
            //Contient le totale d'accélération subit à cause de la gravité
            Vector2f totAcc;

            //Applique l'accélération sur tout les corps due à la gravité
            for(int i = 0; i < _bodies.Count; i++)
            {
                totAcc = new Vector2f(0f, 0f);
                for(int j = 0; j < _bodies.Count; j++)
                {
                    if(j == i) { continue; }
                    totAcc += getAccelVec(_bodies[i], _bodies[j]);
                }
                _bodies[i].Velocity += totAcc;
            }

            //Met à jour tout les corps céléstes
            foreach(Particle body in _bodies)
            {
                body.Update(dt);
            }
        }
        public void Render(RenderWindow w)
        {
            foreach (Particle body in _bodies)
            {
                body.Render(w);
            }
        }

        /// <summary>
        /// Obtient le vecteur d'accélération que subit un corps à cause de la gravité
        /// </summary>
        /// <param name="b1"> Corps qui subbit l'accélération </param>
        /// <param name="b2"> Corps qui cause l'accélération </param>
        /// <returns> Vecteur d'accélération </returns>
        Vector2f getAccelVec(Particle b1, Particle b2)
        {
            //Calcule la distance entre de corps
            float dx = b2.Position.X - b1.Position.X;
            float dy = b2.Position.Y - b1.Position.Y;
            float d2 = (float)Math.Sqrt(dx * dx + dy * dy);

            //Calcule l'accélération produite par la gravité
            float acc = b2.Mass / (d2 + 1);

            //Transforme l'accélération en vecteur
            float angle = (float)Math.Atan2(dy, dx);
            return new Vector2f((float)Math.Cos(angle) * acc, (float)Math.Sin(angle) * acc);
        }

        public void AddBody(params Particle[] b)
        {
            foreach(Particle body in b)
            {
                _bodies.Add(body);
            }
        }

        public void DelBody(Particle b)
        {
            _bodies.Remove(b);
        }

        public void DelAllBodies()
        {
            _bodies = new List<Particle>();
        }

        public List<Particle> Bodies
        {
            get { return _bodies; }
            set { _bodies = value; }
        }
    }

    internal class Particle : IObject
    {
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
        public Particle(float m, float r, Color c, Vector2f p)
        {
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
    }
}
