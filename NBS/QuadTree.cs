using System;
using System.Collections.Generic;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

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
        float centerOfMass;
        //Position du centre de mass
        Vector2f comPosition;
        //Niveau du noeud
        uint layer;
        //Parent du noeud
        QuadTree parent;
        //Enfant du noeud
        QuadTree nw;
        QuadTree ne;
        QuadTree sw;
        QuadTree se;

        public QuadTree(Rectangle r)
        {
            this.r = r;
        }

        public void insert(Particle p)
        {
            //Ne fait rien si la particule n'est pas à l'intérieur du noeud
            if(!r.Contains(p))
            {
                return;
            }

            if(particles.Count == 0)
            {
                particles.Add(p);
                UpdateCOM();
            }

            if(particles.Count == 1)
            {
                particles.Add(p);
                Subdivide();
            }

            if(particles.Count > 1)
            {
                particles.Add(p);
                nw.insert(p);
                ne.insert(p);
                sw.insert(p);
                se.insert(p);
                UpdateCOM();
                nw.UpdateCOM();
                ne.UpdateCOM();
                sw.UpdateCOM();
                se.UpdateCOM();
            }
        }

        void Subdivide()
        {
            //Ne divise seulement lorsque c'est possible
            if(r.h / 2 < 1 || r.w / 2 < 1)
            {
                Console.WriteLine("[Can't divide more]");
                return;
            }

            //Crée les enfants du noeud
            nw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2));
            ne = new QuadTree(new Rectangle(r.x + r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2));
            sw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2));
            se = new QuadTree(new Rectangle(r.x + r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2));

            //Insert les particules dans les enfants du noeuds
            foreach (Particle prt in particles)
            {
                nw.insert(prt);
                ne.insert(prt);
                sw.insert(prt);
                se.insert(prt);
            }

            //Met à jour les centres de masses
            UpdateCOM();
            nw.UpdateCOM();
            ne.UpdateCOM();
            sw.UpdateCOM();
            se.UpdateCOM();
        }

        void UpdateCOM()
        {

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
            vertices[4] = new Vertex(new Vector2f(r.x - r.w, r.y - r.h), Color.Red);
            w.Draw(vertices, PrimitiveType.LineStrip);
            if (ne != null) { ne.Draw(w); }
            if (nw != null) { nw.Draw(w); }
            if (se != null) { se.Draw(w); }
            if (sw != null) { sw.Draw(w); }
        }

        public uint Layer
        {
            get { return layer; }
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
