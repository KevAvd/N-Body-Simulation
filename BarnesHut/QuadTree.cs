using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;
namespace SpaceSim.BarnesHut
{
    class Point
    {
        public float x, y, data;
        public Point(float x, float y, float data)
        {
            this.x = x;
            this.y = y;
            this.data = data;
        }

        public void Draw(RenderWindow w)
        {
            CircleShape shape = new CircleShape(2);
            shape.FillColor = Color.White;
            shape.Position = new Vector2f(x - shape.Radius, y - shape.Radius);
            w.Draw(shape);
        }
    }

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

        public bool Contains(Point p)
        {
            if(p.x > x-w && p.x < x+w && p.y > y-h && p.y < y + h) { return true; }
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
        List<Point> points = new List<Point>();
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
        /// <param name="w"> Fenêtre d'affichage </param>
        public QuadTree(Rectangle r, int cap)
        {
            this.r = r;
            capacity = cap;
        }

        /// <summary>
        /// Insert un point dans le noeud actuel
        /// </summary>
        /// <param name="p"> Point à insérer </param>
        public void Insert(Point p)
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
            foreach (Point p in points)
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
