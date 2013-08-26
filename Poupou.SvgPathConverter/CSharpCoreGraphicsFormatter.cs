// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//
// Licensed under the GNU LGPL 2 license only (no "later versions")

using System;
using System.Drawing;
using System.Globalization;
using System.IO;

namespace Poupou.SvgPathConverter {

	public class CSharpCoreGraphicsFormatter : ISourceFormatter {

		TextWriter writer;

		public CSharpCoreGraphicsFormatter (TextWriter textWriter)
		{
			writer = textWriter;
		}
		
		public void Prologue (string name)
		{
			writer.WriteLine ("\tstatic CGPath {0}_path ()", name);
			writer.WriteLine ("\t{");
            writer.WriteLine("\t\tCGPath p = new CGPath();");
		}
	
		public void Epilogue ()
		{
			//writer.WriteLine ("\t\tc.FillPath ();");
			//writer.WriteLine ("\t\tc.StrokePath ();");
            writer.WriteLine("\t\treturn p;");
			writer.WriteLine ("\t}");
			writer.WriteLine ();
		}
	
		public void MoveTo (PointF pt)
		{
			writer.WriteLine ("\t\tp.MoveToPoint ({0}f, {1}f);", pt.X.ToString (CultureInfo.InvariantCulture), 
				pt.Y.ToString (CultureInfo.InvariantCulture));
		}
	
		public void LineTo (PointF pt)
		{
			writer.WriteLine ("\t\tp.AddLineToPoint ({0}f, {1}f);", pt.X.ToString (CultureInfo.InvariantCulture), 
			 	pt.Y.ToString (CultureInfo.InvariantCulture));
		}
	
		public void ClosePath ()
		{
			writer.WriteLine ("\t\tp.CloseSubpath ();");
		}
	
		public void QuadCurveTo (PointF pt1, PointF pt2)
		{
			writer.WriteLine ("\t\tp.AddQuadCurveToPoint ({0}f, {1}f, {2}f, {3}f);", 
				pt1.X.ToString (CultureInfo.InvariantCulture), pt1.Y.ToString (CultureInfo.InvariantCulture),
				pt2.X.ToString (CultureInfo.InvariantCulture), pt2.Y.ToString (CultureInfo.InvariantCulture));
		}

		public void CurveTo (PointF pt1, PointF pt2, PointF pt3)
		{
			writer.WriteLine ("\t\tp.AddCurveToPoint ({0}f, {1}f, {2}f, {3}f, {4}f, {5}f);", 
				pt1.X.ToString (CultureInfo.InvariantCulture), pt1.Y.ToString (CultureInfo.InvariantCulture),
				pt2.X.ToString (CultureInfo.InvariantCulture), pt2.Y.ToString (CultureInfo.InvariantCulture),
				pt3.X.ToString (CultureInfo.InvariantCulture), pt3.Y.ToString (CultureInfo.InvariantCulture));
		}

		public void ArcTo (PointF size, float angle, bool isLarge, bool sweep, PointF endPoint, PointF startPoint)
		{
			this.ArcHelper (size, angle, isLarge, sweep, endPoint, startPoint);
		}
	}
}