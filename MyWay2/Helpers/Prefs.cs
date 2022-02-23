using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Paneless.Helpers
{
	class Prefs
	{
		Dictionary<string,string> userPrefs = new Dictionary<string, string>();
		public string settingsPath { get; } = "";
		public string settingsFile { get; } = "";


		public Prefs(string MyDocsPath)
		{
			settingsPath = Path.Combine(MyDocsPath, "Paneless").ToString();
			settingsFile = "prefs.txt";
			LoadPrefs();
		}

		public void LoadPrefs()
		{
			string fullPath = settingsPath + "\\" + settingsFile;
			if (File.Exists(fullPath))
			{
				string[] temp;
				string[] prefsLines = File.ReadAllLines(fullPath);
				foreach (string aPref in prefsLines)
				{
					temp = aPref.Split("=");
					userPrefs[temp[0]] = temp[1];
				}
			}
		}

		public void SavePrefs()
		{
			// Creates if necessary, ignores if already there
			string fullPath = settingsPath + "\\" + settingsFile;
			Directory.CreateDirectory(settingsPath);
			List<string> lines = new List<string>();
			foreach (KeyValuePair<string,string> pref in userPrefs)
			{
				lines.Add(pref.Key + "=" + pref.Value);
			}
			File.WriteAllLinesAsync(fullPath, lines);
		}

		public string GetAllPrefs()
		{
			return string.Join(Environment.NewLine, userPrefs);
		}

		public string GetPref(string name)
		{
			string pref = "";
			if (userPrefs.TryGetValue(name, out pref))
				return pref;
			return "";
		}

		public void SetPref(string key, string value)
		{
			userPrefs[key] = value;
			SavePrefs();
		}
	}
}
