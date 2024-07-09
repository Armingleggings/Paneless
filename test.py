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
	
def open_key(hive, path, create=None):
	try:
		# Open the registry key for reading and writing
		key = winreg.OpenKeyEx(hive, path, 0, winreg.KEY_ALL_ACCESS | winreg.KEY_WOW64_64KEY)
		print(f"Key opened: {hive_name(hive)}\\{path}")
	except FileNotFoundError:
		# Handle if the key doesn't exist
		if create:
			print(f"Creating key: {hive_name(hive)}\\{path}")
			key = winreg.CreateKeyEx(hive, path, 0, winreg.KEY_ALL_ACCESS | winreg.KEY_WOW64_64KEY)
		else:
			print(f"Key not found (not creating): {hive_name(hive)}\\{path}")
			return None
	except PermissionError:
		# Handle if there are permission issues
		print(f"Permission denied while accessing the key: {hive_name(hive)}\\{path}")
		return None
	except Exception as e:
		# Handle any other exceptions
		print(f"On: {hive_name(hive)}\\{path}")
		print(f"An error occurred: {e}")
		return None
	return key

key = open_key(winreg.HKEY_CURRENT_USER,r"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32")

current_value, _ = winreg.QueryValueEx(key, "")
print(current_value)