import clr
import sys
import time
#import string

clr.AddReferenceToFile("libsecondlife.dll")
import libsecondlife
print dir(libsecondlife)
print 'imported lib'
x = raw_input()

clr.AddReferenceToFile("smilenet.dll")
import Smile
print dir(Smile.Network)
# Replace credentials with the firstname, lastname and password of an SL account
credentials=['BotFirstName','BotLastName','BotPassword']
currentname=""
print credentials
client=libsecondlife.SecondLife()

x = raw_input()
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
            i = string.strip(i)
            currentname = i
            (fname,lname) = string.split(i," ")
            event.Clear()
            queryID = client.Directory.StartPeopleSearch(libsecondlife.DirectoryManager.DirFindFlags.People, fname+" "+lname, 0)
            event.Wait()
            
        client.Network.Logout()