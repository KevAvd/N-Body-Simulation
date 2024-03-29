﻿using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace NBodySim
{
    internal class Physics
    {
        QuadTree qt;
        List<Particle> particles = new List<Particle>();

        public void CreateQuadTree()
        {
            //Position du rectangle
            float minX = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float minY = float.PositiveInfinity;
            float maxY = float.NegativeInfinity;
            //Position des particules
            float x;
            float y;

            //Trouve la taille du rectangle qui contient toutes les particules
            foreach (Particle p in particles)
            {
                x = p.Position.X;
                y = p.Position.Y;
                if (x - 10 < minX)
                {
                    minX = x - 10;
                }
                if (x + 10 > maxX)
                {
                    maxX = x + 10;
                }
                if (y - 10 < minY)
                {
                    minY = y - 10;
                }
                if (y + 10 > maxY)
                {
                    maxY = y + 10;
                }
            }

            //Calcule la longueur du rectangle
            float w = (maxX - minX) / 2;
            //Calcule la hauteur du rectangle
            float h = (maxY - minY) / 2;

            //Transforme le rectangle en carré
            w = w < h ? h : w;
            h = w;

            //Crée un quadTree à partire du rectangle contenant toutes les particules
            qt = new QuadTree(new Rectangle(w + minX, h + minY, w, h));
            //Insère toutes les particules dans le quadTree
            foreach(Particle p in particles)
            {
                qt.Insert(p);
            }
        }

        public void CalcForce(float accuracy, float smoothValue)
        {
            foreach (Particle p in particles)
            {
                qt.CalcAccel(p, accuracy, smoothValue);
            }
        }

        public void Update(float dt)
        {
            foreach(Particle p in particles)
            {
                p.Update(dt);
            }
        }

        public void DrawQT(RenderWindow w)
        {
            qt.Draw(w);
        }

        public void Render(RenderWindow w)
        {
            foreach (Particle p in particles)
            {
                p.Render(w);
            }
        }

        public List<Particle> Particles
        {
            get { return particles; }
            set { particles = value; }
        }
    }
}
