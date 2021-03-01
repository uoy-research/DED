
# -*- coding: iso-8859-1 -*-
import clr
import sys
sys.path.append('C:\SecondLife')
sys.path.append('C:\Program Files\MSBuild\Microsoft\IronPython')
sys.path.append('C:\Program Files\MSBuild\Microsoft\IronPython\1.0')
#sys.path.append('C:\\Python25\\DLLs')
#sys.path.append('C:\\Python25\\lib')

#sys.path.append('C:\\Python25\\lib\\plat-win')
#sys.path.append('C:\\Python25\\lib\\lib-tk')
#sys.path.append('C:\\Python25')
#sys.path.append('C:\\Python25\\lib\\site-packages\\wx-2.8-msw-unicode')
#['C:\\Python25\\Lib\\idlelib', 'C:\\WINDOWS\\system32\\python25.zip', 'C:\\Python25\\DLLs', 'C:\\Python25\\lib'
#, 'C:\\Python25\\lib\\plat-win', 'C:\\Python25\\lib\\lib-tk', 'C:\\Python25', 'C:\\Python25\\lib\\site-packages', 'C:\\Python25\\lib\\site-packages\\wx-2.8-msw-unicode']
try: 
    import time
except ImportError: print ImportError.msg
print sys.path
print 'About to import string'
#import string
clr.AddReferenceToFile("libsecondlife.dll")
print 'About to import libsecondlife'

import libsecondlife
print 'imported libsecondlife'
# Replace credentials with the firstname, lastname and password of an SL account
credentials=['BotFirstName','BotLastName','BotPassword']
currentname=""
 
class EventTracker:
    def __init__(self):
        self.data=False
    
    def Clear(self):
        self.data=False
        
    def Set(self):
        self.data=True
        
    def Wait(self):
        while not self.data:
            time.sleep(0.1)
            
    # Show a UUID in a familiar, friendly format
    def format_uuid(id): return "%s-%s-%s-%s-%s" %(id[0:8],id[8:12],id[12:16],id[16:20],id[20:])
    
    # Handle the results of the people search
    def DirQueryHandler(queryid, results):
        global currentname
        global event
        print currentname,format_uuid(results[0].AgentID.ToString())
        event.Set()
        event=EventTracker()
        client=libsecondlife.SecondLife()
        client.Directory.OnDirPeopleReply += libsecondlife.DirectoryManager.DirPeopleReplyCallback(DirQueryHandler)
        
        qlist=['Tateru Nino','Torley Linden']
        if not client.Network.Login(credentials[0], credentials[1], credentials[2], "example", "yourname@here.com"):
            print ("ERROR: " + client.Network.LoginMessage)
            sys.exit(0)
            
        for i in qlist:
            i = strip(i)
            currentname = i
            (fname,lname) = string.split(i," ")
            event.Clear()
            queryID = client.Directory.StartPeopleSearch(libsecondlife.DirectoryManager.DirFindFlags.People, fname+" "+lname, 0)
            event.Wait()
            
        client.Network.Logout()