using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Mathematics;

namespace TangramTheatre.SharedObjects
{
    public class TTBorder
    {
        private Body _borderBody;
        private Geom[] _borderGeom;
        private readonly float _borderWidth;
        private readonly float _height;
        private readonly float _width;
        private Vector2 _position;

        public TTBorder(float width, float height, float borderWidth, Vector2 position)
        {
            this._width = width;
            this._height = height;
            this._borderWidth = borderWidth;
            this._position = position;
        }

        public void LoadBorderGeom(PhysicsSimulator physicsSimulator)
        {
            this._borderGeom = new Geom[4];

            //left border
            Vector2 geometryOffset = new Vector2(-(_width * 0.5f - this._borderWidth * 0.5f), 0);
            _borderGeom[0] = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, this._borderBody,
                                                                      this._borderWidth, this._height, geometryOffset, 0);
            this._borderGeom[0].RestitutionCoefficient = .2f;
            this._borderGeom[0].FrictionCoefficient = .5f;
            this._borderGeom[0].CollisionGroup = 100;

            //right border
            geometryOffset = new Vector2((this._width * 0.5f - this._borderWidth * 0.5f), 0);
            _borderGeom[1] = GeomFactory.Instance.CreateGeom(physicsSimulator, this._borderBody, this._borderGeom[0],
                                                             geometryOffset, 0);

            //top border
            geometryOffset = new Vector2(0, -(_height*0.5f - this._borderWidth*0.5f));
            this._borderGeom[2] = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, this._borderBody,
                                                                           this._width, this._borderWidth,
                                                                           geometryOffset, 0);
            this._borderGeom[2].FrictionCoefficient = this._borderGeom[2].RestitutionCoefficient = .2f;
            this._borderGeom[2].CollisionGroup = 100;

            //bottom border
            geometryOffset = new Vector2(0, _height * 0.5f - this._borderWidth * 0.5f);
            this._borderGeom[3] = GeomFactory.Instance.CreateGeom(physicsSimulator, this._borderBody,
                                                                  this._borderGeom[2], geometryOffset, 0);
        }

        public void Load(TangramWindow window, PhysicsSimulator physicsSimulator)
        {
            //use the body factory to create the physics body
            this._borderBody = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, this._width, this._height, 1);
            this._borderBody.IsStatic = true;
            this._borderBody.Position = _position;
            LoadBorderGeom(physicsSimulator);
            float left = (_position.X - this._width / 2f);
            float right = (_position.X + this._width / 2f);
            float top = (_position.Y - this._height / 2f);
            float bottom = (_position.Y + this._height / 2f);

            // rectangle's position should be the point left and top most
            Rectangle leftBorder = window.AddRectangleToCanvas(null, Color.FromArgb(128, 255, 255, 255), new Vector2(this._borderWidth, this._height));
            TangramWindow.PositionTopLeft(leftBorder, new Vector2(left, top));

            Rectangle rightBorder = window.AddRectangleToCanvas(null, Color.FromArgb(128, 255, 255, 255), new Vector2(this._borderWidth, this._height));
            TangramWindow.PositionTopLeft(rightBorder, new Vector2(right - _borderWidth, top));

            Rectangle topBorder = window.AddRectangleToCanvas(null, Color.FromArgb(128, 255, 255, 255), new Vector2(this._width, this._borderWidth));
            TangramWindow.PositionTopLeft(topBorder, new Vector2(left, top));

            Rectangle bottomBorder = window.AddRectangleToCanvas(null, Color.FromArgb(128, 255, 255, 255), new Vector2(this._width, this._borderWidth));
            TangramWindow.PositionTopLeft(bottomBorder, new Vector2(left, bottom - _borderWidth));
        }
    }
}
