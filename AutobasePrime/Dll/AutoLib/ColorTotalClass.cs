using System;
using System.Drawing;
using NetTools;
using AutoLibLocal;
using System.IO;

namespace AutoLib
{
	/// <summary>
	/// Summary description for ColorTotalClass.
	/// </summary>
	public class ColorTotalClass
	{
		public Color TEXT = Color.White;
		public Color BACK = Color.FromArgb(11, 74, 0x80);
		public Color TAG = Color.White;
		public Color DESCRIPTION = Color.White;
		public Color INACTIVE = Color.FromArgb(0x80, 0x80, 0x80);
		public Color HIHI = Color.FromArgb(0x80, 0, 0);
		public Color HIGH = Color.FromArgb(255, 0, 0);
		public Color LOW = Color.FromArgb(0, 0, 255);
		public Color LOLO = Color.FromArgb(0, 0, 0x80);
		public Color ON = Color.FromArgb(255, 0, 0);
		public Color OFF = Color.FromArgb(0x80, 0x80, 0x80);

		public ColorTotalClass()
		{
			//
			// TODO: Add constructor logic here
			//
			Load();
		}

		public void SetToDefault()
		{
			TEXT = Color.White;
            BACK = Color.FromArgb(11, 74, 0x80);
			TAG = Color.White;
			DESCRIPTION = Color.White;
			INACTIVE = Color.FromArgb(0x80, 0x80, 0x80);
			HIHI = Color.FromArgb(0x80, 0, 0);
			HIGH = Color.FromArgb(255, 0, 0);
			LOW = Color.FromArgb(0, 0, 255);
			LOLO = Color.FromArgb(0, 0, 0x80);
			ON = Color.FromArgb(255, 0, 0);
			OFF = Color.FromArgb(0x80, 0x80, 0x80);
		}

		void Load()
		{
			string cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			string filename = String.Format("{0}\\ColorTotal\\ColorTotal.cfg", cfg_dir);

			if(!File.Exists(filename))	return;

			TextReader reader = new StreamReader(filename);

			if(reader == null)	return;

			string one_line;
			CommaBlockString comma = new CommaBlockString();
			string command = "";

			while(true) 
			{
				one_line = reader.ReadLine();
				if(one_line == null)	break;
				comma.Set(one_line);
				comma.GetString(ref command);

				if(command == "TEXT")		comma.GetColorFromARGB(ref TEXT);
				else if(command == "BACK")	comma.GetColorFromARGB(ref BACK);
				else if(command == "TAG")	comma.GetColorFromARGB(ref TAG);
				else if(command == "DESCRIPTION")	comma.GetColorFromARGB(ref DESCRIPTION);
				else if(command == "INACTIVE")	comma.GetColorFromARGB(ref INACTIVE);
				else if(command == "HIHI")	comma.GetColorFromARGB(ref HIHI);
				else if(command == "HIGH")	comma.GetColorFromARGB(ref HIGH);
				else if(command == "LOW")	comma.GetColorFromARGB(ref LOW);
				else if(command == "LOLO")	comma.GetColorFromARGB(ref LOLO);
				else if(command == "ON")	comma.GetColorFromARGB(ref ON);
				else if(command == "OFF")	comma.GetColorFromARGB(ref OFF);
				else {}
			}

			reader.Close();
		}

		public void Save()
		{
			string cfg_dir = TotalConfig.AutoBaseIniGetConfigDirectory();
			string filename = String.Format("{0}\\ColorTotal", cfg_dir);

			Directory.CreateDirectory(filename);

			filename = String.Format("{0}\\ColorTotal\\ColorTotal.cfg", cfg_dir);

			TextWriter writer = new StreamWriter(filename);

			if(writer == null)	return;			

			Color color;
			color = TEXT;
			writer.WriteLine("TEXT,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = BACK;
			writer.WriteLine("BACK,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = TAG;
			writer.WriteLine("TAG,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = DESCRIPTION;
			writer.WriteLine("DESCRIPTION,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = INACTIVE;
			writer.WriteLine("INACTIVE,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = HIHI;
			writer.WriteLine("HIHI,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = HIGH;
			writer.WriteLine("HIGH,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = LOW;
			writer.WriteLine("LOW,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = LOLO;
			writer.WriteLine("LOLO,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = ON;
			writer.WriteLine("ON,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			color = OFF;
			writer.WriteLine("OFF,{0},{1},{2},{3},", color.A, color.R, color.G, color.B);
			
			writer.Close();
		}
	}
}
