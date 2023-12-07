﻿using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_Sharp;

public sealed class Circle : Figure, IEquatable<Circle>
{
    public override SyntaxKind Kind => SyntaxKind.CircleToken;
    public Points Center { get; }
    public float Diameter => Radius * 2;
    public float Radius => Measure.Value;
    public Measure Measure { get; }
    public override string ReturnType => "circle";

    public Circle(Points center, Measure measure)
    {
        Center = center;
        Measure = measure;
    }

    public override object Evaluate(Scope scope)
    {
        return this;
    }

    public override bool Check(Scope scope)
    {
        return true;
    }

    public bool Equals(Circle? other)
    {
        return Center.Equals(other!.Center) && Measure.Equals(other!.Measure);
    }

    public override bool Equals(object? obj) => Equals(obj as Circle);
    public override int GetHashCode() => Center.GetHashCode();

    public override SequenceExpressionSyntax PointsInFigure()
    {
        Dictionary<int, object> elements = new();
        object PointsInCircle()
        {
            float x;
            float[] y;
            Random random = new();
            var position = random.Next(2);
            x = ParsingSupplies.CreateRandomsCoordinates((int)(Center.X - Radio - 1), (int)(Center.X + Radio + 1));
            y = IsInCircle(x);

            return new Points(x, y[position]);
        }

        var result = new InfiniteSequence(PointsInCircle, elements)
        {
            valuesType = "point"
        };

        return result;
    }

    private float[] IsInCircle(float x)
    {
        float distance1 = (float)(Math.Sqrt(Math.Pow(Radius, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;
        float distance2 = (float)(-Math.Sqrt(Math.Pow(Radius, 2) - Math.Pow(x - Center.X, 2))) + Center.Y;
        float[] result = { distance1, distance2 };

        return result;
    }
}
