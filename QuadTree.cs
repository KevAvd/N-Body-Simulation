using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace SpaceSim.BarnesHut
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

            if(x > this.x-w && x < this.x+w && y > this.y-h && y < this.y + h) { return true; }
            else { return false; }
        }
    }

    class QuadTree
    {
        //taille du noeud
        Rectangle r;
        //Capacité du noeud
        readonly int capacity;
        //Contenu du noeud
        List<Particle> points = new List<Particle>();
        //État du noeud
        bool divided = false;
        //Enfant du noeud
        QuadTree nw;
        QuadTree ne;
        QuadTree sw;
        QuadTree se;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="r"> Zone du noeud </param>
        /// <param name="cap"> Capacité du noeud </param>
        public QuadTree(Rectangle r, int cap)
        {
            this.r = r;
            capacity = cap;
        }

        /// <summary>
        /// Insert un point dans le noeud actuel
        /// </summary>
        /// <param name="p"> Point à insérer </param>
        public void Insert(Particle p)
        {
            //Vérifie si le point et dans la zone du noeud actuel
            if (!r.Contains(p))
            {
                return;
            }

            //Vérifie si le noeud peut être divisé
            if(r.w/2 < 1 || r.h/2 < 1)
            {
                return;
            }

            //Vérifie si le noeud à la capacité d'accueillir un nouveau point
            if(points.Count < capacity)
            {
                points.Add(p);
            }
            //Vérifie si le noeud à atteint sa capacité maximale
            if (points.Count >= capacity)
            {
                //Divise le noeud si ce n'est pas déjà fait
                if (!divided)
                {
                    Subdivide();
                    divided = true;
                }
                //Rajoute le nouveau point dans les enfants du noeud
                else
                {
                    nw.Insert(p);
                    ne.Insert(p);
                    sw.Insert(p);
                    se.Insert(p);
                }
            }
        }

        /// <summary>
        /// Divise le noeud actuel en quatre nouveau noeud
        /// </summary>
        public void Subdivide()
        {
            nw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2), capacity);
            ne = new QuadTree(new Rectangle(r.x + r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2), capacity);
            sw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2), capacity);
            se = new QuadTree(new Rectangle(r.x + r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2), capacity);
            foreach (Particle p in points)
            { 
                nw.Insert(p); 
                ne.Insert(p); 
                sw.Insert(p); 
                se.Insert(p); 
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
            vertices[4] = new Vertex(new Vector2f(r.x - r.w, r.y - r.h), Color.Red);
            w.Draw(vertices,PrimitiveType.LineStrip);
            if(ne != null) { ne.Draw(w); }
            if(nw != null) { nw.Draw(w); }
            if(se != null) { se.Draw(w); }
            if(sw != null) { sw.Draw(w); }
        }
    }
}
