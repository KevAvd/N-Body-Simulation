﻿using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace SpaceSim.BarnesHut
{
    enum NodeType
    {
        Root, NorthWest, NorthEast, SouthWest, SouthEast
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

        public bool Contains(Particle p)
        {
            float x = p.Position.X;
            float y = p.Position.Y;

            if(x >= this.x-w && x <= this.x+w && y >= this.y-h && y <= this.y + h) { return true; }
            else { return false; }
        }
    }

    class CenterOfMass
    {
        float mass;
        Vector2f position;

        public CenterOfMass()
        {
            mass = 0f;
            position = new Vector2f(0f,0f);
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        public Vector2f Position
        {
            get { return position; }
            set { position = value; }
        }
    }

    class QuadTree
    {
        //taille du noeud
        Rectangle r;
        //Capacité du noeud
        const int capacity = 1;
        //Contenu du noeud
        List<Particle> particles = new List<Particle>();
        //État du noeud
        bool divided = false;
        //Type de noeud
        NodeType type;
        //Niveau du noeud
        uint layer;
        //Parent du noeud
        QuadTree parent;
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
        public QuadTree(Rectangle r)
        {
            this.r = r;
            parent = this;
            type = NodeType.Root;
            layer = 0;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="r"> Zone du noeud </param>
        /// <param name="cap"> Capacité du noeud </param>
        /// <param name="p"> Parent du noeud </param>
        QuadTree(Rectangle r, QuadTree p, NodeType t, uint l)
        {
            this.r = r;
            parent = p;
            type = t;
            layer = l;
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

            //Ajoute la particule
            particles.Add(p);

            //Ajoute la particule dans les enfants du noeud s'ils existent
            if (divided)
            {
                nw.Insert(p);
                ne.Insert(p);
                sw.Insert(p);
                se.Insert(p);
            }
            //Subdivise le noeud en 4 lorsque la capacité est dépassée
            else if(particles.Count > capacity)
            {
                if (!(r.w / 2 < 1 || r.h / 2 < 1))
                {
                    Subdivide();
                }
            }
        }

        /// <summary>
        /// Divise le noeud actuel en quatre nouveau noeud
        /// </summary>
        void Subdivide()
        {
            divided = true;
            nw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2), this, NodeType.NorthWest, layer + 1);
            ne = new QuadTree(new Rectangle(r.x + r.w / 2, r.y - r.h / 2, r.w / 2, r.h / 2), this, NodeType.NorthEast, layer + 1);
            sw = new QuadTree(new Rectangle(r.x - r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2), this, NodeType.SouthWest, layer + 1);
            se = new QuadTree(new Rectangle(r.x + r.w / 2, r.y + r.h / 2, r.w / 2, r.h / 2), this, NodeType.SouthEast, layer + 1);
            foreach (Particle p in particles)
            { 
                nw.Insert(p); 
                ne.Insert(p); 
                sw.Insert(p); 
                se.Insert(p); 
            }
        }

        public QuadTree GetParent()
        {
            if (parent != this)
            {
                return parent;
            }
            Console.WriteLine("[Error] Parent not found");
            return this;
        }
        public QuadTree GetChild(NodeType child)
        {
            if (divided)
            {
                switch (child)
                {
                    case NodeType.NorthWest: return nw;
                    case NodeType.NorthEast: return ne;
                    case NodeType.SouthWest: return sw;
                    case NodeType.SouthEast: return se;
                }
            }
            Console.WriteLine("[Error] Child Not Found");
            return this;
        }
        public QuadTree GetSibling(NodeType sibling)
        {
            if (parent != this)
            {
                return parent.GetChild(sibling);
            }
            Console.WriteLine("[Error] Siblings Not Found");
            return this;
        }
        public QuadTree FoundParticleNode(Particle p)
        {
            //Cas de base
            if (!divided)
            {
                return this;
            }

            //Cherche la particule dans les enfants
            if (nw.Particles.Contains(p))
            {
                return nw.FoundParticleNode(p);
            }
            if (ne.Particles.Contains(p))
            {
                return ne.FoundParticleNode(p);
            }
            if (sw.Particles.Contains(p))
            {
                return sw.FoundParticleNode(p);
            }
            if (se.Particles.Contains(p))
            {
                return se.FoundParticleNode(p);
            }

            //Particule non trouvée
            Console.WriteLine("[Error] QuadTree doesn't contains the researched particle");
            Console.WriteLine("[PARTICLE][POSITION] X:({0}), Y({1})", p.Position.X, p.Position.Y);
            QuadTree root = this;
            do
            {
                root = root.GetParent();
            }while (root.type != NodeType.Root);
            Console.WriteLine("[NODE] layer({0}), type({1})", this.layer,this.type);
            Console.WriteLine("[NODE][PARENT] layer({0}), type({1})", this.parent.layer, this.parent.type);
            if (root.Particles.Contains(p)) { Console.WriteLine("[ROOT] QuadTree's root contains the researched particle"); }
            else { Console.WriteLine("[ROOT] QuadTree's root doesn't contain the researched particle"); }
            if (this.particles.Contains(p)) { Console.WriteLine("[NODE] this node contain the researched particle"); }
            else { Console.WriteLine("[NODE] this node doesn't contain the researched particle"); }
            if (ne.Particles.Contains(p)) { Console.WriteLine("[NODE][CHILD:NE] this node contain the researched particle"); }
            else { Console.WriteLine("[NODE][CHILD:NE] this node doesn't contain the researched particle"); }
            if (nw.Particles.Contains(p)) { Console.WriteLine("[NODE][CHILD:NW] this node contain the researched particle"); }
            else { Console.WriteLine("[NODE][CHILD:NW] this node doesn't contain the researched particle"); }
            if (se.Particles.Contains(p)) { Console.WriteLine("[NODE][CHILD:SE] this node contain the researched particle"); }
            else { Console.WriteLine("[NODE][CHILD:SE] this node doesn't contain the researched particle"); }
            if (sw.Particles.Contains(p)) { Console.WriteLine("[NODE][CHILD:SW] this node contain the researched particle"); }
            else { Console.WriteLine("[NODE][CHILD:SW] this node doesn't contain the researched particle"); }
            return this;
        }
        public CenterOfMass GetCenterOfMass()
        {
            CenterOfMass com = new CenterOfMass();

            foreach(Particle p in particles)
            {
                com.Mass += p.Mass;
            }
            com.Position = new Vector2f(r.x,r.y);
            return com;
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

        public uint Layer
        {
            get { return layer; }
        }

        public NodeType Type
        {
            get { return type; }
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
