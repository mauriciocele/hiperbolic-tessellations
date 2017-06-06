using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Numerics;

namespace HyperbolicTessellation
{
    public class Tessellation
    {
        public Circle Disk { get; set; }
        public List<Point> Points { get; set; }
        public SortedSet<Poly> Polygons { get; set; }

        public Tessellation(Circle c)
        {
            Disk = c;
            Points = new List<Point>();
            Polygons = new SortedSet<Poly>();
        }

        public void Tessellate(int p, int q, int maxLevel)
        {
            var centralPoly = CreateCenterPolygon(p, q);
            ApplyIsometries(centralPoly, maxLevel);
        }

        Poly CreateCenterPolygon(int p, int q)
        {
            double s = Math.Sin(Math.PI / p);
            double c = Math.Cos(Math.PI / q);
            // Computo OC y AO segun el articulo de Coexeter
            double OC = 1.0 / Math.Sqrt(((c * c) / (s * s)) - 1.0);
            double AO = 1.0 / Math.Sqrt(1.0 - ((s * s) / (c * c)));

            // Creo el circulo con centro = (AO,0) y radio = OC
            var circle = new Circle(center: new Point(AO, 0), radius: OC);

            // Computo el punto B hallando la interseccion del circulo con 
            // la linea que parte desde A en direccion = ( cos(pi/p), sin(pi/p) )
            var lineAB = new Line(o: new Point(0, 0), dir: new Vector(Math.Cos(Math.PI / p), Math.Sin(Math.PI / p)));
            Point B = Geometry.LineCircleIntersection(lineAB, circle);

            // Computo las posiciones de los vertices del polígono central 
            // en la circunferencia de la región fundamental añadiendo un 
            // vertice en la circunferencia cada 2*pi/p radianes
            double dist = (B - lineAB.O).Length;
            for (int i = 0; i < p; ++i)
            {
                double alpha = (double)i / p * 2.0 * Math.PI;
                Point X = Complex2Vector(dist * new Complex(Math.Cos(alpha), Math.Sin(alpha)));
                Points.Add(X);
            }
            // Añado el punto A a los vertices
            Points.Add(Complex2Vector(new Complex(0, 0))); //centro A

            // Defino el poligono central
            Poly poly = new Poly();
            for (int i = 0; i < p; ++i)
                poly.edges.Add(new Edge(i, (i + 1) % p));
            poly.level = 1;
            poly.center = p;
            // para computar la teselacion dual cada lado guarda sus 
            // poligonos adyacentes
            foreach (var e in poly.edges)
                e.Neighbors.Add(poly.center); 

            //retorna el poligono
            return poly;
        }

        void ApplyIsometries(Poly centralPoly, int maxLevel)
        {
            //Este metodo es de fuerza bruta.
            //TODO: Implementar K-d-tree (BSP-tree, Quad-tree o UniformGrid son buenas alternativas) para buscar vertices.
            HashSet<Edge> edges = new HashSet<Edge>();
            Queue<Poly> Q = new Queue<Poly>();
            Q.Enqueue(centralPoly);
            Polygons.Add(centralPoly);
	        while(!(Q.Count == 0))
	        {
		        Poly current = Q.Dequeue();

		        if(current.level >= maxLevel )
			        continue;

                foreach (var edge in current.edges)
		        {
                    if (!edges.Contains(edge))
			        {
                        // procesa el lado solo una vez
                        edges.Add(edge);
				        int oldPointSize = Points.Count;
                        // Refleja el poligono en el lado
                        Poly poly = ReflectPoly(current, edge);
                        if (oldPointSize != Points.Count || !Polygons.Contains(poly))
				        {
                            // si el poligono no existe lo añado a la lista
                            Polygons.Add(poly);
                            Q.Enqueue(poly);
                        }
                        // para computar la teselacion dual cada lado guarda sus 
                        // poligonos adyacentes
                        edge.Neighbors.Add(poly.center);
                    }
		        }
	        }
        }

