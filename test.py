import winreg

def hive_name(hive):
	if hive == winreg.HKEY_CURRENT_USER:
		return "HKEY_CURRENT_USER"
	elif hive == winreg.HKEY_LOCAL_MACHINE:
		return "HKEY_LOCAL_MACHINE"
	elif hive == winreg.HKEY_CLASSES_ROOT:
		return "HKEY_CLASSES_ROOT"
	elif hive == winreg.HKEY_USERS:
		return "HKEY_USERS"
	elif hive == winreg.HKEY_PERFORMANCE_DATA:
		return "HKEY_PERFORMANCE_DATA"
	elif hive == winreg.HKEY_CURRENT_CONFIG:
		return "HKEY_CURRENT_CONFIG"
	else:
		return "UNKNOWN_HIVE"

def open_or_create_key(hive, path):
	try:
		# Open the registry key for reading and writing in 64-bit view
		key = winreg.OpenKey(hive, path, 0, winreg.KEY_READ | winreg.KEY_WRITE | winreg.KEY_WOW64_64KEY)
		print(f"Key opened: {hive_name(hive)}\\{path}")
	except FileNotFoundError:
		# Handle if the key doesn't exist
		print(f"Creating key: {hive_name(hive)}\\{path}")
		key = winreg.CreateKeyEx(hive, path, 0, winreg.KEY_READ | winreg.KEY_WRITE | winreg.KEY_WOW64_64KEY)
	except PermissionError:
		# Handle if there are permission issues
		print(f"Permission denied while accessing the key: {hive_name(hive)}\\{path}")
		key = None
	except Exception as e:
		# Handle any other exceptions
		print(f"An error occurred: {e}")
		key = None
	return key

def get_value(key,which):
	try:
		value, _ = winreg.QueryValueEx(key, which)
		print(f"Current value: {value}")
	except FileNotFoundError:
		print("Current value: <not set>")
	except Exception as e:
		print(f"An error occurred while querying the value: {e}")

def set_value(key,which,what):
	try:
		winreg.SetValueEx(key, which, 0, winreg.REG_DWORD, what)
		print (which, "was set to", what)
	except FileNotFoundError:
		print (which, "could not be set to", what)

def close_key(key):
	if key:
		winreg.CloseKey(key)
		print("Key closed.")

# Test the open_or_create_key function
if __name__ == "__main__":

	print("# This key does exist on my system and has tons of values")
	print("# Expected Output: Key opened: HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")
	key = open_or_create_key(winreg.HKEY_USERS, r"S-1-5-21-2848259066-2618722447-3046218285-1006\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")
	print("# Value name DisallowShaking is NOT in my registry.")
	print("# Expected Output: Current value: <not set> ")
	# Prevents the windows "feature" of minimizing everything if you "shake" a window while dragging. Does nothing if the value isn't actually set.
	get_value(key,"DisallowShaking")
	set_value(key,"DisallowShaking",1)
	get_value(key,"DisallowShaking")
	print("# Value name HideFileExt IS in my registry.")
	print("# Expected Output: HideFileExt set to X (where X is the value set in the code) - needs to be checked in the registry to see if it changed between runs")
	# enables or disables hiding of file extensions. 0 to not hide it.
	set_value(key,"HideFileExt",1)
	close_key(key)

	# This restores the Windows 10 right-click context menu to your system in Windows 11 if the key is present. It has no effect when left
	print("# Neither {86ca1aa0-34aa-4e8b-a509-50c905bae2a2} nor InprocServer32 exist in my registry.")
	print("# Expected Output: Creating Key: Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32")
	key = open_or_create_key(winreg.HKEY_USERS, r"S-1-5-21-2848259066-2618722447-3046218285-1006\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32")
	close_key(key)

	# Setting this key and vlue restores the Windows 10 Windows Explorer ribbon. 
	print("# The Blocked key does not exist in my registry.")
	print("# Expected Output: Creating Key: SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked")
	key = open_or_create_key(winreg.HKEY_USERS, r"S-1-5-21-2848259066-2618722447-3046218285-1006\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked")
	print("# If the key were created, then I can test for value {e2bf9676-5f8f-435c-97eb-11607a5bedf7} which should not exist yet.")
	print("# Expected Output: An error occurred while querying the value:  ")
	get_value(key,"{e2bf9676-5f8f-435c-97eb-11607a5bedf7}")
	set_value(key,"{e2bf9676-5f8f-435c-97eb-11607a5bedf7}", 1)
	close_key(key)
