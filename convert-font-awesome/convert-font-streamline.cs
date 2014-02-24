// Authors:
//	Sebastien Pouliot  <sebastien@xamarin.com>
//
// Copyright 2012-2013 Xamarin Inc.
//
// Licensed under the GNU LGPL 2 license only (no "later versions")

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Poupou.SvgPathConverter;

// This sample shows how you can use the library to converts every SVG path inside FontAwesome into a MonoTouch.Dialog
// based application to show them all. Since MonoTouch uses C# and iOS is CoreGraphics based then the parameters (and
// the extra code generation) are hardcoded inside the sample.

class Program {

	static void Usage (string error, params string[] values)
	{
		Console.WriteLine ("Usage: convert-font-streamline <font-directory> [generated-file.cs]");
		if (error != null)
			Console.WriteLine (error, values);
		Environment.Exit (1);
	}

	public static int Main (string[] args)
	{
		if (args.Length < 1)
			Usage ("error: Path to streamline directory required");

		string font_dir = args [0];
		if( string.IsNullOrEmpty(font_dir) )
			font_dir = "font-streamlines";

		string css_file = Path.Combine (font_dir, "styles.css");
		if (!File.Exists (css_file))
			Usage ("error: Missing '{0}' file.", css_file);

		string[] svgFiles = System.IO.Directory.GetFiles(font_dir, "*.svg", SearchOption.AllDirectories);
		if( svgFiles.Length == 0 )
			Usage ("error: Can not found any svg file.");
		string svg_file = svgFiles[0];


		var output = args [1];
		if( string.IsNullOrEmpty(output) )
			output = "StreamlineIcons.generated.cs";
		TextWriter writer = /*(args.Length < 2) ? Console.Out :*/ new StreamWriter (output);
		writer.WriteLine ("// note: Generated file - do not modify - use convert-font-streamline to regenerate");
		writer.WriteLine ();
		writer.WriteLine ("using MonoTouch.CoreGraphics;");
		writer.WriteLine ("using MonoTouch.Dialog;");
		writer.WriteLine ("using MonoTouch.Foundation;");
		writer.WriteLine ("using MonoTouch.UIKit;");
		writer.WriteLine ();
		writer.WriteLine ("namespace UI.Icons {");
		writer.WriteLine ();
		writer.WriteLine ("\t[Preserve]");
		writer.WriteLine("\tpublic partial class StreamlineIcons {");

		Dictionary<int, string> names = new Dictionary<int, string>();
		string[] lines = File.ReadLines(css_file).ToArray();
		for (int i = 0; i < lines.Length; i++) {
			string line = lines[i];
			// get value first
			int p = line.IndexOf("content: \"", StringComparison.Ordinal);
			if (p == -1)
				continue;
			int begin = line.IndexOf('\"');
			string _value = line.Substring(begin + 1, line.LastIndexOf('\"') - begin - 1);
			int value = 0;
			if( _value.StartsWith("\\e") )
				value = Convert.ToInt32(_value.Substring(1), 16);
			else
				value = (int)_value[0];

			// get names
			string name = null;
			if (names.ContainsKey(value))
				name = names[value];
			for (int j = i - 1; j >= 0; j--)
			{
				line = lines[j];
				if (!line.StartsWith(".icon-", StringComparison.Ordinal))
					break;

				p = line.IndexOf(':');
				if (p == -1)
					break;
				string curName = line.Substring(1, p - 1).Replace('-', '_');
				if (name == null)
				{
					name = curName;
					names.Add(value, name);
				}

				writer.WriteLine("\t\t// {0} : {1}", name, value);
				writer.WriteLine("\t\tpublic UIImage {0} {{ get {{ return Get ({1}_path); }} }}", curName, name);
				writer.WriteLine();
			}
		}
		writer.WriteLine ("\t\t// total: {0}", names.Count);
		writer.WriteLine ();

		// MonoTouch uses C# and CoreGraphics
		var code = new CSharpCoreGraphicsFormatter (writer);
		var parser = new SvgPathParser () {
			Formatter = code
		};

		foreach (string line in File.ReadLines (svg_file)) {
			if (!line.StartsWith ("<glyph unicode=\"&#", StringComparison.Ordinal))
				continue;
			int begin = line.IndexOf('#');
			string _id = line.Substring(begin + 1, line.IndexOf(';') - begin - 1);
			int id = Convert.ToInt32(_id, 10);
			string name;
			if (!names.TryGetValue(id, out name))
				continue;
			int p = line.IndexOf (" d=\"") + 4;
			int e = line.LastIndexOf ('"');
			string data = line.Substring (p, e - p);
			parser.Parse(data, name);
		}
		writer.WriteLine ("\t}");
		writer.WriteLine ("}");
		writer.Close ();

		return 0;
	}
}