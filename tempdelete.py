import ctypes
import ctypes.wintypes as wintypes
import winreg

ntdll = ctypes.WinDLL('ntdll')

ObjectNameInformation = 1
OBJECT_INFORMATION_CLASS = wintypes.DWORD

class UNICODE_STRING(ctypes.Structure):
    _fields_ = (('Length',        wintypes.USHORT),
                ('MaximumLength', wintypes.USHORT),
                ('Buffer',        ctypes.POINTER(wintypes.WCHAR)))
    @property
    def value(self):
        length = self.Length // ctypes.sizeof(wintypes.WCHAR)
        if length and self.Buffer:
            if self.Buffer[length - 1] == '\x00':
                length -= 1
            return self.Buffer[:length]
        return ''

def _check_status(status, func, args):
    if status < 0:
        raise ctypes.WinError(ntdll.RtlNtStatusToDosError(status))
    return args

ntdll.NtQueryObject.errcheck = _check_status
ntdll.NtQueryObject.argtypes = (
    wintypes.HANDLE,          # Handle
    OBJECT_INFORMATION_CLASS, # ObjectInformationClass
    wintypes.LPVOID,          # ObjectInformation
    wintypes.ULONG,           # ObjectInformationLength
    wintypes.PULONG)          # ReturnLength

def get_object_name(handle):
    buf = (wintypes.CHAR * 65536)()
    ntdll.NtQueryObject(handle, ObjectNameInformation, buf,
                        ctypes.sizeof(buf), None)
    return UNICODE_STRING.from_buffer(buf).value

def open_or_create_key(hkey, subkey, access=winreg.KEY_READ):
    try:
        return winreg.OpenKey(hkey, subkey, 0, access)
    except FileNotFoundError:
        return winreg.CreateKeyEx(hkey, subkey, 0, access)

subkey_list = [
    r'Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}'
        r'\InprocServer32',
    r'Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced',
    r'Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked',
]

hkey = winreg.HKEY_CURRENT_USER
access = winreg.KEY_READ | winreg.KEY_WRITE | winreg.KEY_WOW64_64KEY
for subkey in subkey_list:
    with open_or_create_key(hkey, subkey, access) as hsubkey:
        print(get_object_name(hsubkey.handle))