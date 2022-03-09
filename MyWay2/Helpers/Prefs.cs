﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Paneless.Helpers
{
	class Prefs
	{
		Dictionary<string,string> userPrefs = new Dictionary<string, string>();
		public string settingsPath { get; } = "";
		public string settingsFile { get; } = "";
		public string settingsFullPath { get; } = "";


		public Prefs(string MyDocsPath)
		{
			settingsPath = Path.Combine(MyDocsPath, "Paneless").ToString();
			settingsFile = "prefs.txt";
			settingsFullPath = Path.Combine(settingsPath, settingsFile).ToString();
			LoadPrefs();
		}

		public void LoadPrefs()
		{
			// Make sure it's empty. Obviously will be on first, load, but we can load prefs later too
			userPrefs.Clear();
			if (PrefsFileExists())
			{
				string[] temp;
				string[] prefsLines = File.ReadAllLines(settingsFullPath);
				foreach (string aPref in prefsLines)
				{
					// The only input checking we use - is it a simple text=yes/no pattern?
					if (Regex.IsMatch(aPref, @"[A-Za-z]{6,80}=(yes|no)", RegexOptions.IgnoreCase))
					{
						temp = aPref.Split("=");
						userPrefs[temp[0]] = temp[1];
					}
				}
			}
		}

		public void SavePrefs(bool backup=false)
		{
			// creates if necessary, leaves it if it's already there
			Directory.CreateDirectory(settingsPath);
			List<string> lines = new List<string>();
			foreach (KeyValuePair<string,string> pref in userPrefs)
			{
				lines.Add(pref.Key + "=" + pref.Value);
			}
			if (!backup)
				File.WriteAllLinesAsync(settingsFullPath, lines);
			else
			{
				DateTime now = DateTime.Now;
				File.WriteAllLinesAsync(Path.Combine(settingsPath, now.ToString("yyyyMMMdd_HHmmss")+"-"+settingsFile).ToString(), lines);
			}
		}

		public void BackupPrefs()
		{
			SavePrefs(true);
		}

		public bool PrefsFileExists()
		{
			return File.Exists(settingsFullPath);
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
			if (userPrefs.ContainsKey(key))
				userPrefs[key] = value;
			else
				userPrefs.Add(key, value);
		}
	}
}