        private Poly ReflectPoly(Poly current, Edge inversionEdge)
        {
            var geodesic = GetGeodesicLine(inversionEdge);
            List<int> indices = new List<int>();
            foreach(var edge in current.edges)
            {
                if (edge.v0 == inversionEdge.v0 || edge.v0 == inversionEdge.v1)
                    indices.Add(edge.v0);
                else
                    indices.Add(ReflectPoint(geodesic, edge.v0));
            }

            Poly poly = new Poly();
            for (int i = 0; i < indices.Count; ++i)
                poly.edges.Add(new Edge(indices[i], indices[(i + 1) % indices.Count]));
            poly.level = current.level + 1;
            poly.center = ReflectPoint(geodesic, current.center);
            foreach (var edge in poly.edges)
                edge.Neighbors.Add(poly.center);
            return poly;
        }

        private int ReflectPoint(Geodesic geodesic, int pointIndex)
        {
            Point newP = PointInversion(Points[pointIndex], geodesic);

            var pt = Points.FindIndex(p => Math.Abs(p.X - newP.X) < 1e-6 && Math.Abs(p.Y - newP.Y) < 1e-6);
            if (pt >= 0)
                return pt;
            var index = Points.Count;
            Points.Add(newP);
            return index;
        }

        public Geodesic GetGeodesicLine(Edge e)
        {

            Point pointBinv = PointInversion(Points[e.v1], Disk);

	        //Clac the circle from three points
            return new Geodesic(Points[e.v0], Points[e.v1], new Circle(Points[e.v0], Points[e.v1], pointBinv));
        }

        public void Resize(Circle c)
        {
            var complexPoints = new List<Complex>();
            foreach (var p in Points)
                complexPoints.Add(Vector2Complex(p));
            Disk = c;
            for (int p = 0; p < complexPoints.Count; ++p)
            {
                Points[p] = Complex2Vector(complexPoints[p]);
            }
        }

        public void Move(Point point, Point previusPoint)
        {
            int dirY = (point.Y > previusPoint.Y) ? 1 : -1;
            int dirX = (point.X > previusPoint.X) ? 1 : -1;
            double incY = Math.Abs(point.Y - previusPoint.Y) / Disk.Radius;
            double incX = Math.Abs(point.X - previusPoint.X) / Disk.Radius;

            var isometry = Mobius.Translation(dirX * incX, dirY * incY);
            for (int i = 0; i < Points.Count; ++i)
            {
                Complex newP = isometry.Apply(Vector2Complex(Points[i]));
                Points[i] = Complex2Vector(newP);
            }
        }

        public void Rotate(Point point, Point previusPoint)
        {
            int dirY = (point.Y > previusPoint.Y) ? 1 : -1;
            int dirX = (point.X > previusPoint.X) ? 1 : -1;
            double incY = Math.Abs(point.Y - previusPoint.Y) / Disk.Radius;
            double incX = Math.Abs(point.X - previusPoint.X) / Disk.Radius;

            var isometry = Mobius.Rotation(dirY * incY);
            for (int i = 0; i < Points.Count; ++i)
            {
                Complex newP = isometry.Apply(Vector2Complex(Points[i]));
                Points[i] = Complex2Vector(newP);
            }
        }

        Point PointInversion(Point P, Circle C)
        {
            if (C.HasInfiniteRadius)
                throw new Exception("Inversion on circles of Infinite radius not supported");
            return C.Center + ((P - C.Center) * (C.Radius * C.Radius)) / (P - C.Center).LengthSquared;
        }

        Point PointInversion(Point P, Geodesic Geo)
        {
            if (Geo.IsLine)
                return Geo.L.Reflect(P);
            return PointInversion(P, Geo.C);
        }

        Complex Vector2Complex(Point v)
        {
            return new Complex((v.X - Disk.Center.X) / Disk.Radius, (v.Y - Disk.Center.Y) / Disk.Radius);
        }

        Point Complex2Vector(Complex c)
        {
            return new Point(c.Real * Disk.Radius + Disk.Center.X, c.Imaginary * Disk.Radius + Disk.Center.Y);
        }

    }
}
