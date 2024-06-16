from flask import Flask, render_template, request, jsonify
import winreg

app = Flask(__name__)

fixes = {
	"welcome": {
		"pref_name": "WelcomeExperienceAfterEveryDamnedUpdate",
		"img": "graphics/windows_welcome.png",
		"title": "Disable 'Welcome to Windows' after updates",
		"snark": """
			Another volley in Microsoft's cloying desperation to trick you into picking Edge as your browser; the Welcome to Windows screen that shows up after updates is clearly intended into 
			confusing you into selecting their browser (and other options that benefit Microsoft) instead of leaving your crap alone... the way you already set it.
			""",				
		"description": """
			On major updates to Windows, it shows you the Welcome Experience again in an attempt to give you a second chance to do things their way. If you don't need the nag, 
			click this fix to disable this obnoxious behavior.
			""",
		"tags": ["#Windows", "#Update", "#Nags", "#PweaseUseEdgeSenpaiUwU", "#TrickGrandma"],
		"reg_fix": {
			"key_path": r"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", 
			"value_name": "SubscribedContent-310093Enabled",
			"fixed_val": 0, 
			"broke_val": 1,
		}
	},	
}

# -1 means break it
# 0 or None means "get"
# 1 means fix it
# returns 
#   on get: the value
#	on set: "changed", "same", or "error"
def reg_fixer(fixes, which, fix_it=None):
	reg_fix = fixes[which]["reg_fix"]

	try:
		# Open the registry key for reading and writing
		key = winreg.OpenKey(winreg.HKEY_CURRENT_USER, reg_fix["key_path"], 0, winreg.KEY_READ | winreg.KEY_WRITE)
	except FileNotFoundError:
		# Handle if the key doesn't exist
		key = winreg.CreateKey(winreg.HKEY_CURRENT_USER, reg_fix["key_path"])
	
	# Get the previous value of the registry key
	try:
		current_value, _ = winreg.QueryValueEx(key, reg_fix["value_name"])
		current_value = int(current_value)  # Convert value to integer
	except FileNotFoundError:
		current_value = None  # No previous value if the value doesn't exist

	# GET OPERATION STOPS HERE!!!
	if fix_it is None:
		winreg.CloseKey(key)
		return {"current": current_value, "fixed_val": reg_fix["fixed_val"], "broke_val": reg_fix["broke_val"], "is_fixed": (current_value == reg_fix["fixed_val"])}

	# POST OPERATION (fix_it is not None)
	note = None
	try:
		change_to = reg_fix["fixed_val"] if (fix_it == 1) else reg_fix["broke_val"]
		if change_to == current_value:
			note = "no change"
		else:
			# Set the new value for the registry key
			winreg.SetValueEx(key, reg_fix["value_name"], 0, winreg.REG_DWORD, change_to)
			# Another GET just to be sure the change worked
			current_value, _ = winreg.QueryValueEx(key, reg_fix["value_name"])
			current_value = int(current_value)  # Convert value to integer
			if current_value != change_to:
				note = "change failed!"
	except Exception as e:
		# If the update fails, set the previous and current values to be the same
		print(f"Error setting registry value: {e}")
		note = f"Error setting registry value: {e}"

	winreg.CloseKey(key)
	return {"current": current_value, "fixed_val": reg_fix["fixed_val"], "broke_val": reg_fix["broke_val"], "is_fixed": (current_value == reg_fix["fixed_val"]), "note": note}
		
settings = {}

for key, a_fix in fixes.items():
	settings[a_fix["pref_name"]] = reg_fixer(fixes,key)["is_fixed"]

@app.route('/')
def home_page():
	return render_template("index.html", fixes=fixes, settings=settings)

@app.route('/toggle_fix', methods=['POST'])
def toggle_fix():
	global fixes,settings
	
	data = request.get_json()
	fix_key = data.get('fix_key')
	is_fixed = data.get('fixed')

	# undo the fix if it IS fixed otherwise fix it
	result = reg_fixer(fixes,fix_key,-1 if is_fixed else 1)
	
	response = {'status': 'success', 'message': {"text":f'Toggled fix {fix_key}', 'fixed': result["is_fixed"], 'fun_return': result }}
	return jsonify(response)