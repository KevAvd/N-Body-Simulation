using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
namespace NBodySim
{
    class Rectangle
    {
        public float x, y, w, h;
        public Rectangle(float x, float y, float w, float h)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public bool Contains(Particle p)
        {
            float x = p.Position.X;
            float y = p.Position.Y;
            return (x >= this.x - w && x <= this.x + w && y >= this.y - h && y <= this.y + h) ? true : false;
        }
    }

    class QuadTree
    {
        //taille du noeud
        Rectangle r;
        //Contenu du noeud
        List<Particle> particles = new List<Particle>();
        //Centre de mass
        float totalMass = 0;
        //Position du centre de mass
        Vector2f centerOfMass;
        //Enfant du noeud
        QuadTree[] child = new QuadTree[4];

        public QuadTree(Rectangle r)
        {
            this.r = r;
        }

        public void Insert(Particle p)
        {
            //Ne fait rien si la particule n'est pas à l'intérieur du noeud
            if(!r.Contains(p))
            {
                return;
            }

            //Ajoute une particule dans le noeud
            particles.Add(p);

            if(particles.Count == 1)
            {
                //Met à jour le centre de mass
                Update();
            }
            else if(particles.Count == 2)
            {
                //Divise le noeuds en quatre
                Subdivide();
            }
            else if(particles.Count > 2)
            {
                //Insert la particule dans tout les noeuds enfants
                for (int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].Insert(p); } }

                //Met à jour le centre de mass
                Update();
                for (int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].Update(); } }
            }
        }

        void Subdivide()
        {
            //Ne divise seulement lorsque c'est possible
            if(r.h / 2 < 1 || r.w / 2 < 1)
            {
                return;
            }

            //Crée les enfants du noeud
            child[0] = new QuadTree(new Rectangle(r.x - r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2));
            child[1] = new QuadTree(new Rectangle(r.x + r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2));
            child[2] = new QuadTree(new Rectangle(r.x - r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2));
            child[3] = new QuadTree(new Rectangle(r.x + r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2));

            //Insert les particules dans les enfants du noeuds
            foreach (Particle prt in particles)
            {
                for (int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].Insert(prt); } }
            }

            //Met à jour les centres de masses
            Update();
            for (int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].Update(); } }
        }

        void Update()
        {
            if (particles.Count > 0)
            {
                Particle p = particles[particles.Count - 1];
                centerOfMass.X = centerOfMass.X * totalMass;
                centerOfMass.Y = centerOfMass.Y * totalMass;
                totalMass += p.Mass;
                centerOfMass += new Vector2f(p.Mass * p.Position.X, p.Mass * p.Position.Y);
                centerOfMass.X /= totalMass;
                centerOfMass.Y /= totalMass;
            }
        }

        public void CalcAccel(Particle p, float accuracy, float smoothValue)
        {
            if(particles.Count == 1)
            {
                if (p.ID != particles[0].ID)
                {
                    float dx = centerOfMass.X - p.Position.X;
                    float dy = centerOfMass.Y - p.Position.Y;
                    float d2 = dx * dx + dy * dy;
                    float accel = totalMass / (d2 + smoothValue);
                    float angle = (float)Math.Atan2(dy, dx);
                    p.Velocity += new Vector2f((float)Math.Cos(angle) * accel, (float)Math.Sin(angle) * accel);
                }
            }
            else if(particles.Count > 1)
            {
                float s = r.w;
                float dy = centerOfMass.Y - p.Position.Y;
                float dx = centerOfMass.X - p.Position.X;
                float d = (float)Math.Sqrt(dx * dx + dy * dy);
                if (s / d <= accuracy)
                {
                    float accel = totalMass / (d*d + smoothValue);
                    float angle = (float)Math.Atan2(dy, dx);
                    p.Velocity += new Vector2f((float)Math.Cos(angle) * accel, (float)Math.Sin(angle) * accel);
                }
                else
                {
                    for (int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].CalcAccel(p, accuracy, smoothValue); } }
                }
            }
        }

        /// <summary>
        /// Dessine les bordure du noeud actuel
        /// </summary>
        /// <param name="w"> Fenêtre d'affichage </param>
        public void Draw(RenderWindow w)
        {
            Vertex[] vertices = new Vertex[5];
            vertices[0] = new Vertex(new Vector2f(r.x - r.w, r.y - r.h), Color.Red);
            vertices[1] = new Vertex(new Vector2f(r.x + r.w, r.y - r.h), Color.Red);
            vertices[2] = new Vertex(new Vector2f(r.x + r.w, r.y + r.h), Color.Red);
            vertices[3] = new Vertex(new Vector2f(r.x - r.w, r.y + r.h), Color.Red);
            vertices[4] = vertices[0];
            w.Draw(vertices, PrimitiveType.LineStrip);
            for(int i = 0; i < child.Length; i++) { if (child[i] != null) { child[i].Draw(w); } }
        }

        public List<Particle> Particles
        {
            get { return particles; }
        }

        public Rectangle Rectangle
        {
            get { return r; }
        }
    }
}
