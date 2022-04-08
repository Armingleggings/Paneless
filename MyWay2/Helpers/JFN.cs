using System;
using System.Collections.Generic;
using System.Text;

namespace Paneless.Helpers
{
	// JFN - Jeremy Functions :)
	class JFN
	{

		// Used to clean up multi-line strings that I write in code with excess tabs and newlines (which I do to keep it more readable in code).
		public string ClearWS(string str)
		{
			// There were too many times I pressed enter without adding a trailing space so this fixes that.
			str = str.Replace(System.Environment.NewLine, " ");
			// But if I did use a trailing speace, now they are double spaces. Fix that
			str = str.Replace("  ", " ");
			str = str.Replace("\t", "");
			// No tabs.
			return str.Trim();
		}

	}
}
